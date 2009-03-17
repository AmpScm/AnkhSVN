// $Id$
//
// Copyright 2009 The AnkhSVN Project
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
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using Ankh.UI;
using Ankh.VS;

namespace Ankh.VSPackage
{
    partial class AnkhSvnPackage : IOleComponent
    {
        uint _componentId;
        void RegisterAsOleComponent()
        {
            if (_componentId != 0)
                return;

            IOleComponentManager mgr = GetService<IOleComponentManager>(typeof(SOleComponentManager));

            OLECRINFO[] crInfo = new OLECRINFO[1];
            crInfo[0].cbSize = (uint)Marshal.SizeOf(typeof(OLECRINFO));
            crInfo[0].grfcrf = (uint)(_OLECRF.olecrfNeedIdleTime | _OLECRF.olecrfNeedPeriodicIdleTime);
            crInfo[0].grfcadvf = (uint)_OLECADVF.olecadvfModal;
            crInfo[0].uIdleTimeInterval = 1000;

            uint id;
            if (ErrorHandler.Succeeded(mgr.FRegisterComponent(this, crInfo, out id)))
                _componentId = id;
        }

        void IOleComponent.OnEnterState(uint uStateID, int fEnter)
        {
            if (uStateID == (uint)_OLECSTATE.olecstateModal)
            {
                CommandMapper.SetModal(fEnter != 0);
            }
        }

        static List<IAnkhIdleProcessor> _idleProcessors = new List<IAnkhIdleProcessor>();
        void IAnkhPackage.RegisterIdleProcessor(IAnkhIdleProcessor processor)
        {
            if (processor == null)
                throw new ArgumentNullException("processor");

            _idleProcessors.Add(processor);
        }

        // Returns: BOOL
        int IOleComponent.FDoIdle(uint grfidlef)
        {
            AnkhIdleArgs args = new AnkhIdleArgs(this, unchecked((int)grfidlef));
            bool done = true;

            foreach (IAnkhIdleProcessor pp in _idleProcessors)
            {
                args.Done = true;

                pp.OnIdle(args);

                done = done && args.Done;
            }

            return done ? 0 : 1; // TRUE to be called again
        }

        #region NOOPed IOleComponent Members

        int IOleComponent.FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked)
        {
            return 1; // Please do
        }

        int IOleComponent.FPreTranslateMessage(MSG[] pMsg)
        {
            return 0;
        }

        int IOleComponent.FQueryTerminate(int fPromptUser)
        {
            return 1; // Ok
        }

        int IOleComponent.FReserved1(uint dwReserved, uint message, IntPtr wParam, IntPtr lParam)
        {
            return VSConstants.E_NOTIMPL;
        }

        IntPtr IOleComponent.HwndGetWindow(uint dwWhich, uint dwReserved)
        {
            return IntPtr.Zero;
        }

        void IOleComponent.OnActivationChange(IOleComponent pic, int fSameComponent, OLECRINFO[] pcrinfo, int fHostIsActivating, OLECHOSTINFO[] pchostinfo, uint dwReserved)
        {

        }

        void IOleComponent.OnAppActivate(int fActive, uint dwOtherThreadID)
        {

        }

        void IOleComponent.OnLoseActivation()
        {

        }

        void IOleComponent.Terminate()
        {

        }

        #endregion
    }
}
