using System;
using System.Collections.Generic;

namespace ErrorReportExtractor
{
    public interface IReportContainer : IService
    {
        IEnumerable<IErrorReport> GetAllItems( string folder, int? limit );
        IEnumerable<IMailItem> GetPotentialReplies(string folder);
    }
}
