using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.Ids;
using Ankh.Commands;

namespace Ankh.Scc
{
    partial class AnkhSccProvider : IVsSelectionEvents
    {
        uint _selCookie;
        uint _sccContextCookie;
        void MonitorActive()
        {
            IVsMonitorSelection monitor = GetService<IVsMonitorSelection>();

            monitor.AdviseSelectionEvents(this, out _selCookie);
            Guid gCook = AnkhId.SccProviderGuid;
            monitor.GetCmdUIContextCookie(ref gCook, out _sccContextCookie);
        }

        int IVsSelectionEvents.OnCmdUIContextChanged(uint dwCmdUICookie, int fActive)
        {
            if (dwCmdUICookie == _sccContextCookie && fActive != 0)
                MaybeRegisterAsPrimarySccProvider();

            return VSConstants.S_OK;
        }

        bool _maybeRegisteredBefore;
        internal void MaybeRegisterAsPrimarySccProvider()
        {
            if (_maybeRegisteredBefore)
                return;

            IVsMonitorSelection monitor = GetService<IVsMonitorSelection>();

            int active;

            if (ErrorHandler.Succeeded(monitor.IsCmdUIContextActive(_sccContextCookie, out active)))
            {
                if (active == 0)
                    return;
            }

            _maybeRegisteredBefore = true;

            if (_selCookie != 0)
            {
                monitor.UnadviseSelectionEvents(_selCookie);
                _selCookie = 0;
            }


            IAnkhCommandStates states = GetService<IAnkhCommandStates>();


            if (states.GetRawOtherSccProviderActive())
            {
                // Ok: We triggered a bug here. We were active but not disabled, because we where not loaded
                monitor.SetCmdUIContext(_sccContextCookie, 0);
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

        #region IVsSelectionEvents Members
        int IVsSelectionEvents.OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
        {
            return VSConstants.S_OK;
        }

        int IVsSelectionEvents.OnSelectionChanged(IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMISOld, ISelectionContainer pSCOld, IVsHierarchy pHierNew, uint itemidNew, IVsMultiItemSelect pMISNew, ISelectionContainer pSCNew)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}
