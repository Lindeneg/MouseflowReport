using System.Text.RegularExpressions;
using System.Collections.Generic;


namespace MouseflowReport
{
    /// <summary>
    /// Static class to hold global constants.
    /// </summary>
    public static class Var
    {
        public const int    HOURS_IN_DAY          = 24;
        public const int    MAX_ROWS              = 55;
        public const int    DATE_COMPARE_EARLIER  = -1;
        public const int    DATE_COMPARE_EQUAL    = 0;
        public const int    DATE_COMPARE_LATER    = 1;
        public const int    ROW_INCREMENT         = 6;
        public const int    MF_API_STD_LIMIT      = 10000;
        public const int    MF_API_STD_OFFSET     = 10000;
        public const string API_LOCATION_EU       = "eu";
        public const string API_LOCATION_US       = "us";
        public static Regex dateStringRegex       = new Regex(
            @"^\d{4}-\d{1,2}-\d{1,2}([T]\d{2}:\d{2}:\d{2}\.\d+\+\d{2}:\d{2})?$", 
            RegexOptions.Compiled
        );
        public static List<(string, string, ColumnDefaultValue, ColumnAction)> columnInfo = new List<(string, string, ColumnDefaultValue, ColumnAction)>{
            ("totalSessions",     "",                ColumnDefaultValue.Zero, ColumnAction.IncrementOne),
            ("totalDuration",     "duration",        ColumnDefaultValue.Zero, ColumnAction.IncrementSum),
            ("averageDuration",   "totalDuration",   ColumnDefaultValue.Zero, ColumnAction.PerformAverage),
            ("totalEngagement",   "engagementTime",  ColumnDefaultValue.Zero, ColumnAction.IncrementSum),
            ("averageEngagement", "totalEngagement", ColumnDefaultValue.Zero, ColumnAction.PerformAverage),
            ("totalPageViews",    "pages",           ColumnDefaultValue.Zero, ColumnAction.IncrementSum),
            ("averagePageViews",  "totalPageViews",  ColumnDefaultValue.Zero, ColumnAction.PerformAverage),
            ("mostSeenCountry",   "country",         ColumnDefaultValue.Dict, ColumnAction.IncrementDict),
            ("mostSeenBrowser",   "browser",         ColumnDefaultValue.Dict, ColumnAction.IncrementDict),
            ("mostSeenDevice",    "device",          ColumnDefaultValue.Dict, ColumnAction.IncrementDict),
            ("mostSeenSystem",    "os",              ColumnDefaultValue.Dict, ColumnAction.IncrementDict),
            ("mostSeenReferrer",  "referrerType",    ColumnDefaultValue.Dict, ColumnAction.IncrementDict),
            ("mostSeenEntryPage", "entryPage",       ColumnDefaultValue.Dict, ColumnAction.IncrementDict),
            ("mostSeenTag",       "tags",            ColumnDefaultValue.Dict, ColumnAction.HandleIterable),
            ("mostSeenVariable",  "variables",       ColumnDefaultValue.Dict, ColumnAction.HandleIterable)
        };
    }
}

public enum DateOffsetOperation 
{
    Add,
    Subtract
};

public enum ColumnAction
{
    IncrementOne,
    IncrementSum,
    PerformAverage,
    IncrementDict,
    HandleIterable
};

public enum ColumnDefaultValue
{
    Zero,
    Dict
};
