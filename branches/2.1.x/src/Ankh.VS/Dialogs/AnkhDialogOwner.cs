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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;

using Ankh.UI;

namespace Ankh.VS.Dialogs
{
    [GlobalService(typeof(IAnkhDialogOwner))]
    sealed class AnkhDialogOwner : AnkhService, IAnkhDialogOwner
    {
        IVsUIShell _shell;
        IUIService _uiService;

        public AnkhDialogOwner(IAnkhServiceProvider context)
            : base(context)
        {
        }

        IVsUIShell Shell
        {
            get { return _shell ?? (_shell = GetService<IVsUIShell>(typeof(SVsUIShell))); }
        }

        IUIService UIService
        {
            get { return _uiService ?? (_uiService = GetService<IUIService>()); }
        }

        #region IAnkhDialogOwner Members

        public IWin32Window DialogOwner
        {
            get
            {
                if (UIService != null)
                    return UIService.GetDialogOwnerWindow();
                else
                    return null;
            }
        }

        #region IAnkhDialogOwner Members

        public IDisposable InstallFormRouting(Ankh.UI.VSContainerForm container, EventArgs eventArgs)
        {
            return new VSCommandRouting(Context, container);
        }

        public void OnContainerCreated(Ankh.UI.VSContainerForm form)
        {
            VSCommandRouting routing = VSCommandRouting.FromForm(form);

            if (routing != null)
                routing.OnHandleCreated();
        }
        #endregion

        #endregion

        #region IAnkhDialogOwner Members

        public AnkhMessageBox MessageBox
        {
            get { return new AnkhMessageBox(this); }
        }

        #endregion

        #region IAnkhDialogOwner Members


        public void AddCommandTarget(Ankh.UI.VSContainerForm form, IOleCommandTarget commandTarget)
        {
            VSCommandRouting routing = VSCommandRouting.FromForm(form);

            if (routing != null)
                routing.AddCommandTarget(commandTarget);
            else
                throw new InvalidOperationException("Command routing not initialized yet");
        }

        public void AddWindowPane(Ankh.UI.VSContainerForm form, IVsWindowPane pane)
        {
            VSCommandRouting routing = VSCommandRouting.FromForm(form);

            if (routing != null)
                routing.AddWindowPane(pane);
            else
                throw new InvalidOperationException("Command routing not initialized yet");
        }

        #endregion
        
    }
}
