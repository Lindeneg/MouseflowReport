using System;
using Xunit;

using MouseflowReport.Core;


namespace MouseflowReport.Tests
{
    public class MouseflowReportCoreEntryTest
    {
        string user;
        string key;
        string location;
        string path;
        string[] websiteIds;
        string from;
        string to;
        bool includeTotalRow;
        bool removeEmptyRows;
        bool convertMsToMin;
        bool keepMostSeenMaps;

        string[] args;

        Config actualConfig;
        string[] actualWebsiteIds;

        public MouseflowReportCoreEntryTest()
        {
            this.user = "test-user";
            this.key = "test-key";
            this.location = "test-location";
            this.path = "test-path";
            this.websiteIds = new string[]{"some-id", "some-other-id"};
            this.from = "2020-5-1";
            this.to = "2020-10-1";
            this.includeTotalRow = true;
            this.removeEmptyRows = false;
            this.convertMsToMin = false;
            this.keepMostSeenMaps = true;

            this.args = new string[]{
                "-u",
                this.user,
                "-k",
                this.key,
                "-l",
                this.location,
                "-p",
                "some-id,some-other-id",
                "-f",
                this.from,
                "-t",
                this.to,
                "-o",
                this.path,
                "-T",
                "-K"
            };

            (this.actualConfig, this.actualWebsiteIds) = Entry.ParseArgs(this.args);
        }

        [Fact]
        public void CanCorrectlySetUser()
        {
            Assert.True(this.actualConfig.User.Equals(this.user));
        }
        [Fact]
        public void CanCorrectlySetKey()
        {
            Assert.True(this.actualConfig.Key.Equals(this.key));
        }
        [Fact]
        public void CanCorrectlySetLocation()
        {
            Assert.True(this.actualConfig.Location.Equals(this.location));
        }
        [Fact]
        public void CanCorrectlySetPath()
        {
            Assert.True(this.actualConfig.Path.Equals(this.path));
        }
        [Fact]
        public void CanCorrectlySetWebsiteIds()
        {
            Assert.True(this.actualWebsiteIds.Length == this.websiteIds.Length);
            Assert.True(this.actualWebsiteIds[0].Equals(this.websiteIds[0]));
            Assert.True(this.actualWebsiteIds[1].Equals(this.websiteIds[1]));
        }
        [Fact]
        public void CanCorrectlySetIncludeTotalRow()
        {
            Assert.Equal(this.actualConfig.IncludeTotalRow, this.includeTotalRow);
        }
        [Fact]
        public void CanCorrectlySetRemoveEmptyRows()
        {
            Assert.Equal(this.actualConfig.RemoveEmptyRows, this.removeEmptyRows);
        }
        [Fact]
        public void CanCorrectlySetConvertMsToMin()
        {
            Assert.Equal(this.actualConfig.ConvertMsToMin, this.convertMsToMin);
        }
        [Fact]
        public void CanCorrectlySetKeepMostSeenMaps()
        {
            Assert.Equal(this.actualConfig.KeepMostSeenMaps, this.keepMostSeenMaps);
        }
    }
}
