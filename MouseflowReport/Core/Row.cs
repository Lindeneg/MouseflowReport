using System;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;

using MouseflowReport.Util;


namespace MouseflowReport.Core
{
    public class Row
    {
        public Dictionary<string, dynamic> row;

        /// <summary>
        /// Class to hold Row information.
        /// </summary>
        /// <param name="fromDate">DateTime specifying start of Row.</param>
        /// <param name="toDate">  DateTime specifying end of Row.</param>
        /// <returns></returns>
        public Row(DateTime fromDate, DateTime toDate) 
        {
            this.row = new Dictionary<string, dynamic>();

            this.row.Add("from", fromDate);
            this.row.Add("to", toDate);

            foreach ((string, string, ColumnDefaultValue, ColumnAction) info in Var.columnInfo)
            {
                if (info.Item3.Equals(ColumnDefaultValue.Zero))
                {
                    this.row.Add(info.Item1, 0);
                } else if (info.Item3.Equals(ColumnDefaultValue.Dict))
                {
                    this.row.Add(info.Item1, new Dictionary<string, int>());
                } else {
                    Output.Print($"column default value not found '{info.Item1}'");
                }
            }
        }

        /// <summary>
        /// Get and Set Row entries by overloading indexers.
        /// </summary>
        /// <param name="key">String key to use as Row index.</param>
        /// <returns>Dynamic Row entry from <paramref name="key"/>.</returns>
        public dynamic this[string key]
        {
            get
            {
                return this.row[key];
            }
            set
            {
                this.row[key] = value;
            }
        }

        /// <summary>
        /// Increment each entry of Row given a recording.
        /// </summary>
        /// <param name="recording">Dictionary with recording information.</param>
        /// <param name="msToMin">Bool specifying if time measures should be minutes.</param>
        /// <returns></returns>
        public void IncrementRow(Dictionary<string, JsonElement> recording, bool msToMin)
        {

            foreach ((string, string, ColumnDefaultValue, ColumnAction) columnInfo in Var.columnInfo)
            {
                (string colKey, string altKey, ColumnDefaultValue defaultVal, ColumnAction action) = columnInfo;
                switch(action)
                {
                    case ColumnAction.IncrementOne:
                        this[colKey]++;
                        break;
                    case ColumnAction.IncrementSum:
                        double value = recording[altKey].GetDouble();
                        if (msToMin && ReportTable.IsTimeMeasureKey(action, altKey))
                        {
                            value = Date.MillisecondToMinutes(value);
                        }
                        this[colKey] = Math.Round(this[colKey] + value, 3);
                        break;
                    case ColumnAction.IncrementDict:
                        this._IncrementDictionary(colKey, recording[altKey].GetString());
                        break;
                    case ColumnAction.HandleIterable:
                        List<JsonElement> items = recording[altKey].EnumerateArray().ToList();
                        foreach (JsonElement item in items)
                        {
                            this._IncrementDictionary(colKey, item.GetString());
                        }
                        break;
                    case ColumnAction.PerformAverage:
                        this[colKey] = Math.Round(this[altKey] / this["totalSessions"], 3);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Increment a Dictionary in Row given key and target.
        /// </summary>
        /// <param name="key">   String with Row key.</param>
        /// <param name="target">String with target key to increment.</param>
        /// <returns></returns>
        private void _IncrementDictionary(string key, string target)
        {
            Dictionary<string, int> entry = (Dictionary<string, int>)this[key];
            if (target.Equals(String.Empty) || target.Equals("Err")) {
                target = "None";
            }
            if (entry.ContainsKey(target))
            {
                entry[target]++;
            } else
            {
                entry.Add(target, 1);
            }
        }

        /// <summary>
        /// Create a List of Row instances given start and end DateTime and an increment in days.
        /// </summary>
        /// <param name="fromDate"> DateTime start of the Row.</param>
        /// <param name="toDate">   DateTime start of the Row.</param>
        /// <param name="increment">Int specifying increment in days.</param>
        /// <returns>List of Row instances.</returns>
        public static List<Row> CreateRowsFromIncrementalDateRange(DateTime fromDate, DateTime toDate, int increment) 
        {
            List<Row> rows        = new List<Row>();
            DateTime  currentFrom = fromDate;
            DateTime  currentTo   = Date.GetOffsetDate(fromDate, increment, DateOffsetOperation.Add);
            int       guard       = 0;
            while (!(Date.EqualsOrLessThan(toDate, currentTo)) && guard < Var.MAX_ROWS)
            {
                rows.Add(new Row(currentFrom, currentTo));
                currentFrom = Date.GetOffsetDate(currentFrom, increment + 1, DateOffsetOperation.Add);
                currentTo   = Date.GetOffsetDate(currentTo,   increment + 1, DateOffsetOperation.Add);
                guard++;
            }
            Row._AddLastEntryIfDateRemainderExists(rows, toDate);
            return rows;
        }

        /// <summary>
        /// Add Row to List of Rows if Row date range contains remainders.
        /// </summary>
        /// <param name="rows">   List of Rows.</param>
        /// <param name="toDate"> DateTime start of the List of Rows.</param>
        /// <returns></returns>
        private static void _AddLastEntryIfDateRemainderExists(List<Row> rows, DateTime toDate)
        {
            if (rows.Count > 0)
            {
                DateTime currentTo = rows[rows.Count - 1]["to"];
                if (!(Date.Equals(currentTo, toDate)))
                {
                    DateTime newCurrentFrom = Date.GetOffsetDate(currentTo, 1, DateOffsetOperation.Add);
                    Row      row            = new Row(newCurrentFrom, toDate);
                    rows.Add(row);
                }
            }
        }
    }
}
