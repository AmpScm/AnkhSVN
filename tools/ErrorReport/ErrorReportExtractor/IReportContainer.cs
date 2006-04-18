using System;
using System.Collections.Generic;

namespace ErrorReportExtractor
{
    public interface IReportContainer : IService
    {
        IEnumerable<IErrorReport> GetItems( string folder, DateTime? itemsAfter );
        IEnumerable<IMailItem> GetPotentialReplies(string folder);
    }
}
