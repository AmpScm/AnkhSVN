using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

namespace Ankh.Trigger
{
    sealed class SelectionFilter : IVsSelectionEvents, IDisposable
    {
        readonly IVsMonitorSelection _monitor;
        uint _cookie;

        public SelectionFilter(IVsMonitorSelection monitor)
        {
            if (monitor == null)
                throw new ArgumentNullException("monitor");

            _monitor = monitor;

            if (!ErrorHandler.Succeeded(_monitor.AdviseSelectionEvents(this, out _cookie)))
                _cookie = 0;
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
    }
}
