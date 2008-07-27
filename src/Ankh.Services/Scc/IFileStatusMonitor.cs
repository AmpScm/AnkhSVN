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
        /// <summary>
        /// Marks the specified path dirty in the file status cache and calls <see cref="ScheduleGlyphUpdate"/> on the path
        /// </summary>
        /// <param name="path">The path.</param>
        void ScheduleSvnStatus(string path);
        /// <summary>
        /// Marks the specified paths dirty in the file status cache and calls <see cref="ScheduleGlyphUpdate"/> on the paths
        /// </summary>
        /// <param name="path">The path.</param>
        void ScheduleSvnStatus(IEnumerable<string> path);

        /// <summary>
        /// Schedules a glyph and pending changes update for the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        void ScheduleGlyphUpdate(string path);
        /// <summary>
        /// Schedules a glyph and pending changes update for the specified paths.
        /// </summary>
        /// <param name="path">The path.</param>
        void ScheduleGlyphUpdate(IEnumerable<string> path);

        /// <summary>
        /// Adds the specified path to the paths to monitor for pending changes
        /// </summary>
        /// <param name="path"></param>
        void ScheduleMonitor(string path);
        /// <summary>
        /// Adds the specified paths to the paths to monitor for pending changes
        /// </summary>
        /// <param name="path">The path.</param>
        void ScheduleMonitor(IEnumerable<string> paths);

        /// <summary>
        /// Called when a file is changed outside VS (E.g. via a diff tool)
        /// </summary>
        /// <param name="path"></param>
        void ExternallyChanged(string path);
    }
}
