using System;
using System.Collections.Generic;
using System.Text;

namespace Fines.IssueZillaLib
{
    public interface IIssueSink
    {
        void StoreIssues( issuezilla issues );
    }
}
