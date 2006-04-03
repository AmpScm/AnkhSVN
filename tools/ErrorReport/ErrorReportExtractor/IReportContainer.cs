using System;
using System.Collections.Generic;

namespace ErrorReportExtractor
{
    public interface IReportContainer
    {
        IEnumerable<IErrorReport> GetAllItems( int? limit );
    }
}
