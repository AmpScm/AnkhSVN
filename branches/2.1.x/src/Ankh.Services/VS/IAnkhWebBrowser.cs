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

namespace Ankh.VS
{
    public interface IAnkhWebBrowser
    {
        void Navigate(Uri url);
        void Navigate(Uri url, AnkhBrowserArgs args);
        void Navigate(Uri url, AnkhBrowserArgs args, out AnkhBrowserResults results);
    }

    public class AnkhBrowserArgs
    {
        __VSCREATEWEBBROWSER _createFlags = __VSCREATEWEBBROWSER.VSCWB_AutoShow |
                        __VSCREATEWEBBROWSER.VSCWB_NoHistory |
                        __VSCREATEWEBBROWSER.VSCWB_StartCustom |
                        __VSCREATEWEBBROWSER.VSCWB_OptionDisableStatusBar;
        string _baseCaption;
        bool _external;

        public string BaseCaption
        {
            get { return _baseCaption; }
            set { _baseCaption = value; }
        }

        [CLSCompliant(false)]
        public __VSCREATEWEBBROWSER CreateFlags
        {
            get { return _createFlags; }
            set { _createFlags = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to start the browser outside the main window
        /// </summary>
        /// <value><c>true</c> if external; otherwise, <c>false</c>.</value>
        public bool External
        {
            get { return _external; }
            set { _external = value; }
        }
    }

    public abstract class AnkhBrowserResults
    {
        [CLSCompliant(false)]
        public virtual IVsWebBrowser WebBrowser 
        { 
            get { throw new NotImplementedException(); }
        }
        [CLSCompliant(false)]
        public virtual IVsWindowFrame Frame
        {
            get { throw new NotImplementedException(); }
        }
    }
}
