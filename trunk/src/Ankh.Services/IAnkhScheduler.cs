using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;

namespace Ankh
{
    // This class is defined in Ankh.Ids, because its implementation is in Ankh.Trigger, which has
    // no other external dependencies than Ankh.Ids
    public interface IAnkhScheduler
    {
        /// <summary>
        /// Schedules the specified command at or after the specified time
        /// </summary>
        /// <param name="time"></param>
        /// <param name="command"></param>
        void ScheduleAt(DateTime time, AnkhCommand command);
        /// <summary>
        /// Schedules the specified command at or after the specified time
        /// </summary>
        /// <param name="time"></param>
        /// <param name="dlg"></param>
        /// <param name="args"></param>
        void ScheduleAt(DateTime time, Delegate dlg, params object[] args);
        /// <summary>
        /// Schedules the specified command at or after the specified interval
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="command"></param>
        void Schedule(TimeSpan timeSpan, AnkhCommand command);
        /// <summary>
        /// Schedules the specified command at or after the specified interval
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="command"></param>
        void Schedule(TimeSpan timeSpan, Delegate dlg, params object[] args);
    }
}
