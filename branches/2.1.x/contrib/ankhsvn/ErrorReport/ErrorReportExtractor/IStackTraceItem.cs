using System;
using System.Collections.Generic;
using System.Text;

namespace ErrorReportExtractor
{
    public interface IStackTraceItem
    {
        string MethodName { get; }
        string Parameters { get; }
        string Filename { get; }
        int? LineNumber { get; }
        int SequenceNumber { get; }
    }
}
