using System;
using System.Collections.Generic;
using System.Text;

namespace ErrorReportExtractor
{
    public interface IStackTrace : IList<IStackTraceItem>
    {
        string Text { get; }
    }
}
