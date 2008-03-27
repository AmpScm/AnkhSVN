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
        /// <summary>
        /// Schedules a glyph refresh of a project
        /// </summary>
        /// <param name="project"></param>
        void MarkDirty(SvnProject project);
        /// <summary>
        /// Schedules a glyph refresh of all specified projects
        /// </summary>
        /// <param name="project"></param>
        void MarkDirty(IEnumerable<SvnProject> projects);

        /// <summary>
        /// Schedules a full data reload and glyph refresh of a project
        /// </summary>
        /// <param name="project"></param>
        /// <remarks>Should only be initiated at the users request or after a known-invalid state</remarks>
        void MarkFullRefresh(SvnProject project);
        /// <summary>
        /// Schedules a full data reload and glyph refresh of all specified projects
        /// </summary>
        /// <param name="project"></param>
        /// <remarks>Should only be initiated at the users request or after a known-invalid state</remarks>
        void MarkFullRefresh(IEnumerable<SvnProject> project);
    }
}
