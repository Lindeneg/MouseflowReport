using System;
using System.Data;
using System.Text;
using System.IO;
using System.Collections.Generic;

using MouseflowReport.Util;


namespace MouseflowReport.Core
{
    /* 
    Really stupid here, no reason at all to first convert to DataTable and then create a CSV string.
    The CSV string can be created as the first thing from the Report itself.. duh.. will redo at some point.
    */
    public class ReportTable
    {
        private Report        _report;
        private DataTable     _dataTable;
        private StringBuilder _stringBuilder;
        private string        _timeUnitString;

        /// <summary>
        /// Class to convert Report instance to CSV file format.
        /// </summary>
        /// <param name="report">Report instance to convert.</param>
        /// <returns></returns>
        public ReportTable(Report report)
        {
            this._report         = report;
            this._dataTable      = new DataTable(this._report.WebsiteId);
            this._stringBuilder  = new StringBuilder();
            this._timeUnitString = this._report.Config.ConvertMsToMin ? "_min" : "_ms";
        }

        /// <summary>
        /// Add rows and columns to DataTable. Then build CSV string.
        /// </summary>
        /// <returns></returns>
        public void ConvertToCSVFormat()
        {
            this._dataTable.Columns.Add("fromDate", typeof(string));
            this._dataTable.Columns.Add("toDate",   typeof(string));
            foreach ((string, string, ColumnDefaultValue, ColumnAction) info in Var.columnInfo)
            {
                string key = info.Item1;
                if (ReportTable.IsTimeMeasureKey(info.Item4, info.Item2))
                {
                    key += this._timeUnitString;
                }
                this._dataTable.Columns.Add(key, typeof(string));
            }
            this._AddRows();
            this._BuildCSVString();
        }

        /// <summary>
        /// Save finished CSV string to local disk.
        /// </summary>
        /// <param name="path">String with path to output directory.</param>
        /// <returns></returns>
        public void SaveToDisk(string path)
        {
            StreamWriter streamWriter = new StreamWriter($"{path}/{this._report.WebsiteId}.csv");
            streamWriter.WriteLine(this._stringBuilder);
            streamWriter.Close();
        }

        /// <summary>
        /// Extract highest key:value pair of Dictionary if appropriate Config flag is set.
        /// </summary>
        /// <param name="mostSeen">Dictionary of string:int pairs to parse.</param>
        /// <returns>
        /// Dictionary of highest key:value pair if appropriate Config flag is set else <paramref name="mostSeen"/>.
        /// </returns>
        public Dictionary<string, int> ParseMostSeen(Dictionary<string, int> mostSeen)
        {
            if (this._report.Config.KeepMostSeenMaps) { return mostSeen; }
            string key = "None";
            int val = 0;
            foreach (string cKey in mostSeen.Keys)
            {
                int cValue = (int)mostSeen[cKey];
                if (cValue > val)
                {
                    key = cKey; 
                    val = cValue;
                }
            }
            return new Dictionary<string, int>{{key, val}};
        }

        /// <summary>
        /// Stringifies a Dictionary.
        /// </summary>
        /// <param name="mostSeen">Dictionary to stringify.</param>
        /// <returns>
        /// String version of <paramref name="mostSeen"/>.
        /// </returns>
        public string MostSeenMapToString(Dictionary<string, int> mostSeen)
        {
            string                  result         = "";
            Dictionary<string, int> mostSeenParsed = this.ParseMostSeen(mostSeen);
            int i                                  = 0;

            foreach (string key in mostSeenParsed.Keys)
            {
                result += $"{key}={mostSeenParsed[key]}";

                if (i < mostSeenParsed.Count - 1)
                {
                    result += "|";
                }
                i++;
            }
            return result;
        }

        /// <summary>
        /// Add each Row in Report to DataTable.
        /// </summary>
        /// <returns></returns>
        private void _AddRows()
        {
            foreach (Row row in this._report.Rows)
            {
                if (row["totalSessions"] <= 0 && this._report.Config.RemoveEmptyRows) { continue; }

                DataRow dataRow = this._dataTable.NewRow();

                dataRow["fromDate"] = Date.ToString(row["from"]);
                dataRow["toDate"]   = Date.ToString(row["to"]);

                foreach ((string, string, ColumnDefaultValue, ColumnAction) info in Var.columnInfo)
                {
                    (string colKey, string altKey, ColumnDefaultValue defaultVal, ColumnAction action) = info;
                    switch (action)
                    {
                        case ColumnAction.IncrementDict:
                        case ColumnAction.HandleIterable:
                            dataRow[colKey] = this.MostSeenMapToString(row[colKey]);
                            break;
                        case ColumnAction.PerformAverage:
                        case ColumnAction.IncrementSum:
                            string key = ReportTable.IsTimeMeasureKey(action, altKey) ? colKey + this._timeUnitString : colKey;
                            dataRow[key] = row[colKey].ToString();
                            break;
                        default:
                            dataRow[colKey] = row[colKey].ToString();
                            break;
                    }
                }
                this._dataTable.Rows.Add(dataRow);
            }
        }

        /// <summary>
        /// Convert DataTable to CSV formatted string.
        /// </summary>
        /// <returns></returns>
        private void _BuildCSVString()
        {
            for (int i = 0; i < this._dataTable.Columns.Count; i++)
            {
                if (i == this._dataTable.Columns.Count - 1)
                    this._stringBuilder.Append(this._dataTable.Columns[i].ColumnName.ToString().Replace(",", ";"));
                else
                    this._stringBuilder.Append(this._dataTable.Columns[i].ColumnName.ToString().Replace(",", ";") + ',');
            }
            this._stringBuilder.Append(Environment.NewLine);
            for (int i = 0; i < this._dataTable.Rows.Count; i++)
            {
                for (int j = 0; j < this._dataTable.Columns.Count; j++)
                {
                    if (j == this._dataTable.Columns.Count - 1)
                        this._stringBuilder.Append(this._dataTable.Rows[i][j].ToString().Replace(",", ";"));
                    else
                        this._stringBuilder.Append(this._dataTable.Rows[i][j].ToString().Replace(",", ";") + ',');
                }
                if (i != this._dataTable.Rows.Count - 1)
                    this._stringBuilder.Append(Environment.NewLine);
            }
        }

        /// <summary>
        /// Check if given action and key is time measure unit.
        /// </summary>
        /// <param name="action">ColumnAction to check.</param>
        /// <param name="altKey">String with key to check.</param>
        /// <returns>
        /// Bool descriping if time measure unit.
        /// </returns>
        public static bool IsTimeMeasureKey(ColumnAction action, string altKey)
        {
            return (
                (action.Equals(ColumnAction.PerformAverage) || action.Equals(ColumnAction.IncrementSum)) 
                && !(altKey.Equals("pages")) && !(altKey.Equals("totalPageViews"))
            );
        }
    }
}
