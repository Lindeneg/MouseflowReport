using System.Threading.Tasks;

using MouseflowReport.Util;
using MouseflowReport.Core;


namespace MouseflowReport
{
    /// <summary>
    /// Generate CSV report(s) for Mouseflow connected Website(s) with API access.
    /// </summary>
    class MouseflowReport
    {
        static int Main(string[] args)
        {
            (Config config, string[] websiteIds) = Entry.ParseArgs(args);

            Output.Print("initialzing mouseflow report");
            MouseflowReport.Init(config, websiteIds).Wait();
            Output.Print("all reports finished");

            return 0;
        }

        /// <summary>
        /// Initiates Report generating and dumps finished Reports to specified path.
        /// </summary>
        /// <param name="config">    Config instance to be used in Reports.</param>
        /// <param name="websiteIds">Array of strings with websiteIds.</param>
        /// <returns>
        /// Task with Bool descriping completion status.
        /// </returns>
        async static Task<bool> Init(Config config, string[] websiteIds)
        {
            Task<Report>[] reports = new Task<Report>[websiteIds.Length];
            for (int i = 0; i < websiteIds.Length; i++)
            {
                reports.SetValue(new Report(config, websiteIds[i]).Init(), i);
            }
            foreach (Task<Report> currentReport in reports)
            {
                Report report = await currentReport;
                report.SaveReportToDisk(config.Path);
            }
            return true;
        }
    }
}
