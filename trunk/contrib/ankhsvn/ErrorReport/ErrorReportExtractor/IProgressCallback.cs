using System;
using System.Collections.Generic;
using System.Text;

namespace ErrorReportExtractor
{
    public interface IProgressCallback
    {
        void Verbose(string message, params object[] args);
        void Info(string message, params object[] args);
        void Warning(string message, params object[] args);
        void Error(string message, params object[] args);
        void Progress();
        bool VerboseMode { get; set; }
        void Exception(Exception ex);
    }
}
