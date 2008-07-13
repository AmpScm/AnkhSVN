using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.Ids;
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
                if(_sccContextCookie == 0)
                {
                    Guid gCook = AnkhId.SccProviderGuid;
                    if(!ErrorHandler.Succeeded(SelectionMonitor.GetCmdUIContextCookie(ref gCook, out _sccContextCookie)))
                        _sccContextCookie = 0;
                }
                return _sccContextCookie;
            }
        }

        bool _tryRegisteredBefore;
        internal void TryRegisterSccProvider()
        {
            if (_tryRegisteredBefore)
            {
                return;
            }

            int active;
            if(!ErrorHandler.Succeeded(SelectionMonitor.IsCmdUIContextActive(SccContextCookie, out active))
                || (active == 0))
            {
                return; 
            }

            _tryRegisteredBefore = true;            

            IAnkhCommandStates states = GetService<IAnkhCommandStates>();

            if (states.GetRawOtherSccProviderActive())
            {
                // Ok: We triggered a bug here. We were active but not disabled, because we weren't loaded
                SelectionMonitor.SetCmdUIContext(_sccContextCookie, 0);
            }
            else
            {
                // Ok, Visual decided to activate the user context with our GUID
                // This tells us VS wants us to be the active SCC
                //
                // This is not documented directly. But it is documented that we should
                // enable our commands on that context

                // Set us active; this makes VS initialize the provider
                RegisterAsPrimarySccProvider();
            }
        }
    }
}
