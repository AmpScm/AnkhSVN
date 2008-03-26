using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Selection;

namespace Ankh.Scc
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>The default implementation of this service is thread safe</remarks>
    public interface IProjectNotifier
    {
        void MarkDirty(SvnProject project);
        void MarkDirty(IEnumerable<SvnProject> project);

        void MarkFullRefresh(SvnProject project);
        void MarkFullRefresh(IEnumerable<SvnProject> project);
    }
}
