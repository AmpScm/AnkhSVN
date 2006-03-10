using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ErrorReportExtractor
{
    public class ErrorReport : IErrorReport
    {
        public ErrorReport(string id, string subject, string body, string senderName, DateTime receivedTime)
        {
            this.id = id;
            this.subject = subject;
            this.body = body;
            this.senderName = senderName;
            this.receivedTime = receivedTime;

            this.ParseBody();
        }

        
        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }
	
        public string Body
        {
            get { return body; }
            set { body = value; }
        }
	
        public string SenderName
        {
            get { return senderName; }
            set { senderName = value; }
        }
	
        public DateTime ReceivedTime
        {
            get { return receivedTime; }
            set { receivedTime = value; }
        }
	
        public string ID
        {
            get { return id; }
            set { id = value; }
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

        public bool RepliedTo
        {
            get { return repliedTo; }
            set { repliedTo = value; }
        }

        private void ParseBody()
        {
            this.ParseVersion();
            this.ParseExceptionType();
            this.ParseDteVersion();
            this.stackTrace = new StackTrace(this.body);
        }

        private void ParseDteVersion()
        {
            Match match;
            if ((match = DteVersionRegex.Match(this.body)) != Match.Empty)
            {
                this.dteVersion = match.Groups["DteVersion"].Value;
            }
            else if ((match = OldDteVersionRegex.Match(this.body)) != Match.Empty)
            {
                this.dteVersion = match.Groups["DteVersion"].Value;
            }
        }

        private void ParseExceptionType()
        {
            Match match;
            if ((match = ExceptionTypeRegex.Match(this.body)) != Match.Empty)
            {
                this.exceptionType = match.Groups["ExceptionType"].Value;
            }
            
        }

        private void ParseVersion()
        {
            Match match = VersionRegex.Match(this.body);
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

        private static readonly Regex ExceptionTypeRegex = new Regex(@"(?<ExceptionType>(\w+\.)*(\w+)Exception):",
                RegexOptions.IgnoreCase);

        private static readonly Regex DteVersionRegex = new Regex(@"dte=(?<DteVersion>(\d+\.)*(\d))",
                RegexOptions.IgnoreCase);

        private static readonly Regex OldDteVersionRegex = new Regex(@"DTE Version:\s*(?<DteVersion>(\d+\.)*(\d))",
                RegexOptions.IgnoreCase);

        private string id;
        private DateTime receivedTime;
        private string senderName;
        private string body;
        private string subject;
        private int? minorVersion;
        private int? majorVersion;
        private int? revision;
        private int? patchVersion;
        private string exceptionType;
        private string dteVersion;
        private StackTrace stackTrace;
        private bool repliedTo;

        

        #region IErrorReport Members


       

        #endregion
    }
}
