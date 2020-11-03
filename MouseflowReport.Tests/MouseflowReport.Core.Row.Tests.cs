using System;
using Xunit;
using MouseflowReport.Core;
using MouseflowReport.Util;


namespace MouseflowReport.Tests
{
    public class MouseflowReportCoreRowTest
    {
        DateTime baseFrom;
        DateTime baseTo;
        Row      baseRow;
        public MouseflowReportCoreRowTest()
        {
            this.baseFrom = new DateTime(2020, 5, 1);
            this.baseTo = new DateTime(2020, 5, 7);
            this.baseRow = new Row(this.baseFrom, this.baseTo);
        }
        [Fact]
        public void CanInitializeFromConstructorWithCorrectCount()
        {
            Assert.Equal(this.baseRow.row.Count - 2, Var.columnInfo.Count);
        }
        [Fact]
        public void CanInitializeFromConstructorWithCorrectFromDate()
        {
            Assert.True(Date.Equals(this.baseFrom, this.baseRow.row["from"]));
        }
        [Fact]
        public void CanInitializeFromConstructorWithCorrectToDate()
        {
            Assert.True(Date.Equals(this.baseTo, this.baseRow.row["to"]));
        }
        [Fact]
        public void CanGetViaIndex()
        {
            int expectedResult = 0;
            int actualResult = (int)this.baseRow.row["totalSessions"];
            Assert.Equal(expectedResult, actualResult);
        }
        [Fact]
        public void CanSetViaIndex()
        {
            int expectedResult = 10;
            this.baseRow.row["totalSessions"] = 10;
            Assert.Equal(expectedResult, this.baseRow.row["totalSessions"]);
        }
    }
}
