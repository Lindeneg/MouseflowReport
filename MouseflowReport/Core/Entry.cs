using System;

using MouseflowReport.Util;


namespace MouseflowReport.Core
{

    /// <summary>
    /// Static class to serve as CLI argument parser and Config setter.
    /// </summary>
    public static class Entry
    {
        /// <summary>
        /// Parse array of strings and try to generate Config instance from it.
        /// </summary>
        /// <param name="args">Array of strings with CLI arguments.</param>
        /// <returns>
        /// Tuple with Config instance and array of strings with websiteIds.
        /// </returns>
        public static (Config, string[]) ParseArgs(string[] args)
        {
            string   user             = ""; 
            string   key              = ""; 
            string   location         = "eu"; 
            string   path             = "."; 
            string[] websiteIds       = {};
            DateTime from             = Date.GetOffsetDate(DateTime.Now, 30, DateOffsetOperation.Subtract); 
            DateTime to               = DateTime.Now;
            bool     includeTotalRow  = false; 
            bool     removeEmptyRows  = false; 
            bool     convertMsToMin   = false; 
            bool     keepMostSeenMaps = false;

            int i = 0;
            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "--help":
                        Console.WriteLine(@"
$ ./mfrcl run -u USER -k KEY -l LOCATION -p PROJECTS -f FROMDATE -t TODATE -o OUTPUT [ ...FLAGS ]

Required:
    -u USER       -> mouseflow user email
    -k KEY        -> mouseflow api key
    -l LOCATION   -> mouseflow server location
    -p PROJECTS   -> comma-seprated mouseflow website ids
    -f FROMDATE   -> ISO 8601 date string
    -t TODATE     -> ISO 8601 date string
    -o OUTPUT     -> Path to output directory

Optional:
    -T            -> generate a total row for accumulative data
    -R            -> remove all empty rows
    -C            -> convert millisecond time measures to minutes
    -K            -> keep all entries in most seen maps
    -D            -> output debug information
                        ");
                        System.Environment.Exit(0);
                        break;
                    case "-D":
                        Output.startTime = DateTime.Now;
                        Output.debug = true;
                        break;
                    
                    case "-T":
                        includeTotalRow = true;
                        break;

                    case "-R":
                        removeEmptyRows = true;
                        break;

                    case "-C":
                        convertMsToMin = true;
                        break;

                    case "-K":
                        keepMostSeenMaps = true;
                        break;

                    case "-o":
                        if (i + 1 < args.Length)
                        {
                            path = args[i + 1];
                            
                        }
                        break;

                    case "-u":
                        if (i + 1 < args.Length)
                        {
                            user = args[i + 1];
                            
                        }
                        break;

                    case "-k":
                        if (i + 1 < args.Length)
                        {
                            key = args[i + 1];
                            
                        }
                        break;

                    case "-l":
                        if (i + 1 < args.Length)
                        {
                            location = args[i + 1];
                            
                        }
                        break;

                    case "-p":
                        if (i + 1 < args.Length)
                        {
                            websiteIds = args[i + 1].Split(",");
                            
                        }
                        break;
                    case "-f":
                        if (i + 1 < args.Length)
                        {
                            from = Date.FromString(args[i + 1]);
                            
                        }
                        break;

                    case "-t":
                        if (i + 1 < args.Length)
                        {
                            to = Date.FromString(args[i + 1]);
                            
                        }
                        break;

                    default:
                        break;
                }
                i++;
            }
            return (new Config(user, key, location, path, from, to, includeTotalRow, removeEmptyRows, convertMsToMin, keepMostSeenMaps), websiteIds);
        }
    }
}
