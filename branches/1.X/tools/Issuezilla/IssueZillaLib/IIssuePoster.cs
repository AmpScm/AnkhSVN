using System;
using System.Collections.Generic;
using System.Text;

namespace Fines.IssueZillaLib
{
    public interface IIssuePoster
    {
        void PostNewIssue( issue issue, string comment );
        void UpdateIssue( issue issue, string comment );
    }
}
