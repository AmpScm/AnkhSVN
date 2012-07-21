using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using VSConstants = Microsoft.VisualStudio.VSConstants;

namespace Ankh.VS
{
    [GlobalService(typeof(IAnkhGlobalCommandHook))]
    sealed class GlobalCommandHook : AnkhService, IAnkhGlobalCommandHook, IOleCommandTarget
    {
        readonly Dictionary<Guid, Dictionary<int, EventHandler>> _commandMap = new Dictionary<Guid,Dictionary<int,EventHandler>>();
        bool _hooked;
        uint _cookie;
        public GlobalCommandHook(IAnkhServiceProvider context)
            : base(context)
        {
        }

        public void HookCommand(System.ComponentModel.Design.CommandID command, EventHandler handler)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            else if (handler == null)
                throw new ArgumentNullException("handler");

            Dictionary<int, EventHandler> map;

            if (!_commandMap.TryGetValue(command.Guid, out map))
            {
                map = new Dictionary<int, EventHandler>();
                _commandMap[command.Guid] = map;
            }

            EventHandler handlers;
            if (!map.TryGetValue(command.ID, out handlers))
                handlers = null;

            map[command.ID] = (handlers + handler);

            if (!_hooked)
            {
                IVsRegisterPriorityCommandTarget svc = GetService<IVsRegisterPriorityCommandTarget>(typeof(SVsRegisterPriorityCommandTarget));

                if (svc != null
                    && ErrorHandler.Succeeded(svc.RegisterPriorityCommandTarget(0, this, out _cookie)))
                {
                    _hooked = true;
                }
            }
        }

        public void UnhookCommand(System.ComponentModel.Design.CommandID command, EventHandler handler)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            else if (handler == null)
                throw new ArgumentNullException("handler");
            Dictionary<int, EventHandler> map;

            if (!_commandMap.TryGetValue(command.Guid, out map))
                return;

            EventHandler handlers;
            if (!map.TryGetValue(command.ID, out handlers))
                return;

            handlers -= handler;

            if (handlers == null)
            {
                map.Remove(command.ID);

                if (map.Count == 0)
                {
                    _commandMap.Remove(command.Guid);

                    if (_commandMap.Count == 0)
                    {
                        Unhook();
                    }
                }
            }

            map[command.ID] = (handlers + handler);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                    Unhook();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        void Unhook()
        {
            if (_hooked)
            {
                _hooked = false;
                GetService<IVsRegisterPriorityCommandTarget>(typeof(SVsRegisterPriorityCommandTarget)).UnregisterPriorityCommandTarget(_cookie);
            }
        }

        static bool GuidRefIsNull(ref Guid pguidCmdGroup)
        {
            // According to MSDN the Guid for the command group can be null and in this case the default
            // command group should be used. Given the interop definition of IOleCommandTarget, the only way
            // to detect a null guid is to try to access it and catch the NullReferenceExeption.
            Guid commandGroup;
            try
            {
                commandGroup = pguidCmdGroup;
            }
            catch (NullReferenceException)
            {
                // Here we assume that the only reason for the exception is a null guidGroup.
                // We do not handle the default command group as definied in the spec for IOleCommandTarget,
                // so we have to return OLECMDERR_E_NOTSUPPORTED.
                return true;
            }

            return false;
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (GuidRefIsNull(ref pguidCmdGroup))
                return (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;

            Dictionary<int, EventHandler> cmdMap;
            if (!_commandMap.TryGetValue(pguidCmdGroup, out cmdMap))
                return (int)OLEConstants.OLECMDERR_E_UNKNOWNGROUP;

            EventHandler handler;
            if (cmdMap.TryGetValue((int)nCmdID, out handler))
            {
                handler(this, EventArgs.Empty);
            }

            return (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if ((prgCmds == null))
                return VSConstants.E_POINTER;
            else if (GuidRefIsNull(ref pguidCmdGroup))
                return (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;

            Dictionary<int, EventHandler> cmdMap;
            if (!_commandMap.TryGetValue(pguidCmdGroup, out cmdMap))
                return (int)OLEConstants.OLECMDERR_E_UNKNOWNGROUP;

            if (!cmdMap.ContainsKey((int)prgCmds[0].cmdID))
                return (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;

            return (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;
        }
    }
}
