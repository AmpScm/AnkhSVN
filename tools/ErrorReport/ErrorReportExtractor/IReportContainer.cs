using System;
using System.Collections.Generic;

namespace ErrorReportExtractor
{
    public interface IReportContainer : IService
    {
        IEnumerable<IErrorReport> GetItems( string folder, DateTime? itemsAfter, int? startID );
        IEnumerable<IMailItem> GetPotentialReplies(string folder, int? startIndex);
    }
}
