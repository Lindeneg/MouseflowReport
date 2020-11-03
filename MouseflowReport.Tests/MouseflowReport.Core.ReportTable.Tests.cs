using System;
using System.Collections.Generic;
using Xunit;

using MouseflowReport.Core;


namespace MouseflowReport.Tests
{
    public class MouseflowReportCoreReportTableTest
    {

        ReportTable reportTableWithMostSeenMapsTrue;
        ReportTable reportTableWithMostSeenMapsFalse;

        public MouseflowReportCoreReportTableTest()
        {
            DateTime d1 = new DateTime(2020, 5, 1);
            DateTime d2 = new DateTime(2020, 10, 1);
            Config config1 = new Config("test", "test", "test", "test", d1, d2, keepMostSeenMaps: true);
            Config config2 = new Config("test", "test", "test", "test", d1, d2, keepMostSeenMaps: false);
            Report reportWithMostSeenMapsTrue = new Report(config1, "test");
            Report reportWithMostSeenMapsFalse = new Report(config2, "test");

            this.reportTableWithMostSeenMapsTrue = new ReportTable(reportWithMostSeenMapsTrue);
            this.reportTableWithMostSeenMapsFalse = new ReportTable(reportWithMostSeenMapsFalse);
        }

        [Fact]
        public void CanParseMostSeenMapWithKeepMostSeenMapsFalse()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>{
                {"dk", 4},
                {"de", 1},
                {"uk", 5}
            };
            Dictionary<string, int> expectedResult = new Dictionary<string, int>{{"uk", 5}};
            Dictionary<string, int> actualResult = this.reportTableWithMostSeenMapsFalse.ParseMostSeen(dict);
            Assert.True(actualResult.Count == 1);
            Assert.True(actualResult["uk"] == expectedResult["uk"]);
        }

        [Fact]
        public void CanParseMostSeenMapWithKeepMostSeenMapsTrue()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>{
                {"dk", 4},
                {"de", 1},
                {"uk", 5}
            };
            Dictionary<string, int> actualResult = this.reportTableWithMostSeenMapsTrue.ParseMostSeen(dict);
            Assert.True(actualResult.Count == 3);
            Assert.True(actualResult.Equals(dict));
        }

        [Fact]
        public void CanConvertMostSeenMapToStringWithSingleEntry()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>{
                {"dk", 4}
            };
            string expectedResult = "dk=4";
            string actualResult = this.reportTableWithMostSeenMapsTrue.MostSeenMapToString(dict);
            Assert.True(expectedResult.Equals(actualResult));
        }

        [Fact]
        public void CanConvertMostSeenMapToStringWithMultipleEntries()
        {
            Dictionary<string, int> dict = new Dictionary<string, int>{
                {"dk", 4},
                {"uk", 5},
                {"de", 3}
            };
            string expectedResult = "dk=4|uk=5|de=3";
            string actualResult = this.reportTableWithMostSeenMapsTrue.MostSeenMapToString(dict);
            Assert.True(expectedResult.Equals(actualResult));
        }
        [Fact]
        public void CanCorrectlyFindTimeMeasureKey()
        {
            (string, string, ColumnDefaultValue, ColumnAction) column = Var.columnInfo[1];
            bool expectedResult = true;
            bool actualResult = ReportTable.IsTimeMeasureKey(column.Item4, column.Item2);
            Assert.Equal(expectedResult, actualResult);
        }
        [Fact]
        public void CanCorrectlyNotFindTimeMeasureKey()
        {
            (string, string, ColumnDefaultValue, ColumnAction) column = Var.columnInfo[0];
            bool expectedResult = false;
            bool actualResult = ReportTable.IsTimeMeasureKey(column.Item4, column.Item2);
            Assert.Equal(expectedResult, actualResult);
        }
    }
}
