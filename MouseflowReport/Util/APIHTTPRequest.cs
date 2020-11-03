using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

using MouseflowReport.Core;


namespace MouseflowReport.Util
{
    public class APIHTTPRequest
    {
        private string            url;
        private HttpClientHandler handler;
        private HttpClient        client;

        /// <summary>
        /// Class to perform requests to Mouseflow REST API.
        /// </summary>
        /// <param name="url">  String with URL to send GET request to.</param>
        /// <param name="user"> String with Mouseflow API username.</param>
        /// <param name="token">String with Mouseflow API token.</param>
        /// <returns></returns>
        public APIHTTPRequest(string url, string user, string token)
        {
            this.url                 = url;
            this.handler             = new HttpClientHandler();
            this.handler.Credentials = new NetworkCredential(user, token);
            this.client              = new HttpClient(this.handler);

        }

        /// <summary>
        /// Perform GET request to Mouseflow REST API and serialize the JSON response.
        /// </summary>
        /// <returns>
        /// Task with Dictionary of API response if StatusCode is 2**.
        /// Throws HttpRequestException on StatusCode 400/401/403/404/429/500/503.
        /// </returns>
        async public Task<Dictionary<string, dynamic>> GetJSONAPIResponse()
        {
            string reqStr = this.url.Split("/websites")[1];
            Output.Print($"request: '{reqStr}'");
            HttpResponseMessage response = await this.client.GetAsync(this.url);
            switch(response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw new HttpRequestException($"bad request (400) -> '{this.url}'");

                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.Forbidden:
                    throw new HttpRequestException($"request rejected (401/403) -> API-key likely invalid");

                case HttpStatusCode.NotFound:
                    throw new HttpRequestException($"request target not found (404) -> '{this.url}'");

                case HttpStatusCode.TooManyRequests:
                    throw new HttpRequestException($"request rejected (429) -> too many requests from specified API-key");

                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.ServiceUnavailable:
                    throw new HttpRequestException("request not handled (500/503) -> mouseflow servers seem to be down");

                default:
                    break;
            }
            Output.Print($"response: '{response.StatusCode}' for '{reqStr}'");
            string jsonString = await response.Content.ReadAsStringAsync();
            return (Dictionary<string, dynamic>)JsonSerializer.Deserialize(jsonString, typeof(Dictionary<string, dynamic>));
        }

        /// <summary>
        /// Get List of strings with formatted URLs necessary to fetch all recordings.
        /// </summary>
        /// <param name="websiteId">String with Mouseflow website ID</param>
        /// <param name="count">    Int with recording count</param>
        /// <param name="config">   Config instance to be used</param>
        /// <returns>
        /// List of strings with formatted URLs necessary to fetch all recordings for website <paramref name="websiteId"/>.
        /// </returns>
        public static List<string> GetURLFormattedQueryList(string websiteId, int count, Config config)
        {
            string                     baseURL = APIHTTPRequest.GetBaseURL(config.Location);
            List<string>               urls    = new List<string>();
            int                        cCount  = 0; 
            Dictionary<string, string> queries = new Dictionary<string, string>{
                {"fromDate", Date.ToString(config.FromDate)},
                {"toDate",   Date.ToString(config.ToDate)},
                {"limit",    Var.MF_API_STD_LIMIT.ToString()},
                {"offset",   cCount.ToString()}
            };
            while (cCount < count)
            {
                string url = APIHTTPRequest.BuildURL(
                    baseURL,
                    $"websites/{websiteId}/recordings",
                    queries
                );
                urls.Add(url);
                cCount += Var.MF_API_STD_OFFSET;
                queries["offset"] = cCount.ToString();
            }
            Output.Print($"generated '{urls.Count}' url{Output.GetPlural(urls.Count)} with count '{count}'");
            return urls;
        }

        /// <summary>
        /// Get string with URL for base Mouseflow REST API entry point.
        /// </summary>
        /// <param name="location">String with Mouseflow API server location.</param>
        /// <returns>
        ///  String with URL for base Mouseflow REST API entry point.
        /// </returns>
        public static string GetBaseURL(string location)
        {
            string loweredLocation = location.ToLower();
            if (
                !(loweredLocation.Equals(Var.API_LOCATION_EU)) &&
                !(loweredLocation.Equals(Var.API_LOCATION_US)))
            {
                throw new ArgumentException("api location must be either 'us' or 'eu'");
            }
            return $"api-{location.ToLower()}.mouseflow.com";
        }

        /// <summary>
        /// Build URL with specified path and query.
        /// </summary>
        /// <param name="baseURL"> String with Mouseflow API base entry point.</param>
        /// <param name="pathname">String with desired Mouseflow API end point.</param>
        /// <param name="queries"> Dictionary with key:value pairs corresponding to key=value query parameters.</param>
        /// <returns>
        ///  String with formatted URL with and queries ready for requests.
        /// </returns>
        public static string BuildURL(string baseURL, string pathname, Dictionary<string, string> queries)
        {
            string url = $"https://{baseURL}/{pathname}";
            if (queries.Count > 0)
            {
                int i = 0;
                url += "?";
                foreach(KeyValuePair<string, string> query in queries)
                {
                    url += $"{query.Key}={query.Value}" + (i < queries.Count - 1 ? "&" : "");
                    i++;
                }
            }
            return url;
        }
    }
}
