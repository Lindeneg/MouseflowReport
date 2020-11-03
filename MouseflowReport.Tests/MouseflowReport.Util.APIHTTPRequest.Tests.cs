using System;
using System.Collections.Generic;
using Xunit;
using MouseflowReport.Core;
using MouseflowReport.Util;


namespace MouseflowReport.Tests
{
    public class MouseflowReportUtilAPIHTTPRequestTest
    {
        [Fact]
        public void CanGetBaseURLEUWithCaps()
        {
            string expectedResult = "api-eu.mouseflow.com";
            string actualResult = APIHTTPRequest.GetBaseURL("EU");
            Assert.True(expectedResult.Equals(actualResult));
        }
        [Fact]
        public void CanGetBaseURLUSWithCaps()
        {
            string expectedResult = "api-us.mouseflow.com";
            string actualResult = APIHTTPRequest.GetBaseURL("US");
            Assert.True(expectedResult.Equals(actualResult));
        }
        [Fact]
        public void CanGetBaseURLEUWithoutCaps()
        {
            string expectedResult = "api-eu.mouseflow.com";
            string actualResult = APIHTTPRequest.GetBaseURL("eu");
            Assert.True(expectedResult.Equals(actualResult));
        }
        [Fact]
        public void CanGetBaseURLUSWithoutCaps()
        {
            string expectedResult = "api-us.mouseflow.com";
            string actualResult = APIHTTPRequest.GetBaseURL("us");
            Assert.True(expectedResult.Equals(actualResult));
        }
        [Fact]
        public void InvalidLocationThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>  APIHTTPRequest.GetBaseURL("dk"));
        }
        [Fact]
        public void CanCorrectlyBuildURLWithoutParams()
        {
            string expectedResult = "https://api-us.mouseflow.com/websites/id/recordings";
            string actualResult = APIHTTPRequest.BuildURL(APIHTTPRequest.GetBaseURL("us"), "websites/id/recordings", new Dictionary<string, string>());
            Assert.True(expectedResult.Equals(actualResult));
        }
        [Fact]
        public void CanCorrectlyBuildURLWithParams()
        {
            string expectedResult = "https://api-us.mouseflow.com/websites/id/recordings?fromDate=2020-5-1&toDate=2020-10-1&offset=10000";
            string actualResult = APIHTTPRequest.BuildURL(APIHTTPRequest.GetBaseURL("us"), "websites/id/recordings", new Dictionary<string, string>{
                {"fromDate", "2020-5-1"},
                {"toDate", "2020-10-1"},
                {"offset", "10000"}
            });
            Assert.True(expectedResult.Equals(actualResult));
        }
        [Fact]
        public void CanCorrectlyBuildFormattedQueryList()
        {
            Config config = new Config("n", "n", "us", "n", new DateTime(2020, 5, 1), new DateTime(2020, 10, 1));
            string expectedResult1 = "https://api-us.mouseflow.com/websites/websiteId/recordings?fromDate=2020-5-1&toDate=2020-10-1&limit=10000&offset=0";
            string expectedResult2 = "https://api-us.mouseflow.com/websites/websiteId/recordings?fromDate=2020-5-1&toDate=2020-10-1&limit=10000&offset=10000";
            List<string> actualResult = APIHTTPRequest.GetURLFormattedQueryList("websiteId", 11000, config);
            Assert.True(actualResult.Count == 2);
            Assert.True(expectedResult1.Equals(actualResult[0]));
            Assert.True(expectedResult2.Equals(actualResult[1]));
        }
    }
}
