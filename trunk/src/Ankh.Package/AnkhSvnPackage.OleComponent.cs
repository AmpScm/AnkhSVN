﻿using System;
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

        #region NOOPed IOleComponent Members

        int IOleComponent.FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked)
        {
            return 1; // Please do
        }

        static List<IAnkhIdleProcessor> _idleProcessors = new List<IAnkhIdleProcessor>();
        void IAnkhPackage.RegisterIdleProcessor(IAnkhIdleProcessor processor)
        {
            if (processor == null)
                throw new ArgumentNullException("processor");

            _idleProcessors.Add(processor);
        }

        int IOleComponent.FDoIdle(uint grfidlef)
        {
            bool periodic = 0 != (grfidlef & (uint)_OLEIDLEF.oleidlefPeriodic);
            bool done = true;

            foreach (IAnkhIdleProcessor pp in _idleProcessors)
            {
                done = pp.OnIdle(periodic) && done;
            }

            return done ? VSConstants.S_OK : VSConstants.S_FALSE;
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
