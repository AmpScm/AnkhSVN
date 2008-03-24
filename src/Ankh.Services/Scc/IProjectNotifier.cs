using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Selection;

namespace Ankh.Scc
{
    public interface IProjectNotifier
    {
        void MarkDirty(SvnProject project);
        void MarkDirty(IEnumerable<SvnProject> project);

        void MarkFullRefresh(SvnProject project);
        void MarkFullRefresh(IEnumerable<SvnProject> project);
    }
}
