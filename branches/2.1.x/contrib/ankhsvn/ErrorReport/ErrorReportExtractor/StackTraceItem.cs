using System;
using System.Collections.Generic;
using System.Text;

namespace ErrorReportExtractor
{
    class StackTraceItem : IStackTraceItem
    {
        public StackTraceItem(string methodName, string parameters, string filename, int? lineNumber, int sequenceNumber)
        {
            this.methodName = methodName;
            this.parameters = parameters;
            this.filename = filename;
            this.lineNumber = lineNumber;
            this.sequenceNumber = sequenceNumber;

        }
        #region IStackTraceItem Members

        public string MethodName
        {
            get { return this.methodName; }
        }

        public string Parameters
        {
            get { return this.parameters; }
        }

        public string Filename
        {
            get { return this.filename; }
        }

        public int? LineNumber
        {
            get { return this.lineNumber; }
        }

        public int SequenceNumber
        {
            get { return this.sequenceNumber; }
        }

        #endregion

        private int? lineNumber;
        private string methodName;
        private string parameters;
        private string filename;
        private int sequenceNumber;
        
    }
}
