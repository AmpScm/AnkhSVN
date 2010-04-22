using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ErrorReportExtractor
{
    public class ErrorReport : MailItem, IErrorReport
    {
        public ErrorReport(int errorReportID, int mailItemID, string subject, string body, string senderEmail, string senderName,  DateTime receivedTime)
            : base(errorReportID, mailItemID, subject, body, senderEmail, senderName, receivedTime)
        {
            this.errorReportID = errorReportID;
            this.ParseBody();
        }

        public ErrorReport()
        {

        }

        public int? MajorVersion
        {
            get { return this.majorVersion; }
        }

        public int? MinorVersion
        {
            get { return this.minorVersion; }
        }

        public int? PatchVersion
        {
            get { return this.patchVersion; }
        }

        public int? Revision
        {
            get { return this.revision; }
        }

        public string ExceptionType
        {
            get { return this.exceptionType; }
        }


        public string DteVersion
        {
            get { return this.dteVersion;  }
        }

        public IStackTrace StackTrace
        {
            get { return this.stackTrace; }
        }

        public string ExceptionMessage
        {
            get { return this.exceptionMessage; }
            set { this.exceptionMessage = value; }
        }

        public bool HasReplies
        {
            get { return hasReplies; }
            set { hasReplies = value; }
        }

        protected override void OnBodyChanged()
        {
            this.ParseBody();
        }

        private void ParseBody()
        {
            this.ParseVersion();
            this.ParseException();
            this.ParseDteVersion();
            this.stackTrace = new StackTrace(this.Body);
        }

        private void ParseDteVersion()
        {
            Match match;
            if ((match = DteVersionRegex.Match(this.Body)) != Match.Empty)
            {
                this.dteVersion = match.Groups["DteVersion"].Value;
            }
            else if ((match = OldDteVersionRegex.Match(this.Body)) != Match.Empty)
            {
                this.dteVersion = match.Groups["DteVersion"].Value;
            }
        }

        private void ParseException()
        {
            Match match;
            if ((match = ExceptionRegex.Match(this.Body)) != Match.Empty)
            {
                this.exceptionType = match.Groups["ExceptionType"].Value;
                this.exceptionMessage = match.Groups[ "ExceptionMessage" ].Value.Trim();
            }
            
        }

        private void ParseVersion()
        {
            Match match = VersionRegex.Match(this.Body);
            if (match != Match.Empty)
            {
                this.majorVersion = int.Parse(match.Groups["Major"].Value);
                this.minorVersion = int.Parse(match.Groups["Minor"].Value);
                this.patchVersion = int.Parse(match.Groups["Patch"].Value);
                this.revision = int.Parse(match.Groups["Revision"].Value);
            }
            
        }

        private static readonly Regex VersionRegex = new Regex(
            @"Version:\s+(?<Major>\d{1,2})\.(?<Minor>\d{1,2})\.(?<Patch>\d{1,4})\.(?<Revision>\d{3,10})", 
            RegexOptions.IgnoreCase);

        private static readonly Regex ExceptionRegex = 
            new Regex( @"(?<ExceptionType>(\w+\.)*(\w+)Exception):\s+(?<ExceptionMessage>[^\n]+)\n",
                RegexOptions.ExplicitCapture |
                RegexOptions.Singleline |
                RegexOptions.IgnorePatternWhitespace );

        private static readonly Regex DteVersionRegex = new Regex(@"dte=(?<DteVersion>(\d+\.)*(\d))",
                RegexOptions.IgnoreCase);

        private static readonly Regex OldDteVersionRegex = new Regex(@"DTE Version:\s*(?<DteVersion>(\d+\.)*(\d))",
                RegexOptions.IgnoreCase);

        private int? minorVersion;
        private int? majorVersion;
        private int? revision;
        private int? patchVersion;
        private string exceptionType;
        private string exceptionMessage;
        private string dteVersion;
        private StackTrace stackTrace;
        private bool hasReplies;
        private int errorReportID;

       

        #region IErrorReport Members


       

        #endregion
    }
}
