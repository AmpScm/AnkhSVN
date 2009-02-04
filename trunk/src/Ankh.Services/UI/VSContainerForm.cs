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
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;

using Ankh.Ids;
using Ankh.VS;


namespace Ankh.UI
{
    [CLSCompliant(false)]
    public interface IAnkhVSContainerForm
    {
        //IVsToolWindowToolbarHost ToolBarHost { get; }
        VSContainerMode ContainerMode { get; set; }
        void AddCommandTarget(IOleCommandTarget commandTarget);
        void AddWindowPane(IVsWindowPane pane);
    }

    [Flags]
    public enum VSContainerMode
    {
        Default = 0,

        TranslateKeys = 1,
        UseTextEditorScope = 2,
    }


    /// <summary>
    /// .Net form which when shown modal let's the VS command routing continue
    /// </summary>
    /// <remarks>If the IAnkhDialogOwner service is not available this form behaves like any other form</remarks>
    public class VSContainerForm : VSDialogForm, IAnkhVSContainerForm, IAnkhCommandHookAccessor
    {
        AnkhToolBar _toolbarId;
        VSContainerMode _mode;

        /// <summary>
        /// Initializes a new instance of the <see cref="VSContainerForm"/> class.
        /// </summary>
        public VSContainerForm()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VSContainerForm"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        protected VSContainerForm(IContainer container)
            : this()
        {
            container.Add(this);
        }

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>The mode.</value>
        protected VSContainerMode ContainerMode
        {
            get { return _mode; }
            set
            {
                if (_mode != value)
                {
                    _mode = value;

                    // Hook changes?
                }
            }
        }

        VSContainerMode IAnkhVSContainerForm.ContainerMode
        {
            get { return ContainerMode; }
            set { ContainerMode = value; }
        }

        protected override IDisposable DialogRunContext(IAnkhServiceProvider context)
        {
            IAnkhDialogOwner owner = null;
            if (context != null)
                owner = context.GetService<IAnkhDialogOwner>();

            if (owner != null)
                return owner.InstallFormRouting(this, EventArgs.Empty);
            else
                return base.DialogRunContext(context);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (!DesignMode && DialogOwner != null)
                DialogOwner.OnContainerCreated(this);
        }

        public AnkhToolBar ToolBar
        {
            get { return _toolbarId; }
            set { _toolbarId = value; }
        }

        [CLSCompliant(false)]
        protected void AddCommandTarget(IOleCommandTarget commandTarget)
        {
            if (commandTarget == null)
                throw new ArgumentNullException("commandTarget");

            if (DialogOwner == null)
                throw new InvalidOperationException("DialogOwner not available");

            DialogOwner.AddCommandTarget(this, commandTarget);
        }

        [CLSCompliant(false)]
        protected void AddWindowPane(IVsWindowPane pane)
        {
            if (pane == null)
                throw new ArgumentNullException("pane");

            if (DialogOwner == null)
                throw new InvalidOperationException("DialogOwner not available");

            DialogOwner.AddWindowPane(this, pane);
        }

        #region IAnkhVSContainerForm Members

        void IAnkhVSContainerForm.AddCommandTarget(IOleCommandTarget commandTarget)
        {
            AddCommandTarget(commandTarget);
        }

        void IAnkhVSContainerForm.AddWindowPane(IVsWindowPane pane)
        {
            AddWindowPane(pane);
        }

        #endregion

        #region IAnkhCommandHookAccessor Members

        AnkhCommandHook _hook;
        AnkhCommandHook IAnkhCommandHookAccessor.CommandHook
        {
            get { return _hook; }
            set { _hook = value; }
        }

        #endregion
    }
}
