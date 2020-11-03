using System;


namespace MouseflowReport.Util
{
    /// <summary>
    /// Static class to perform debug operations.
    /// </summary>
    public static class Output
    {
        public static bool     debug     = false;
        public static DateTime startTime = DateTime.Now;

        /// <summary>
        /// Print a message to stdout if the static bool 'Output.debug' is true.
        /// </summary>
        /// <param name="msg">String with message to be printed.</param>
        /// <returns></returns>
        public static void Print(string msg, string state = "debug")
        {
            if (Output.debug)
            {
                int targetPrefixLength = 15;
                int diffOffset         = 6;
                double diff = Math.Round(DateTime.Now.Subtract(startTime).TotalSeconds, 4);
                int targetDiff = targetPrefixLength - (diff.ToString().Length + diffOffset);
                string spaces = "";
                if (targetDiff > 0)
                {
                    for (int i = 0; i < targetDiff; i++)
                    {
                        spaces += " ";
                    }
                }
                Console.WriteLine($"{state.ToUpper()} {diff}s {spaces} -> {msg}");
            }
        }

        /// <summary>
        /// Returns a plural "s" if applicable in the context
        /// </summary>
        /// <param name="context">Int descriping the context.</param>
        /// <returns>String with an "s" if <paramref name="context"/> is greater than one</returns>
        public static string GetPlural(int context)
        {
            if (context > 1)
            {
                return "s";
            }
            return "";
        }
    }
}
