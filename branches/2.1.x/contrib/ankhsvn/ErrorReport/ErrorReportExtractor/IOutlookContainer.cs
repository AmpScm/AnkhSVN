using System;
using System.Collections.Generic;
using System.Text;

namespace ErrorReportExtractor
{
    public interface IOutlookContainer : IReportContainer
    {
        IEnumerable<IErrorReport> GetItems( string folder, int? startIndex, out int lastIndexRetrieved );
        IEnumerable<IMailItem> GetPotentialReplies( string folder, int? startIndex, out int lastIndexRetrieved );
    }
}
