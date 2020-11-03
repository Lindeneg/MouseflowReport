using System;
using Xunit;
using MouseflowReport.Core;
using MouseflowReport.Util;


namespace MouseflowReport.Tests
{
    public class MouseflowReportCoreReportTest
    {
        DateTime fromDate;
        DateTime toDate;
        int      knownWeekCount;
        string   websiteId;
        Config   config;
        Report   report;
        public MouseflowReportCoreReportTest()
        {
            this.fromDate       = new DateTime(2020, 5, 1);
            this.toDate         = new DateTime(2020, 10, 1);
            this.knownWeekCount = 22;
            this.websiteId      = "no-id";
            this.config         = new Config("test", "test", "test", "test", this.fromDate, this.toDate);
            this.report         = new Report(this.config, this.websiteId);
        }

        [Fact]
        public void CanSetFromDateCorrectly()
        {
            Assert.True(Date.Equals(this.fromDate, this.report.Rows[0]["from"]));
        }

        [Fact]
        public void CanSetToDateCorrectly()
        {
            Assert.True(Date.Equals(this.toDate, this.report.Rows[this.report.Rows.Count - 1]["to"]));
        }

        [Fact]
        public void CanGenerateCorrectRowCount()
        {
            Assert.Equal(this.knownWeekCount, this.report.Rows.Count);
        }

        [Fact]
        public void CanSetCorrectWebsiteId()
        {
            Assert.True(this.report.WebsiteId.Equals(this.websiteId));
        }
    }
}
