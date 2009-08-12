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
using Ankh.UI;
using System.Windows.Forms;
using Microsoft.VisualStudio.OLE.Interop;
using System.ComponentModel.Design;
using Ankh.Commands;
using Microsoft.VisualStudio;
using Ankh.Scc.UI;

namespace Ankh.VS.Dialogs
{
    [GlobalService(typeof(IAnkhCommandHandlerInstallerService))]
    class VSCommandInstaller : AnkhService, IAnkhCommandHandlerInstallerService
    {
        public VSCommandInstaller(IAnkhServiceProvider context)
            : base(context)
        {
        }

        public void Install(Control control, System.ComponentModel.Design.CommandID command, EventHandler<Ankh.Commands.CommandEventArgs> handler, EventHandler<Ankh.Commands.CommandUpdateEventArgs> updateHandler)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            IAnkhCommandHookAccessor acc = null;
            Control ctrl = control;
            while (ctrl != null)
            {
                acc = ctrl as IAnkhCommandHookAccessor;

                if (acc != null)
                    break;

                ctrl = ctrl.Parent;
            }
            if (acc == null)
                return; // Can't install hook

            if (acc.CommandHook != null)
            {
                acc.CommandHook.Install(control, command, handler, updateHandler);
                return;
            }

            Control topParent = control.TopLevelControl;

            if (topParent == null)
            {
                Control p = control;
                while (p != null)
                {
                    topParent = p;
                    p = p.Parent;
                }
            }

            IAnkhVSContainerForm ct = topParent as IAnkhVSContainerForm;

            if (ct != null)
            {
                ContextCommandHandler cx = new ContextCommandHandler(this, topParent);
                acc.CommandHook = cx;
                cx.Install(control, command, handler, updateHandler);
                ct.AddCommandTarget(cx);
                return;
            }

            IAnkhToolWindowControl toolWindow = topParent as IAnkhToolWindowControl;
            if (toolWindow != null && toolWindow.ToolWindowHost != null)
            {
                ContextCommandHandler cx = new ContextCommandHandler(this, topParent);
                acc.CommandHook = cx;
                cx.Install(control, command, handler, updateHandler);
                toolWindow.ToolWindowHost.AddCommandTarget(cx);
                return;
            }
        }

    }

    class ContextCommandHandler : AnkhCommandHook, IOleCommandTarget
    {
        class CommandData
        {
            public readonly Control Control;
            public readonly EventHandler<Ankh.Commands.CommandEventArgs> Handler;
            public readonly EventHandler<Ankh.Commands.CommandUpdateEventArgs> UpdateHandler;

            public CommandData(Control control, EventHandler<Ankh.Commands.CommandEventArgs> handler, EventHandler<Ankh.Commands.CommandUpdateEventArgs> updateHandler)
            {
                Control = control;
                Handler = handler;
                UpdateHandler = updateHandler;
            }
        }

        Dictionary<CommandID, List<CommandData>> _data = new Dictionary<CommandID, List<CommandData>>();
        public ContextCommandHandler(IAnkhServiceProvider context, Control control)
            : base(context, control)
        {
        }

        public override void Install(Control control, CommandID command, EventHandler<Ankh.Commands.CommandEventArgs> handler, EventHandler<Ankh.Commands.CommandUpdateEventArgs> updateHandler)
        {
            CommandData cd = new CommandData(control, handler, updateHandler);

            List<CommandData> items;
            if (!_data.TryGetValue(command, out items))
                _data[command] = items = new List<CommandData>();

            items.Add(cd);
        }

        #region IOleCommandTarget Members

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            CommandID cd = new CommandID(pguidCmdGroup, unchecked((int)nCmdID));

            List<CommandData> items;

            if (!_data.TryGetValue(cd, out items))
                return (int)Constants.OLECMDERR_E_NOTSUPPORTED;

            foreach (CommandData d in items)
            {
                if (!d.Control.ContainsFocus)
                    continue;

                CommandEventArgs ce = new CommandEventArgs((AnkhCommand)cd.ID, GetService<AnkhContext>());
                if (d.UpdateHandler != null)
                {
                    CommandUpdateEventArgs ud = new CommandUpdateEventArgs(ce.Command, ce.Context);

                    d.UpdateHandler(d.Control, ud);

                    if (!ud.Enabled)
                        return (int)Constants.OLECMDERR_E_DISABLED;
                }

                d.Handler(d.Control, ce);

                return VSConstants.S_OK;
            }

            return (int)Constants.OLECMDERR_E_NOTSUPPORTED;
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if ((prgCmds == null))
                return Microsoft.VisualStudio.VSConstants.E_INVALIDARG;

            System.Diagnostics.Debug.Assert(cCmds == 1, "Multiple commands"); // Should never happen in VS

            CommandID cd = new CommandID(pguidCmdGroup, unchecked((int)prgCmds[0].cmdID));

            List<CommandData> items;

            if (!_data.TryGetValue(cd, out items))
                return (int)Constants.OLECMDERR_E_NOTSUPPORTED;

            foreach (CommandData d in items)
            {
                if (!d.Control.ContainsFocus)
                    continue;

                CommandUpdateEventArgs ee = new CommandUpdateEventArgs((AnkhCommand)cd.ID, GetService<AnkhContext>());

                if (d.UpdateHandler != null)
                    d.UpdateHandler(d.Control, ee);

                OLECMDF cmdf = OLECMDF.OLECMDF_SUPPORTED;

                ee.UpdateFlags(ref cmdf);

                prgCmds[0].cmdf = (uint)cmdf;

                return VSConstants.S_OK;
            }

            return (int)Constants.OLECMDERR_E_NOTSUPPORTED;
        }

        #endregion
    }
}
