// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
using System.Timers;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.Commands;

namespace Ankh.Services
{
    [GlobalService(typeof(IAnkhScheduler))]
    sealed class AnkhScheduler : AnkhService, IAnkhScheduler
    {
        readonly Timer _timer;
        IAnkhCommandService _commands;
        readonly SortedList<DateTime, AnkhAction> _actions = new SortedList<DateTime, AnkhAction>();
        Guid _grp = AnkhId.CommandSetGuid;

        public AnkhScheduler(IAnkhServiceProvider context)
            : base(context)
        {
            _timer = new Timer();
            _timer.Enabled = false;
            _timer.Elapsed += new ElapsedEventHandler(OnTimerElapsed);
        }

        IAnkhCommandService Commands
        {
            get { return _commands ?? (_commands = GetService<IAnkhCommandService>()); }
        }

        void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {            
            try
            {
                _timer.Enabled = false;
                DateTime now = DateTime.Now;
                while (true)
                {
                    AnkhAction action;
                    lock (_actions)
                    {
                        if (_actions.Count == 0)
                            break;
                        DateTime d = _actions.Keys[0];

                        if (d > now)
                            break;

                        action = _actions.Values[0];
                        _actions.RemoveAt(0);
                    }

                    action();
                }
            }
            catch
            { }
            finally
            {
                Reschedule();
            }
        }

        void Reschedule()
        {
            lock (_actions)
            {
                if (_actions.Count == 0)
                    return;

                double tLeft = (_actions.Keys[0] - DateTime.Now).TotalMilliseconds;
                if (tLeft < 0.0)
                    tLeft = 10.0;

                _timer.Interval = tLeft;
                _timer.Enabled = true;
            }
        }
                   

        #region IAnkhScheduler Members

        public void ScheduleAt(DateTime time, AnkhCommand command)
        {
            ScheduleAt(time, CreateHandler(command));
        }

        public void ScheduleAt(DateTime time, AnkhAction action)
        {
            if(action == null)
                throw new ArgumentNullException("action");

            lock (_actions)
            {
                while (_actions.ContainsKey(time))
                    time = time.Add(TimeSpan.FromMilliseconds(1));

                _actions.Add(time, action);

                Reschedule();
            }
        }

        AnkhAction CreateHandler(AnkhCommand command)
        {
            return delegate
            {
                Commands.PostExecCommand(command);
            };
        }

        public void Schedule(TimeSpan timeSpan, AnkhCommand command)
        {
            ScheduleAt(DateTime.Now + timeSpan, CreateHandler(command));
        }

        public void Schedule(TimeSpan timeSpan, AnkhAction action)
        {
            ScheduleAt(DateTime.Now + timeSpan, action);
        }

        #endregion
    }
}
