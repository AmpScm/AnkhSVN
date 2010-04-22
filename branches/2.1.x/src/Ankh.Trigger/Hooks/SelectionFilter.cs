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
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Diagnostics;

namespace Ankh.Trigger.Hooks
{
    sealed class SelectionFilter : IVsSelectionEvents, IDisposable
    {
        readonly IVsUIShell _shell;
        readonly IVsMonitorSelection _monitor;
        readonly IServiceProvider _sp;
        uint _cookie;

        uint _sccCookie;

        public SelectionFilter(IServiceProvider sp, IVsMonitorSelection monitor)
        {
            if (sp == null)
                throw new ArgumentNullException("sp");
            else if (monitor == null)
                throw new ArgumentNullException("monitor");

            _monitor = monitor;

            if (!ErrorHandler.Succeeded(_monitor.AdviseSelectionEvents(this, out _cookie)))
                _cookie = 0;

            Guid g = AnkhId.SccProviderGuid;
            if (!ErrorHandler.Succeeded(_monitor.GetCmdUIContextCookie(ref g, out _sccCookie)))
                _sccCookie = 0;

            _sp = sp;
            _shell = (IVsUIShell)sp.GetService(typeof(IVsUIShell));
        }

        Guid _grp = AnkhId.CommandSetGuid;
        internal void PostCommand(AnkhCommand command)
        {
            object n = null;
            _shell.PostExecCommand(ref _grp, (uint)command, 0, ref n);
        }

        void IDisposable.Dispose()
        {
            if (_cookie != 0)
            {
                _monitor.UnadviseSelectionEvents(_cookie);
                _cookie = 0;
            }
        }

        int IVsSelectionEvents.OnCmdUIContextChanged(uint dwCmdUICookie, int fActive)
        {
            bool active = (fActive != 0);
            if (dwCmdUICookie == _sccCookie && _sccCookie != 0)
            {
                if (_loaded && active)
                {
                    PostCommand(AnkhCommand.ActivateSccProvider);
                }
            }
            return VSConstants.S_OK;
        }

        int IVsSelectionEvents.OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
        {
            return VSConstants.S_OK;
        }

        int IVsSelectionEvents.OnSelectionChanged(IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMISOld, ISelectionContainer pSCOld, IVsHierarchy pHierNew, uint itemidNew, IVsMultiItemSelect pMISNew, ISelectionContainer pSCNew)
        {
            return VSConstants.S_OK;
        }

        bool _loaded;
        internal void Load()
        {
            if (_loaded)
                return;

            _loaded = true;
            int active;

            if (ErrorHandler.Succeeded(_monitor.IsCmdUIContextActive(_sccCookie, out active)) && (active != 0))
            {
                PostCommand(AnkhCommand.ActivateSccProvider);
            }
        }
    }
}
