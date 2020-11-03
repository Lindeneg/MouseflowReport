using System;
using Xunit;
using MouseflowReport.Util;

namespace MouseflowReport.Tests
{
    public class MouseflowReportUtilDateTest
    {
        [Fact]
        public void CanDetectValidDateStringWithZeros()
        {
            string testCase = "2020-05-01";
            bool result = Date.IsString(testCase);
            Assert.True(result);
        }

        [Fact]
        public void CanDetectValidDateStringWithoutZeros()
        {
            string testCase = "2020-5-1";
            bool result = Date.IsString(testCase);
            Assert.True(result);
        }

        [Fact]
        public void CanDetectInvalidDateString()
        {
            string testCase = "i-am-not-a-date-string";
            bool result = Date.IsString(testCase);
            Assert.False(result);
        }

        [Fact]
        public void CanCompareEqualDatesCorrectlyWithValidDates()
        {
            DateTime d1 = new DateTime(2020, 5, 19);
            DateTime d2 = new DateTime(2020, 5, 19);
            bool result = Date.Equals(d1, d2);
            Assert.True(result);
        }

        [Fact]
        public void CanCompareEqualDatesCorrectlyWithInvalidDates()
        {
            DateTime d1 = new DateTime(2020, 5, 19);
            DateTime d2 = new DateTime(2020, 4, 19);
            bool result = Date.Equals(d1, d2);
            Assert.False(result);
        }

        [Fact]
        public void CanCompareEqualOrLessThanDatesCorrectlyWithValidDates()
        {
            DateTime d1 = new DateTime(2020, 5, 15);
            DateTime d2 = new DateTime(2020, 5, 19);
            bool result = Date.EqualsOrLessThan(d1, d2);
            Assert.True(result);
        }

        [Fact]
        public void CanCompareEqualOrLessThanDatesCorrectlyWithInvalidDates()
        {
            DateTime d1 = new DateTime(2020, 5, 19);
            DateTime d2 = new DateTime(2020, 5, 15);
            bool result = Date.EqualsOrLessThan(d1, d2);
            Assert.False(result);
        }

        [Fact]
        public void CanDetectDateWithinDateRange()
        {
            DateTime target = new DateTime(2020, 5, 7);
            DateTime d1 = new DateTime(2020, 5, 1);
            DateTime d2 = new DateTime(2020, 5, 14);
            bool result = Date.IsWithinRange(target, d1, d2);
            Assert.True(result);
        }

        [Fact]
        public void CanDetectDateNotWithinDateRange()
        {
            DateTime target = new DateTime(2020, 5, 15);
            DateTime d1 = new DateTime(2020, 5, 1);
            DateTime d2 = new DateTime(2020, 5, 14);
            bool result = Date.IsWithinRange(target, d1, d2);
            Assert.False(result);
        }

        [Fact]
        public void CanConvertStringToDateWithValidString()
        {
            DateTime expectedReturnDate = new DateTime(2020, 5, 19);
            DateTime actualReturnDate = Date.FromString("2020-5-19");
            bool result = Date.Equals(expectedReturnDate, actualReturnDate);
            Assert.True(result);
        }

        [Fact]
        public void CanConvertStringToDateWithInvalidString()
        {
            DateTime expectedReturnDate = new DateTime(1, 1, 1);
            DateTime actualReturnDate = Date.FromString("not-a-date");
            bool result = Date.Equals(expectedReturnDate, actualReturnDate);
            Assert.True(result);
        }

        [Fact]
        public void CanConvertDateToStringWithValidDate()
        {
            string expectedReturnString = "2020-5-19"; 
            string actualReturnString = Date.ToString(new DateTime(2020, 5, 19));
            bool result = expectedReturnString.Equals(actualReturnString);
            Assert.True(result);
        }

        [Fact]
        public void CanOffsetDateFromOriginWithAddOperator()
        {
            DateTime origin = new DateTime(2020, 5, 19);
            DateTime expectedReturnDate = new DateTime(2020, 5, 26);
            DateTime actualReturnDate = Date.GetOffsetDate(origin, 7, DateOffsetOperation.Add);
            bool result = Date.Equals(expectedReturnDate, actualReturnDate);
            Assert.True(result);
        }

        [Fact]
        public void CanOffsetDateFromOriginWithSubtractOperator()
        {
            DateTime origin = new DateTime(2020, 5, 19);
            DateTime expectedReturnDate = new DateTime(2020, 5, 12);
            DateTime actualReturnDate = Date.GetOffsetDate(origin, 7, DateOffsetOperation.Subtract);
            bool result = Date.Equals(expectedReturnDate, actualReturnDate);
            Assert.True(result);
        }
    }
}
