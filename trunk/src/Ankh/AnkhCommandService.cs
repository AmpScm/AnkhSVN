﻿using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Microsoft.VisualStudio.Shell.Interop;
using System.ComponentModel.Design;
using AnkhSvn.Ids;
using Microsoft.VisualStudio.OLE.Interop;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;

namespace Ankh
{
    class AnkhCommandService : IAnkhCommandService
    {
        readonly IAnkhServiceProvider _context;

        public AnkhCommandService(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            _context = context;
        }

        IVsUIShell _uiShell;

        protected IVsUIShell UIShell
        {
            get { return _uiShell ?? (_uiShell = (IVsUIShell)_context.GetService(typeof(SVsUIShell))); }
        }

        IOleCommandTarget _commandDispatcher;
        protected IOleCommandTarget CommandDispatcher
        {
            get { return _commandDispatcher ?? (_commandDispatcher = (IOleCommandTarget)_context.GetService(typeof(SUIHostCommandDispatcher))); }
        }

        AnkhContext _ankhContext;
        protected AnkhContext AnkhContext
        {
            get { return _ankhContext ?? (_ankhContext = _context.GetService<AnkhContext>()); }
        }

        #region IAnkhCommandService Members

        public bool ExecCommand(AnkhSvn.Ids.AnkhCommand command)
        {
            // The commandhandler in the package always checks enabled; no need to do it here
            return ExecCommand(new CommandID(AnkhId.CommandSetGuid, (int)command), false);
        }

        public bool ExecCommand(System.ComponentModel.Design.CommandID command)
        {
            return ExecCommand(command, true);
        }

        public bool ExecCommand(System.ComponentModel.Design.CommandID command, bool verifyEnabled)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            // TODO: Assert that we are in the UI thread

            IOleCommandTarget dispatcher = CommandDispatcher;

            if(dispatcher == null)
                return false;

            Guid g = command.Guid;

            if (verifyEnabled)
            {
                OLECMD[] cmd = new OLECMD[1];
                cmd[0].cmdID = unchecked((uint)command.ID);

                if (VSConstants.S_OK != dispatcher.QueryStatus(ref g, 1, cmd, IntPtr.Zero))
                    return false;

                OLECMDF flags = (OLECMDF)cmd[0].cmdf;

                if ((flags & OLECMDF.OLECMDF_SUPPORTED) == (OLECMDF)0)
                    return false; // Not supported

                if ((flags & OLECMDF.OLECMDF_ENABLED) == (OLECMDF)0)
                    return false; // Not enabled
            }

            return VSConstants.S_OK == dispatcher.Exec(ref g, 
                unchecked((uint)command.ID), (uint)OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, IntPtr.Zero, IntPtr.Zero);
        }

        public bool PostExecCommand(AnkhSvn.Ids.AnkhCommand command)
        {
            return PostExecCommand(new CommandID(AnkhId.CommandSetGuid, (int)command), null);
        }

        public bool PostExecCommand(AnkhSvn.Ids.AnkhCommand command, object args)
        {
            return PostExecCommand(new CommandID(AnkhId.CommandSetGuid, (int)command), args);
        }

        public bool PostExecCommand(System.ComponentModel.Design.CommandID command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            return PostExecCommand(command, null);
        }

        public bool PostExecCommand(System.ComponentModel.Design.CommandID command, object args)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            // TODO: Verify if we should marshal to the UI thread ourselved

            IVsUIShell shell = UIShell;

            if (shell != null)
            {
                Guid set = command.Guid;
                object a = args;

                return VSConstants.S_OK == shell.PostExecCommand(ref set, 
                    unchecked((uint)command.ID), (uint)OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, ref a);
            }

            return false;
        }

        #endregion

        #region IAnkhCommandService Members

        public bool DirectlyExecCommand(AnkhCommand command, object args)
        {
            // TODO: Assert that we are in the UI thread

            CommandMapper mapper = _context.GetService<CommandMapper>();

            if (mapper == null)
                return false;

            return mapper.Execute(command, new CommandEventArgs(command, AnkhContext, args, false, false));
        }        

        #endregion
    }
}
