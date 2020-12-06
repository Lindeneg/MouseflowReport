using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

using MouseflowReport.Util;


namespace MouseflowReport.Core
{
    public class Report
    {
        private Config                                _config;
        private string                                _websiteId;
        private List<Row>                             _rows;
        private Row                                   _totalRow;
        private int                                   _count;
        private List<string>                          _urls;
        private List<Dictionary<string, JsonElement>> _recordings;

        /// <summary>
        /// Class to contain Report information for website.
        /// </summary>
        /// <param name="config">      Config instance to be used.</param>
        /// <param name="websiteId">   String with website Mouseflow ID.</param>
        /// <param name="rowIncrement">Int specifying difference in days between Report rows.</param>
        /// <returns></returns>
        public Report(Config config, string websiteId, int rowIncrement = Var.ROW_INCREMENT)
        {

            Output.Print($"initializing report for website '{websiteId}'");

            this._websiteId           = websiteId;
            this._config              = config;
            this._rows                = Row.CreateRowsFromIncrementalDateRange(
                this.Config.FromDate, 
                this.Config.ToDate, 
                rowIncrement
            );
            this._totalRow            = new Row(this.Config.FromDate, this.Config.ToDate);
            this._count               = 0;
            this._urls                = new List<string>();
            this._recordings          = new List<Dictionary<string, JsonElement>>();
        }

        public string    WebsiteId { get { return this._websiteId; }}
        public Config    Config    { get { return this._config; }}
        public List<Row> Rows      { get { return this._rows; }}
        public Row       TotalRow  { get { return this._totalRow; }}

        /// <summary>
        /// Generates an entire Report ready to be saved as a file when Task is resolved.
        /// </summary>
        /// <returns>
        /// Task of the Report instance itself.
        /// </returns>
        async public Task<Report> Init()
        {
            this._count = await this._GetWebsiteRecordingCount();
            this._urls  = APIHTTPRequest.GetURLFormattedQueryList(
                this.WebsiteId, 
                this._count, 
                this.Config
            );

            if (!(this._count > 0) || !(this._urls.Count > 0))
            {
                Output.Print($"specified website '{this.WebsiteId}' contains no session entries", state: "error");
                return this;
            }

            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (string url in this._urls)
            {
                tasks.Add(this._RequestRecordingURL(url));
            }

            while (tasks.Count > 0)
            {
                for (int i = 0; i < tasks.Count; i++) {
                    if (tasks[i].IsCompleted) {
                        tasks.Remove(tasks[i]);
                    }
                }
            }
            this._GenerateReport();
            return this;
        }

        /// <summary>
        /// Save Report as CSV to specified path.
        /// </summary>
        /// <param name="path">String with path to save Report to.</param>
        /// <returns></returns>
        public void SaveReportToDisk(string path)
        {
            Output.Print($"saving report to path: {path}/{this.WebsiteId}.csv");
            if (this.Config.IncludeTotalRow)
            {
                this._rows.Insert(0, this._totalRow);
            }
            ReportTable reportTable = new ReportTable(this);
            reportTable.ConvertToCSVFormat();
            reportTable.SaveToDisk(path);
        }

        /// <summary>
        /// Parse each recording and increment appropriate Row values.
        /// </summary>
        /// <returns></returns>
        private void _GenerateReport()
        {
            Output.Print($"parsing '{this._recordings.Count}' recording{Output.GetPlural(this._recordings.Count)} for website '{this.WebsiteId}'");
            if (this._recordings.Count > 0)
            {
                foreach (var recording in this._recordings)
                {
                    if (recording.ContainsKey("created"))
                    {
                        int rowIndex = this._GetRowIndexFromRecordingCreatedString((recording["created"].GetString()));
                        if (rowIndex > -1)
                        {
                            this.Rows[rowIndex].IncrementRow(recording, this.Config.ConvertMsToMin);
                            if (this.Config.IncludeTotalRow)
                            {
                                this.TotalRow.IncrementRow(recording, this.Config.ConvertMsToMin);
                            }
                        }
                    }
                }
                Output.Print($"parse completed");
            }
        }

        /// <summary>
        /// Get amount of recordings associated with Report website.
        /// </summary>
        /// <returns>
        /// Task of int holding the recording count.
        /// </returns>
        async private Task<int> _GetWebsiteRecordingCount()
        {
            string baseURL                       = APIHTTPRequest.GetBaseURL(this.Config.Location);
            Dictionary<string, string> queries   = new Dictionary<string, string>{
                {"fromDate", Date.ToString(this.Config.FromDate)}, 
                {"toDate", Date.ToString(this.Config.ToDate)}
            };
            string fullURL                       = APIHTTPRequest.BuildURL(baseURL, $"websites/{this.WebsiteId}/recordings", queries);
            APIHTTPRequest httpRequest           = new APIHTTPRequest(fullURL, this.Config.User, this.Config.Key);
            Dictionary<string, dynamic> response = await httpRequest.GetJSONAPIResponse();
            int count                            = response.ContainsKey("count") ? response["count"].GetInt32() : 0;
            
            Output.Print($"found '{count}' recording{Output.GetPlural(count)} for website '{this.WebsiteId}'");
            return count;
        }

        /// <summary>
        /// Match recording created time with appropriate Row and return Row index.
        /// </summary>
        /// <param name="stringCreated">String with recording created time.</param>
        /// <returns>
        /// Int holding index of matched Row. Returns a non-positive int if no match.
        /// </returns>
        private int _GetRowIndexFromRecordingCreatedString(string stringCreated)
        {
            if (Date.IsString(stringCreated))
            {
                DateTime dateCreated = Date.FromString(stringCreated);
                int i = 0;
                foreach (Row row in this.Rows)
                {
                    if (Date.IsWithinRange(dateCreated, row["from"], row["to"]))
                    {
                        return i;
                    }
                    i++;
                }
            }
            Output.Print($"could not find row for '{stringCreated}'", state: "error");
            return -1;
        }

        /// <summary>
        /// Send GET request to specified recording URL
        /// </summary>
        /// <param name="url">String with recordings uRL.</param>
        /// <returns></returns>
        async private Task<bool> _RequestRecordingURL(string url)
        {

            APIHTTPRequest httpRequest = new APIHTTPRequest(url, this.Config.User, this.Config.Key);
            Dictionary<string, dynamic> response = await httpRequest.GetJSONAPIResponse();
            if (response.ContainsKey("recordings"))
            {
                string stringifiedRecordings = JsonSerializer.Serialize(response["recordings"], typeof(JsonElement));
                List<Dictionary<string, JsonElement>> recordings = (List<Dictionary<string, JsonElement>>)JsonSerializer.Deserialize(stringifiedRecordings, typeof(List<Dictionary<string, JsonElement>>));
                this._AppendRecordingsToList(recordings);
            }
            return true;
        }

        /// <summary>
        /// Append API response of recordings to internal container.
        /// </summary>
        /// <param name="recordings">List of recordings to append.</param>
        /// <returns></returns>
        private void _AppendRecordingsToList(List<Dictionary<string, JsonElement>> recordings)
        {
            foreach (Dictionary<string, JsonElement> recording in recordings)
            {
                this._recordings.Add(recording);
            }
        }
    }
}
