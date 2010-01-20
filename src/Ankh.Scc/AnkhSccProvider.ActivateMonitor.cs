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
using Ankh.Commands;

namespace Ankh.Scc
{
    partial class AnkhSccProvider
    {
        IVsMonitorSelection _selectionMonitor;
        uint _sccContextCookie;

        IVsMonitorSelection SelectionMonitor
        {
            get { return _selectionMonitor ?? (_selectionMonitor = GetService<IVsMonitorSelection>()); }
        }

        uint SccContextCookie
        {
            get
            {
                if (_sccContextCookie == 0)
                {
                    Guid gCook = AnkhId.SccProviderGuid;
                    if (!ErrorHandler.Succeeded(SelectionMonitor.GetCmdUIContextCookie(ref gCook, out _sccContextCookie)))
                        _sccContextCookie = 0;
                }
                return _sccContextCookie;
            }
        }

        bool _tryRegisteredBefore;
        bool _wasZombie;
        internal void TryRegisterSccProvider()
        {
            IAnkhCommandStates states;

            if (IsActive)
            {
                if (_wasZombie)
                {
                    states = GetService<IAnkhCommandStates>();

                    if (states != null && states.UIShellAvailable)
                    {
                        _wasZombie = false;

                        GetService<IAnkhServiceEvents>().OnSccShellActivate(EventArgs.Empty);
                    }
                }

                return;
            }
            if (_tryRegisteredBefore || SelectionMonitor == null)
                return;

            int active;
            if (!ErrorHandler.Succeeded(SelectionMonitor.IsCmdUIContextActive(SccContextCookie, out active))
                || (active == 0))
            {
                return;
            }

            states = GetService<IAnkhCommandStates>();

            if (states.GetRawOtherSccProviderActive())
            {
                // Ok: We triggered a bug here. We were active but not disabled, because we weren't loaded
                SelectionMonitor.SetCmdUIContext(_sccContextCookie, 0);
            }
            else
            {
                // Ok, Visual Studio decided to activate the user context with our GUID
                // This tells us VS wants us to be the active SCC
                //
                // This is not documented directly. But it is documented that we should
                // enable our commands on that context

                // Set us active; this makes VS initialize the provider
                RegisterAsPrimarySccProvider();

                _tryRegisteredBefore = true;

                if (!states.UIShellAvailable)
                    _wasZombie = true;
            }
        }
    }
}
