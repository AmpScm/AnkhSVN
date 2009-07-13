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
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;

using Ankh.UI;

namespace Ankh.VS
{
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public interface IAnkhDialogOwner
    {
        /// <summary>
        /// Gets the dialog owner.
        /// </summary>
        /// <value>The dialog owner.</value>
        IWin32Window DialogOwner { get; }

        /// <summary>
        /// Installs the form routing.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        IDisposable InstallFormRouting(VSContainerForm container, EventArgs eventArgs);

        /// <summary>
        /// Called when the container has created a handle
        /// </summary>
        /// <param name="vSContainerForm">The v S container form.</param>
        void OnContainerCreated(VSContainerForm vSContainerForm);


        /// <summary>
        /// Gets a message box instance
        /// </summary>
        /// <value>The message box.</value>
        AnkhMessageBox MessageBox { get; }

        /// <summary>
        /// Adds the command target.
        /// </summary>
        /// <param name="commandTarget">The command target.</param>
        void AddCommandTarget(VSContainerForm container, IOleCommandTarget commandTarget);

        /// <summary>
        /// Adds the window pane.
        /// </summary>
        /// <param name="vSContainerForm">The v S container form.</param>
        /// <param name="pane">The pane.</param>
        void AddWindowPane(VSContainerForm vSContainerForm, IVsWindowPane pane);
    }    
}
