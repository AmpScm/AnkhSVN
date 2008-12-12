// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;

namespace Ankh.Scc
{
    [GlobalService(typeof(IAnkhTextEditingTracker))]
    class TextEditingTracker : AnkhService, IAnkhTextEditingTracker, IVsTextManagerEvents2
    {
        IVsTextManager _manager;
        IVsTextManager2 _manager2;
        IConnectionPoint _connectionPoint;
        uint _cookie;

        public TextEditingTracker(IAnkhServiceProvider context)
            : base(context)
        {
        }

        IVsTextManager Manager
        {
            get { return _manager ?? (_manager = GetService<IVsTextManager>(typeof(SVsTextManager))); }
        }

        IVsTextManager2 Manager2
        {
            get { return _manager2 ?? (_manager2 = GetService<IVsTextManager2>(typeof(SVsTextManager))); }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            IConnectionPointContainer ct = Manager2 as IConnectionPointContainer ?? Manager as IConnectionPointContainer;

            if (ct != null)
            {
                Guid gv = typeof(IVsTextManagerEvents2).GUID;

                ct.FindConnectionPoint(ref gv, out _connectionPoint);

                if (_connectionPoint != null)
                {
                    _connectionPoint.Advise(this, out _cookie);
                }                
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_connectionPoint != null)
                    _connectionPoint.Unadvise(_cookie);
            }
            finally
            {
                _connectionPoint = null;
                base.Dispose(disposing);
            }
        }

        public int OnRegisterMarkerType(int iMarkerType)
        {
            return VSConstants.S_OK;
        }

        public int OnRegisterView(IVsTextView pView)
        {
            return VSConstants.S_OK;
        }

        public int OnReplaceAllInFilesBegin()
        {
            return VSConstants.S_OK;
        }

        public int OnReplaceAllInFilesEnd()
        {
            return VSConstants.S_OK;
        }

        public int OnUnregisterView(IVsTextView pView)
        {
            return VSConstants.S_OK;
        }

        public int OnUserPreferencesChanged2(VIEWPREFERENCES2[] pViewPrefs, FRAMEPREFERENCES2[] pFramePrefs, LANGPREFERENCES2[] pLangPrefs, FONTCOLORPREFERENCES2[] pColorPrefs)
        {
            return VSConstants.S_OK;
        }
    }
}
