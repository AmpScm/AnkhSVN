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
    /// <summary>
    /// 
    /// </summary>
    public interface IAnkhDialogHelpService
    {
        /// <summary>
        /// Shows generic help for the specified form
        /// </summary>
        /// <param name="form">The form.</param>
        void RunHelp(VSDialogForm form);
    }

    /// <summary>
    /// .Net form which when shown modal let's the VS command routing continue
    /// </summary>
    /// <remarks>If the IAnkhDialogOwner service is not available this form behaves like any other form</remarks>
    public class VSDialogForm : System.Windows.Forms.Form, IAnkhServiceProvider
    {
        IAnkhServiceProvider _context;
        IAnkhDialogOwner _dlgOwner;

        /// <summary>
        /// Initializes a new instance of the <see cref="VSContainerForm"/> class.
        /// </summary>
        public VSDialogForm()
        {
            ShowInTaskbar = false;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VSContainerForm"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        protected VSDialogForm(IContainer container)
            : this()
        {
            container.Add(this);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set
            {
                if (_context != value)
                {
                    _context = value;
                    _dlgOwner = null;
                    OnContextChanged(EventArgs.Empty);
                }
            }
        }

        [DefaultValue(false)]
        public new bool ShowInTaskbar
        {
            get { return base.ShowInTaskbar; }
            set { base.ShowInTaskbar = value; }
        }

        protected virtual void OnContextChanged(EventArgs e)
        {
        }

        /// <summary>
        /// Gets the dialog owner service
        /// </summary>
        /// <value>The dialog owner.</value>
        [Browsable(false), CLSCompliant(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected IAnkhDialogOwner DialogOwner
        {
            get
            {
                if (_dlgOwner == null && _context != null)
                    _dlgOwner = ((IAnkhServiceProvider)this).GetService<IAnkhDialogOwner>();

                return _dlgOwner;
            }
        }

        /// <summary>
        /// Gets the name of the dialog for help references
        /// </summary>
        /// <value>The name of the dialog help type.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DialogHelpTypeName
        {
            get { return GetType().FullName; }
        }

        bool _addedHelp;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode)
                return;

            if (!this.HelpButton && this.ControlBox && !_addedHelp)
            {
                IAnkhDialogHelpService helpService = GetService<IAnkhDialogHelpService>();

                if (helpService != null)
                {
                    _addedHelp = true;
                    HelpButton = true;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.HelpButtonClicked"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the event data.</param>
        protected override void OnHelpButtonClicked(CancelEventArgs e)
        {
            base.OnHelpButtonClicked(e);

            if (_addedHelp && !e.Cancel)
            {
                e.Cancel = true; // Don't go in context help mode

                IAnkhDialogHelpService helpService = GetService<IAnkhDialogHelpService>();

                helpService.RunHelp(this);
            }
        }

        /// <summary>
        /// Obsolete: Use ShowDialog(Context)
        /// </summary>
        [Obsolete("Always use ShowDialog(Context) even when the context is already set")]
        public new DialogResult ShowDialog()
        {
            if (Context != null)
                return ShowDialog(Context);
            else
                return ShowDialog(new AnkhServiceContainer());
        }

        [Obsolete("Always use ShowDialog(Context) even when the context is already set")]
        public new DialogResult ShowDialog(IWin32Window owner)
        {
            if (Context != null)
                return ShowDialog(Context, owner);
            else
                return ShowDialog(new AnkhServiceContainer(), owner);
        }

        /// <summary>
        /// Shows the form as a modal dialog box with the VS owner window
        /// </summary>
        /// <param name="context">The context.</param>
        public DialogResult ShowDialog(IAnkhServiceProvider context)
        {
            if (context == null && _context == null)
                throw new ArgumentNullException("context");

            return ShowDialog(context, null);
        }

        /// <summary>
        /// Show the form as a modal dialog with the specified owner window
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="owner">The owner.</param>
        public DialogResult ShowDialog(IAnkhServiceProvider context, IWin32Window owner)
        {
            bool setContext = false;

            if (context == null)
            {
                if (Context == null)
                    throw new ArgumentNullException("context");
            }
            else if (Context == null)
                setContext = true;
            else if (context != Context)
                throw new ArgumentOutOfRangeException("context", "context must match context or be null");

            if (setContext)
                Context = context;
            try
            {
                using (DialogRunContext(Context))
                {
                    OnBeforeShowDialog(EventArgs.Empty);

                    IUIService uiService = Context.GetService<IUIService>();

                    try
                    {
                        return RunDialog(owner, uiService);
                    }
                    finally
                    {
                        OnAfterShowDialog(EventArgs.Empty);
                    }
                }
            }
            finally
            {
                if (setContext)
                    Context = null;
            }
        }

        protected virtual IDisposable DialogRunContext(IAnkhServiceProvider context)
        {
            return null;
        }

        /// <summary>
        /// Runs the dialog.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="uiService">The UI service.</param>
        /// <returns></returns>
        internal virtual DialogResult RunDialog(IWin32Window owner, IUIService uiService)
        {
            if (uiService != null)
                return uiService.ShowDialog(this);
            else
                return base.ShowDialog(owner);
        }

        protected virtual void OnBeforeShowDialog(EventArgs e)
        {
        }

        protected virtual void OnAfterShowDialog(EventArgs e)
        {

        }    

        #region IAnkhServiceProvider Members

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service to get</typeparam>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        [DebuggerStepThrough]
        T IAnkhServiceProvider.GetService<T>()
        {
            return GetService<T>();
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service to get</typeparam>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        protected T GetService<T>()
            where T : class
        {
            return ((IAnkhServiceProvider)this).GetService<T>(typeof(T));            
        }

        [DebuggerStepThrough]
        T IAnkhServiceProvider.GetService<T>(Type serviceType)
        {
            T value;
            if (Context != null)
            {
                value = Context.GetService<T>(serviceType);

                if (value != null)
                    return value;
            }

            return base.GetService(serviceType) as T;
        }

        #endregion

        #region IServiceProvider Members

        [DebuggerStepThrough]
        object System.IServiceProvider.GetService(Type serviceType)
        {
            object value;

            if (Context != null)
            {
                value = Context.GetService(serviceType);

                if (value != null)
                    return value;
            }

            return base.GetService(serviceType);
        }

        #endregion
    }
}
