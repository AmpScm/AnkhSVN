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

namespace Ankh
{
    class AnkhCommandService : AnkhService, IAnkhCommandService
    {
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
            get { return _commandDispatcher ?? (_commandDispatcher = GetService<IOleCommandTarget>(typeof(SUIHostCommandDispatcher))); }
        }

        AnkhContext _ankhContext;
        protected AnkhContext AnkhContext
        {
            get { return _ankhContext ?? (_ankhContext = GetService<AnkhContext>()); }
        }

        #region IAnkhCommandService Members

        public bool ExecCommand(Ankh.Ids.AnkhCommand command)
        {
            // The commandhandler in the package always checks enabled; no need to do it here
            return ExecCommand(command, false);
        }

        public bool ExecCommand(Ankh.Ids.AnkhCommand command, bool verifyEnabled)
        {
            // The commandhandler in the package always checks enabled; no need to do it here
            return ExecCommand(new CommandID(AnkhId.CommandSetGuid, (int)command), verifyEnabled);
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

            return ErrorHandler.Succeeded(dispatcher.Exec(ref g, 
                unchecked((uint)command.ID), (uint)OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, IntPtr.Zero, IntPtr.Zero));
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

        public bool PostExecCommand(System.ComponentModel.Design.CommandID command, object args, CommandPrompt prompt)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            // TODO: Verify if we should marshal to the UI thread ourselved

            IVsUIShell shell = UIShell;

            if (shell != null)
            {
                Guid set = command.Guid;
                object a = args;

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

                return VSConstants.S_OK == shell.PostExecCommand(ref set, 
                    unchecked((uint)command.ID), flags, ref a);
            }

            return false;
        }

        #endregion

        #region IAnkhCommandService Members

        public bool DirectlyExecCommand(AnkhCommand command)
        {
            return DirectlyExecCommand(command, null, CommandPrompt.DoDefault);
        }

        public bool DirectlyExecCommand(AnkhCommand command, object args)
        {
            return DirectlyExecCommand(command, args, CommandPrompt.DoDefault);
        }

        public bool DirectlyExecCommand(AnkhCommand command, object args, CommandPrompt prompt)
        {
            // TODO: Assert that we are in the UI thread

            CommandMapper mapper = GetService<CommandMapper>();

            if (mapper == null)
                return false;

            return mapper.Execute(command, new CommandEventArgs(command, AnkhContext, args, prompt == CommandPrompt.Always, prompt == CommandPrompt.Never));
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
    }
}
