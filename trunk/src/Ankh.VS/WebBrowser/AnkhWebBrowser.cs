﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;

namespace Ankh.VS.WebBrowser
{
    class AnkhWebBrowser : IAnkhWebBrowser
    {
        readonly IAnkhServiceProvider _serviceProvider;

        public AnkhWebBrowser(IAnkhServiceProvider context)
        {
            if(context == null)
                throw new ArgumentNullException("context");

            _serviceProvider = context;
        }

        public void Navigate(Uri url)
        {
            BrowserResults results;
            Navigate(url, new BrowserArgs(), out results);
        }

        public void Navigate(Uri url, BrowserArgs args)
        {
            BrowserResults results;
            Navigate(url, args, out results);
        }

        public void Navigate(Uri url, BrowserArgs args, out BrowserResults results)
        {
            IVsWebBrowsingService browserSvc = (IVsWebBrowsingService)_serviceProvider.GetService(typeof(SVsWebBrowsingService));

            Guid windowGuid = new Guid(ToolWindowGuids80.WebBrowserWindow);
            IVsWebBrowser browser;
            IVsWindowFrame ppFrame;
            int hr = browserSvc.CreateWebBrowser(
                (uint)args.CreateFlags,
                ref windowGuid,
                args.BaseCaption,
                url.ToString(),
                new BrowserUser(),
                out browser,
                out ppFrame);
            results = new Results(browser, ppFrame);
        }

        private class Results : BrowserResults
        {
            readonly IVsWebBrowser _browser;
            readonly IVsWindowFrame _frame;
            public Results(IVsWebBrowser browser, IVsWindowFrame frame)
            {
                if (browser == null)
                    throw new ArgumentNullException("browser");
                if (frame == null)
                    throw new ArgumentNullException("frame");

                _browser = browser;
                _frame = frame;
            }
            
            public override IVsWebBrowser WebBrowser
            {
                get { return _browser; }
            }

            public override IVsWindowFrame Frame
            {
                get { return _frame; }
            }
        }

    }

    class BrowserUser : IVsWebBrowserUser
    {
        #region IVsWebBrowserUser Members

        public int Disconnect()
        {
            return VSConstants.S_OK;
        }

        public int FilterDataObject(Microsoft.VisualStudio.OLE.Interop.IDataObject pDataObjIn, out Microsoft.VisualStudio.OLE.Interop.IDataObject ppDataObjOut)
        {
            ppDataObjOut = null;
            return VSConstants.S_OK;
        }

        public int GetCmdUIGuid(out Guid pguidCmdUI)
        {
            pguidCmdUI = Guid.Empty;
            return VSConstants.S_OK;
        }

        public int GetCustomMenuInfo(object pUnkCmdReserved, object pDispReserved, uint dwType, uint dwPosition, out Guid pguidCmdGroup, out int pdwMenuID)
        {
            pguidCmdGroup = Guid.Empty;
            pdwMenuID = 0;
            return VSConstants.S_OK;
        }

        public int GetCustomURL(uint nPage, out string pbstrURL)
        {
            pbstrURL = null;
            return VSConstants.S_OK;
        }

        public int GetDropTarget(Microsoft.VisualStudio.OLE.Interop.IDropTarget pDropTgtIn, out Microsoft.VisualStudio.OLE.Interop.IDropTarget ppDropTgtOut)
        {
            ppDropTgtOut = null;
            return VSConstants.S_OK;
        }

        public int GetExternalObject(out object ppDispObject)
        {

            ppDispObject = null;
            return VSConstants.S_OK;
        }

        public int GetOptionKeyPath(uint dwReserved, out string pbstrKey)
        {
            pbstrKey = null;
            return VSConstants.S_OK;
        }

        public int Resize(int cx, int cy)
        {
            return VSConstants.S_OK;
        }

        public int TranslateAccelarator(Microsoft.VisualStudio.OLE.Interop.MSG[] lpmsg)
        {
            return VSConstants.S_OK;
        }

        public int TranslateUrl(uint dwReserved, string lpszURLIn, out string lppszURLOut)
        {
            lppszURLOut = null;
            return VSConstants.S_OK;
        }

        #endregion
    }

}
