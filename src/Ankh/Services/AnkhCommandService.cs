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
using Ankh.Commands;
using Microsoft.VisualStudio.Shell.Interop;
using System.ComponentModel.Design;
using Ankh.Ids;
using Microsoft.VisualStudio.OLE.Interop;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace Ankh.Services
{
    [GlobalService(typeof(IAnkhCommandService))]
    class AnkhCommandService : AnkhService, IAnkhCommandService
    {
        readonly List<int> _delayedCommands = new List<int>();
        public AnkhCommandService(IAnkhServiceProvider context)
            : base(context)
        {
        }

        IVsUIShell _uiShell;
        protected IVsUIShell UIShell
        {
            get { return _uiShell ?? (_uiShell = GetService<IVsUIShell>(typeof(SVsUIShell))); }
        }

        IOleCommandTarget _commandDispatcher;
        protected IOleCommandTarget CommandDispatcher
        {
            [DebuggerStepThrough]
            get { return _commandDispatcher ?? (_commandDispatcher = GetService<IOleCommandTarget>(typeof(SUIHostCommandDispatcher))); }
        }

        AnkhContext _ankhContext;
        protected AnkhContext AnkhContext
        {
            [DebuggerStepThrough]
            get { return _ankhContext ?? (_ankhContext = GetService<AnkhContext>()); }
        }

        #region IAnkhCommandService Members

        public CommandResult ExecCommand(Ankh.Ids.AnkhCommand command)
        {
            // The commandhandler in the package always checks enabled; no need to do it here
            return ExecCommand(command, false);
        }

        public CommandResult ExecCommand(Ankh.Ids.AnkhCommand command, bool verifyEnabled)
        {
            // The commandhandler in the package always checks enabled; no need to do it here
            return ExecCommand(new CommandID(AnkhId.CommandSetGuid, (int)command), verifyEnabled);
        }

        public CommandResult ExecCommand(System.ComponentModel.Design.CommandID command)
        {
            return ExecCommand(command, true);
        }

        public CommandResult ExecCommand(System.ComponentModel.Design.CommandID command, bool verifyEnabled)
        {
            return ExecCommand(command, verifyEnabled, null);
        }

        public CommandResult ExecCommand(System.ComponentModel.Design.CommandID command, bool verifyEnabled, object argument)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            // TODO: Assert that we are in the UI thread

            IOleCommandTarget dispatcher = CommandDispatcher;

            if(dispatcher == null)
                return new CommandResult(false);

            Guid g = command.Guid;

            if (verifyEnabled)
            {
                OLECMD[] cmd = new OLECMD[1];
                cmd[0].cmdID = unchecked((uint)command.ID);

                if (VSConstants.S_OK != dispatcher.QueryStatus(ref g, 1, cmd, IntPtr.Zero))
                    return new CommandResult(false);

                OLECMDF flags = (OLECMDF)cmd[0].cmdf;

                if ((flags & OLECMDF.OLECMDF_SUPPORTED) == (OLECMDF)0)
                    return new CommandResult(false); // Not supported

                if ((flags & OLECMDF.OLECMDF_ENABLED) == (OLECMDF)0)
                    return new CommandResult(false); // Not enabled
            }

            IntPtr vIn = IntPtr.Zero;
            IntPtr vOut = IntPtr.Zero;
            try
            {
                vOut = Marshal.AllocCoTaskMem(128);
                NativeMethods.VariantInit(vOut);
                
                if (argument != null)
                {
                    vIn = Marshal.AllocCoTaskMem(128);
                    Marshal.GetNativeVariantForObject(argument, vIn);
                }

                bool ok = ErrorHandler.Succeeded(dispatcher.Exec(ref g,
                    unchecked((uint)command.ID), (uint)OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, IntPtr.Zero, IntPtr.Zero));

                return new CommandResult(ok, Marshal.GetObjectForNativeVariant(vOut));
            }
            finally
            {
                if (vIn != IntPtr.Zero)
                {
                    NativeMethods.VariantClear(vIn);
                    Marshal.FreeCoTaskMem(vIn);
                }
                if (vOut != IntPtr.Zero)
                {
                    NativeMethods.VariantClear(vOut);
                    Marshal.FreeCoTaskMem(vOut);
                }
            }
        }

        static class NativeMethods
        {
            [DllImport("oleaut32.dll")]
            public static extern int VariantClear(IntPtr v);

            [DllImport("oleaut32.dll")]
            public static extern int VariantInit(IntPtr v);
        }

        /// <summary>
        /// Posts the tick command.
        /// </summary>
        /// <param name="tick">if set to <c>true</c> [tick].</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public bool PostTickCommand(ref bool tick, AnkhCommand command)
        {
            if (tick)
                return false;

            tick = true;
            bool ok = false;
            try
            {
                return ok = PostExecCommand(command);
            }
            finally
            {
                if (!ok)
                    tick = false;
            }
        }

        public void SafePostTickCommand(ref bool tick, AnkhCommand command)
        {
            if(tick)
                return;

            tick = true;
            lock(_delayTasks)
            {
                _delayedCommands.Add((int)command);
            }
        }

        public bool PostExecCommand(Ankh.Ids.AnkhCommand command)
        {
            return PostExecCommand(command, null, CommandPrompt.DoDefault);
        }

        public bool PostExecCommand(Ankh.Ids.AnkhCommand command, object args)
        {
            return PostExecCommand(command, args, CommandPrompt.DoDefault);
        }

        public bool PostExecCommand(Ankh.Ids.AnkhCommand command, object args, CommandPrompt prompt)
        {
            return PostExecCommand(new CommandID(AnkhId.CommandSetGuid, (int)command), args, prompt);
        }

        public bool PostExecCommand(System.ComponentModel.Design.CommandID command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            return PostExecCommand(command, null);
        }

        public bool PostExecCommand(System.ComponentModel.Design.CommandID command, object args)
        {
            return PostExecCommand(command, args, CommandPrompt.DoDefault);
        }

        delegate void PostTask();
        bool _delayed;
        readonly List<PostTask> _delayTasks = new List<PostTask>();

        public bool PostExecCommand(CommandID command, object args, CommandPrompt prompt)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            lock(_delayTasks)
            {
                if(_delayed)
                {
                    _delayTasks.Add(
                        delegate
                        {
                            PerformPost(command, prompt, args);
                        });

                    return true;
                }
                else if(PerformPost(command, prompt, args))
                {
                    for(int i = 0; i < _delayedCommands.Count; i++)
                    {
                        if(!PerformPost(new CommandID(AnkhId.CommandSetGuid, _delayedCommands[i]), CommandPrompt.DoDefault, null))
                        {
                            _delayedCommands.RemoveRange(0, i);
                            return true;
                        }
                    }
                    _delayedCommands.Clear();
                    return true;
                }
                else
                    return false;
            }
        }

        bool PerformPost(CommandID command, CommandPrompt prompt, object args)
        {
            IVsUIShell shell = UIShell;

            if (shell != null)
            {
                uint flags;
                switch (prompt)
                {
                    case CommandPrompt.Always:
                        flags = (uint)OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER;
                        break;
                    case CommandPrompt.Never:
                        flags = (uint)OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER;
                        break;
                    default:
                        flags = (uint)OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT;
                        break;
                }

                Guid set = command.Guid;
                object a = args;

                return VSConstants.S_OK == shell.PostExecCommand(ref set,
                        unchecked((uint)command.ID), flags, ref a);
            }

            return false;
        }

        readonly List<DelayDelegateCheck> _checks = new List<DelayDelegateCheck>();
        readonly WindowsFormsSynchronizationContext _syncContext = new WindowsFormsSynchronizationContext();
        public void DelayPostCommands(DelayDelegateCheck check)
        {
            if (check == null)
                throw new ArgumentNullException("check");
            
            _checks.Add(check);
            if (!_delayed)
            {
                _delayed = true;
                _syncContext.Post(TryRelease, null);
            }
        }

        void TryRelease(object v)
        {
            if(_delayed)
                TryReleaseDelayed();

            if (_delayed)
                PostCheck();                
        }

        void PostCheck()
        {
            PostTask pt = delegate()
            {
                Thread.Sleep(50);
                _syncContext.Post(TryRelease, null);
            };

            pt.BeginInvoke(null, null);
        }

        void TryReleaseDelayed()
        {
            for (int i = 0; i < _checks.Count; i++)
            {
                if (!_checks[i]())
                    _checks.RemoveAt(i--);
            }

            lock (_delayTasks)
            {
                if (_checks.Count == 0)
                {
                    try
                    {
                        foreach (PostTask dpc in _delayTasks)
                            dpc();
                    }
                    finally
                    {
                        _delayTasks.Clear();
                        _delayed = false;
                    }
                }
            }
        }

        #endregion

        #region IAnkhCommandService Members

        public CommandResult DirectlyExecCommand(AnkhCommand command)
        {
            return DirectlyExecCommand(command, null, CommandPrompt.DoDefault);
        }

        public CommandResult DirectlyExecCommand(AnkhCommand command, object args)
        {
            return DirectlyExecCommand(command, args, CommandPrompt.DoDefault);
        }

        public CommandResult DirectlyExecCommand(AnkhCommand command, object args, CommandPrompt prompt)
        {
            // TODO: Assert that we are in the UI thread

            CommandMapper mapper = GetService<CommandMapper>();

            if (mapper == null)
                return new CommandResult(false, null);

            CommandEventArgs e = new CommandEventArgs(command, AnkhContext, args, prompt == CommandPrompt.Always, prompt == CommandPrompt.Never);
            bool ok = mapper.Execute(command, e);

            return new CommandResult(ok, e.Result);
        }        

        #endregion

        #region IAnkhCommandService Members


        /// <summary>
        /// Updates the command UI.
        /// </summary>
        /// <param name="performImmediately">if set to <c>true</c> [perform immediately].</param>
        public void UpdateCommandUI(bool performImmediately)
        {
            IVsUIShell shell = UIShell;

            if (shell != null)
                shell.UpdateCommandUI(performImmediately ? 1 : 0);
        }

        #endregion

        #region IAnkhCommandService Members

        public void ShowContextMenu(AnkhCommandMenu menu, int x, int y)
        {
            IMenuCommandService mcs = GetService<IMenuCommandService>();

            IVsUIShell shell = GetService<IVsUIShell>();
            if (mcs != null)
            {
                try
                {
                    mcs.ShowContextMenu(new CommandID(AnkhId.CommandSetGuid, (int)menu), x, y);
                }
                catch (COMException)
                {
                    /* Menu is not declared correctly (no items) */
                }
            }            
        }

        public void ShowContextMenu(AnkhCommandMenu menu, System.Drawing.Point location)
        {
            ShowContextMenu(menu, location.X, location.Y);
        }

        #endregion
    }
}
