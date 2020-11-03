using System;


namespace MouseflowReport.Util
{
    /// <summary>
    /// Static class to perform DateTime operations.
    /// </summary>
    public static class Date
    {
        /// <summary>
        /// Checks if string is valid ISO 8601 format.
        /// </summary>
        /// <param name="arg">String instance to be checked.</param>
        /// <returns>
        /// Bool descriping if <paramref name="arg"/> is valid ISO 8601 format.
        /// </returns>
        public static bool IsString(string arg) 
        {
            if (!(arg.GetType() == typeof(string))) 
            {
                throw new ArgumentException("Argument must me of type string");
            }
            return Var.dateStringRegex.Matches(arg).Count > 0;
        }

        /// <summary>
        /// Checks if two DateTime instances are on the same year,month,day.
        /// </summary>
        /// <param name="thisDate">DateTime instance to check for.</param>
        /// <param name="thatDate">DateTime instance to check against.</param>
        /// <returns>
        /// Bool descriping if <paramref name="thisDate"/> and <paramref name="thatDate"/> are on the same year,month,day.
        /// </returns>
        public static bool Equals(DateTime thisDate, DateTime thatDate)
        {
            return (
                thisDate.Year  == thatDate.Year  &&
                thisDate.Month == thatDate.Month &&
                thisDate.Day   == thatDate.Day
            );
        }

        /// <summary>
        /// Checks if one DateTime instance has same or earlier year,month,day compared to another DateTime instance
        /// </summary>
        /// <param name="thisDate">DateTime instance to check for.</param>
        /// <param name="thatDate">DateTime instance to check against.</param>
        /// <returns>
        /// Bool descriping if <paramref name="thisDate"/> is on the same or before the year,month,day of <paramref name="thatDate"/>
        /// </returns>
        public static bool EqualsOrLessThan(DateTime thisDate, DateTime thatDate)
        {
            return (
                thisDate.Year  == thatDate.Year  &&
                thisDate.Month == thatDate.Month &&
                thisDate.Day   <= thatDate.Day
            );
        }

        /// <summary>
        /// Checks if one DateTime instance is found within the range of two other DateTime instances.
        /// </summary>
        /// <param name="target">DateTime instance to be found within the range.</param>
        /// <param name="from">  DateTime instance to specify the start of the range.</param>
        /// <param name="to">>   DateTime instance to specify the end of the range.</param>
        /// <returns>
        /// Bool descriping if <paramref name="target"/> is found within the range of <paramref name="thisDate"/> and <paramref name="thatDate"/>
        /// </returns>
        public static bool IsWithinRange(DateTime target, DateTime from, DateTime to)
        {
            int fromCompare = target.CompareTo(from);
            int toCompare   = target.CompareTo(to);
            return (
                (fromCompare >= Var.DATE_COMPARE_LATER   || fromCompare == Var.DATE_COMPARE_EQUAL) &&
                (toCompare   <= Var.DATE_COMPARE_EARLIER || toCompare   == Var.DATE_COMPARE_EQUAL)
            );
        }

        /// <summary>
        /// Converts a ISO 8601 formatted string to a DateTime instance.
        /// </summary>
        /// <param name="arg">String with ISO 8601 date.</param>
        /// <returns>
        /// DateTime instance converted from the ISO 8601 string <paramref name="arg"/>.
        /// </returns>
        public static DateTime FromString(string arg)
        {
            int y, m, d;
            
            if (Date.IsString(arg))
            {
                string[] stringArr = arg.Split("T")[0].Split("-");
                int.TryParse(stringArr[0], out y);
                int.TryParse(stringArr[1], out m);
                int.TryParse(stringArr[2], out d);
            
            } else 
            {
                y = 1; m = 1; d = 1;
            }
            return new DateTime(y, m, d);
        }

        /// <summary>
        /// Converts a DateTime instance to ISO 8601 formatted string.
        /// </summary>
        /// <param name="arg">DateTime instance.</param>
        /// <returns>
        /// String in ISO 8601 format converted from DateTime instance <paramref name="arg"/>.
        /// </returns>
        public static string ToString(DateTime arg)
        {
            return $"{arg.Year}-{arg.Month}-{arg.Day}";
        }

        /// <summary>
        /// Get a DateTime instance from another DateTime instance and an offset in days.
        /// </summary>
        /// <param name="origin">             DateTime instance describing the origin.</param>
        /// <param name="offsetInDays">       Positive integer describing the offset in days.</param>
        /// <param name="dateOffsetOperation">DateOffsetOperation describing the arithmetic operation.</param>
        /// <returns>
        /// Datetime instance generated from <paramref name="origin"/> plus|minus <paramref name="offsetInDays"/>. 
        /// </returns>
        public static DateTime GetOffsetDate(DateTime origin, int offsetInDays, DateOffsetOperation dateOffsetOperation)
        {
            DateTime result;
            TimeSpan timeSpan = new TimeSpan(Var.HOURS_IN_DAY * offsetInDays, 0, 0);
            if (dateOffsetOperation == DateOffsetOperation.Add)
            {
                result = origin + timeSpan;
            } else if (dateOffsetOperation == DateOffsetOperation.Subtract)
            {
                result = origin - timeSpan;
            } else {
                result = origin;
            }
            return result;
        }

        /// <summary>
        /// Converts a double millisecond time measure to minutes.
        /// </summary>
        /// <param name="ms">Double ms time measure to be converted.</param>
        /// <returns>
        /// Double time measure in minutes converted from <paramref name="ms"/> in milliseconds.
        /// </returns>
        public static double MillisecondToMinutes(double ms)
        {
            return Math.Round(ms / 1000 / 60, 3);
        }
    }
}
