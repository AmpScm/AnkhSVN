using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Ankh.Scc
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>The default implementation of this service is thread safe</remarks>
    public interface IFileStatusMonitor
    {
        void ScheduleSvnStatus(string path);
        void ScheduleSvnStatus(IEnumerable<string> path);

        void ScheduleGlyphUpdate(string path);
        void ScheduleGlyphUpdate(IEnumerable<string> path);
    }
}
