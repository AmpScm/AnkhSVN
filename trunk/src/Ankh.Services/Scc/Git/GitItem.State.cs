using System;
using System.Collections.Generic;

namespace Ankh
{
    public interface IGitItemStateUpdate
    {
        IList<GitItem> GetUpdateQueueAndClearScheduled();

        void SetDocumentDirty(bool value);
        void SetSolutionContained(bool inSolution, bool sccExcluded);
    }

    partial class GitItem : IGitItemStateUpdate
    {
        IList<GitItem> IGitItemStateUpdate.GetUpdateQueueAndClearScheduled()
        {
            throw new NotImplementedException();
        }

        void IGitItemStateUpdate.SetDocumentDirty(bool value)
        {
            throw new NotImplementedException();
        }

        void IGitItemStateUpdate.SetSolutionContained(bool inSolution, bool sccExcluded)
        {
            throw new NotImplementedException();
        }
    }
}
