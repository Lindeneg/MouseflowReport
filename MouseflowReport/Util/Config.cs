using System;


namespace MouseflowReport.Core
{
    public class Config
    {
        private string   _user;
        private string   _key;
        private string   _location;
        private string   _path;
        private DateTime _fromDate;
        private DateTime _toDate;
        private bool     _includeTotalRow;
        private bool     _removeEmptyRows;
        private bool     _convertMsToMin;
        private bool     _keepMostSeenMaps;

        /// <summary>
        /// Class that contains configuration for Report format.
        /// </summary>
        /// <param name="user">             String with Mouseflow API username.</param>
        /// <param name="key">              String with Mouseflow API key.</param>
        /// <param name="location">         String with Mouseflow API server location.</param>
        /// <param name="path">             String with path to a directory where reports are dumped to.</param>
        /// <param name="fromDate">         DateTime instance specifying start of Report.</param>
        /// <param name="toDate">           DateTime instance specifying end of Report.</param>
        /// <param name="includeTotalRow">  Bool specifying if a total row should be included.</param>
        /// <param name="removeEmptyRows">  Bool specifying if empty rows should be neglected.</param>
        /// <param name="convertMsToMin">   Bool specifying if time measures should be minutes.</param>
        /// <param name="keepMostSeenMaps"> Bool specifying if most seen maps should be kept intact.</param>
        /// <returns></returns>
        public Config(
            string    user, 
            string    key, 
            string    location, 
            string    path,
            DateTime fromDate, 
            DateTime toDate,
            bool     includeTotalRow  = false, 
            bool     removeEmptyRows  = false,
            bool     convertMsToMin   = false,
            bool     keepMostSeenMaps = false
        )
        {
            this._user                = user;
            this._key                 = key;
            this._location            = location;
            this._path                = path;
            this._fromDate            = fromDate;
            this._toDate              = toDate;
            this._includeTotalRow     = includeTotalRow;
            this._removeEmptyRows     = removeEmptyRows;
            this._convertMsToMin      = convertMsToMin;
            this._keepMostSeenMaps    = keepMostSeenMaps;

            this._ValidateConfig();

        }
        public string User             { get { return this._user; }}
        public string Key              { get { return this._key; }}
        public string Location         { get { return this._location; }}
        public string Path             { get { return this._path; }}
        public DateTime FromDate       { get { return this._fromDate; }}
        public DateTime ToDate         { get { return this._toDate; }}
        public bool IncludeTotalRow    { get { return this._includeTotalRow; }}
        public bool RemoveEmptyRows    { get { return this._removeEmptyRows; }}
        public bool ConvertMsToMin     { get { return this._convertMsToMin; }}
        public bool KeepMostSeenMaps   { get { return this._keepMostSeenMaps; }}

        /// <summary>
        /// Validates config constructor arguments. Exits with exitCode '1' on invalid entries.
        /// </summary>
        /// <returns></returns>
        private void _ValidateConfig()
        {
            if (!(Math.Abs(this.FromDate.Subtract(this.ToDate).Days) >= Var.ROW_INCREMENT)) 
            {
                Console.WriteLine("error: difference between from and to date is less than the chosen row increment");
                System.Environment.Exit(1);
            }

            if (this.User.Equals(String.Empty)) {
                Console.WriteLine("error: username is not specified, use 'mfrcl run --help'");
                System.Environment.Exit(1);
            }
            if (this.Path.Equals(String.Empty)) {
                Console.WriteLine("error: path is not specified, use 'mfrcl run --help'");
                System.Environment.Exit(1);
            }
            if (this.Location.Equals(String.Empty)) {
                Console.WriteLine("error: location is not specified, use 'mfrcl run --help'");
                System.Environment.Exit(1);
            }
            if (this.Key.Equals(String.Empty)) {
                Console.WriteLine("error: key is not specified, use 'mfrcl run --help'");
                System.Environment.Exit(1);
            }
        }
    }
}
