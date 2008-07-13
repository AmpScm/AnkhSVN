using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.Ids;
using System.Diagnostics;

namespace Ankh.Trigger.Hooks
{
    sealed class SelectionFilter : IVsSelectionEvents, IDisposable
    {
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
        }

        void IDisposable.Dispose()
        {
            if (_cookie != 0)
            {
                _monitor.UnadviseSelectionEvents(_cookie);
                _cookie = 0;
            }
        }

        bool _scheduled;
        int IVsSelectionEvents.OnCmdUIContextChanged(uint dwCmdUICookie, int fActive)
        {
            bool active = (fActive != 0);
            if (dwCmdUICookie == _sccCookie && _sccCookie != 0)
            {
                if (!_scheduled)
                {
                    _scheduled = true;

                    IAnkhScheduler scheduler = (IAnkhScheduler)_sp.GetService(typeof(IAnkhScheduler));

                    if (scheduler != null)
                        scheduler.Schedule(new TimeSpan(0, 0, 8), AnkhCommand.ActivateSccProvider);
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
    }
}
