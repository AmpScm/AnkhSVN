using System;
using System.Collections.Generic;

namespace ErrorReportExtractor
{
    public interface IReportContainer : IService
    {
        IEnumerable<IErrorReport> GetItems( );
        IEnumerable<IMailItem> GetPotentialReplies();
    }
}
