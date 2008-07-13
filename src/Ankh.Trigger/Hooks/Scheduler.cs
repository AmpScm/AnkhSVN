using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.Ids;

namespace Ankh.Trigger.Hooks
{
    sealed class Scheduler : IAnkhScheduler
    {
        readonly Timer _timer;
        readonly IVsUIShell _shell;
        readonly SortedList<DateTime, KeyValuePair<Delegate, object[]>> _actions = new SortedList<DateTime, KeyValuePair<Delegate, object[]>>();
        Guid _grp = AnkhId.CommandSetGuid;

        public Scheduler(IVsUIShell shell)
        {
            if (shell == null)
                throw new ArgumentNullException("shell");

            _shell = shell;
            _timer = new Timer();
            _timer.Enabled = false;
            _timer.Elapsed += new ElapsedEventHandler(OnTimerElapsed);
        }

        void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {            
            try
            {
                _timer.Enabled = false;
                DateTime now = DateTime.Now;
                while (true)
                {
                    KeyValuePair<Delegate, object[]> vals;
                    lock (_actions)
                    {
                        if (_actions.Count == 0)
                            break;
                        DateTime d = _actions.Keys[0];

                        if (d > now)
                            break;

                        vals = _actions.Values[0];
                        _actions.RemoveAt(0);
                    }

                    vals.Key.DynamicInvoke(vals.Value);
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

        public void ScheduleAt(DateTime time, Ankh.Ids.AnkhCommand command)
        {
            ScheduleAt(time, CreateHandler(command));
        }

        public void ScheduleAt(DateTime time, Delegate dlg, params object[] args)
        {
            if(dlg == null)
                throw new ArgumentNullException("dlg");

            lock (_actions)
            {
                while (_actions.ContainsKey(time))
                    time = time.Add(TimeSpan.FromMilliseconds(1));

                _actions.Add(time, new KeyValuePair<Delegate,object[]>(dlg, args));

                Reschedule();
            }
        }

        delegate void DoSomething();

        DoSomething CreateHandler(AnkhCommand command)
        {
            return delegate
            {
                object n = null;
                _shell.PostExecCommand(ref _grp, (uint)command, 0, ref n);
            };
        }

        public void Schedule(TimeSpan timeSpan, Ankh.Ids.AnkhCommand command)
        {
            ScheduleAt(DateTime.Now + timeSpan, CreateHandler(command));
        }

        public void Schedule(TimeSpan timeSpan, Delegate dlg, params object[] args)
        {
            ScheduleAt(DateTime.Now + timeSpan, dlg, args);
        }

        #endregion
    }
}
