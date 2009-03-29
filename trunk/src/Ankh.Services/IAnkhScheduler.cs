// $Id$
//
// Copyright 2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;

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
