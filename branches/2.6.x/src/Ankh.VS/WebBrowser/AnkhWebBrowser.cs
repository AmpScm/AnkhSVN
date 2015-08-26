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
using System.Diagnostics;
using Ankh.Configuration;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.VS.WebBrowser
{
    [GlobalService(typeof(IAnkhWebBrowser))]
    sealed class AnkhWebBrowser : AnkhService, IAnkhWebBrowser
    {
        public AnkhWebBrowser(IAnkhServiceProvider context)
            : base(context)
        {
        }

        public void Navigate(Uri url)
        {
            Navigate(url, new AnkhBrowserArgs());
        }

        public void Navigate(Uri url, AnkhBrowserArgs args)
        {
            AnkhBrowserResults results;

            bool useExternal = args.External;

            IAnkhConfigurationService cs = GetService<IAnkhConfigurationService>();
            if (cs != null && cs.Instance.ForceExternalBrowser)
                useExternal = true;

            if (args != null && useExternal)
            {
                try
                {
                    NavigateInExternalBrowser(url);
                    return;
                }
                catch { } // BA: log/ignore the exception, and open the URL using VS's browser service
            }
            Navigate(url, args, out results);
        }

        public void Navigate(Uri url, AnkhBrowserArgs args, out AnkhBrowserResults results)
        {
            IVsWebBrowsingService browserSvc = GetService<IVsWebBrowsingService>(typeof(SVsWebBrowsingService));

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

        private void NavigateInExternalBrowser(Uri url)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = url.AbsoluteUri;
            startInfo.UseShellExecute = true;
            startInfo.Verb = "Open";
            Process.Start(startInfo);
        }

        private class Results : AnkhBrowserResults
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
            return VSErr.S_OK;
        }

        public int FilterDataObject(Microsoft.VisualStudio.OLE.Interop.IDataObject pDataObjIn, out Microsoft.VisualStudio.OLE.Interop.IDataObject ppDataObjOut)
        {
            ppDataObjOut = null;
            return VSErr.S_OK;
        }

        public int GetCmdUIGuid(out Guid pguidCmdUI)
        {
            pguidCmdUI = Guid.Empty;
            return VSErr.S_OK;
        }

        public int GetCustomMenuInfo(object pUnkCmdReserved, object pDispReserved, uint dwType, uint dwPosition, out Guid pguidCmdGroup, out int pdwMenuID)
        {
            pguidCmdGroup = Guid.Empty;
            pdwMenuID = 0;
            return VSErr.S_OK;
        }

        public int GetCustomURL(uint nPage, out string pbstrURL)
        {
            pbstrURL = null;
            return VSErr.S_OK;
        }

        public int GetDropTarget(Microsoft.VisualStudio.OLE.Interop.IDropTarget pDropTgtIn, out Microsoft.VisualStudio.OLE.Interop.IDropTarget ppDropTgtOut)
        {
            ppDropTgtOut = null;
            return VSErr.S_OK;
        }

        public int GetExternalObject(out object ppDispObject)
        {

            ppDispObject = null;
            return VSErr.S_OK;
        }

        public int GetOptionKeyPath(uint dwReserved, out string pbstrKey)
        {
            pbstrKey = null;
            return VSErr.S_OK;
        }

        public int Resize(int cx, int cy)
        {
            return VSErr.S_OK;
        }

        public int TranslateAccelarator(Microsoft.VisualStudio.OLE.Interop.MSG[] lpmsg)
        {
            return VSErr.S_OK;
        }

        public int TranslateUrl(uint dwReserved, string lpszURLIn, out string lppszURLOut)
        {
            lppszURLOut = null;
            return VSErr.S_OK;
        }

        #endregion
    }

}
