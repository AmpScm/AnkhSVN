using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Remoting.Contexts;
using Ankh.VS;
using Microsoft.VisualStudio.Shell.Interop;
using System.ComponentModel.Design;
using System.ComponentModel;
using Ankh.Ids;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio.OLE.Interop;
using System.Diagnostics;

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
    public class VSContainerForm : System.Windows.Forms.Form, IAnkhVSContainerForm, IAnkhServiceProvider, IAnkhCommandHookAccessor
    {
        IAnkhServiceProvider _context;
        IAnkhDialogOwner _dlgOwner;
        AnkhToolBar _toolbarId;
        VSContainerMode _mode;

        public VSContainerForm()
        {
            ShowInTaskbar = false;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
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

        protected virtual void OnContextChanged(EventArgs e)
        {
        }

        /// <summary>
        /// Gets the dialog owner service
        /// </summary>
        /// <value>The dialog owner.</value>
        [Browsable(false), CLSCompliant(false)]
        protected IAnkhDialogOwner DialogOwner
        {
            get 
            { 
                if(_dlgOwner == null && _context != null)
                    _dlgOwner = _context.GetService<IAnkhDialogOwner>();

                return _dlgOwner; 
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
            if(context == null && _context == null)
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

            if(context == null)
            {
                if(Context == null)
                    throw new ArgumentNullException("context");
            }
            else if(Context == null)
                setContext = true;
            else if(context != Context)
                throw new ArgumentOutOfRangeException("context", "context must match context or be null");
            
            if(setContext)
                Context = context;

            IUIService uiService = null;

            if(Context != null)
                uiService = Context.GetService<IUIService>();

            try
            {
                if(owner == null && DialogOwner != null)
                    owner = DialogOwner.DialogOwner;

                OnBeforeShowDialog(EventArgs.Empty);

                DialogResult rslt;

                if (DialogOwner != null)
                {
                    using (DialogOwner.InstallFormRouting(this, EventArgs.Empty))
                    {
                        if (uiService != null)
                            rslt = uiService.ShowDialog(this);
                        else
                            rslt = base.ShowDialog(owner);
                    }
                }
                else
                {
                    if (uiService != null)
                        rslt = uiService.ShowDialog(this);
                    else
                        rslt = base.ShowDialog(owner);
                }

                OnAfterShowDialog(EventArgs.Empty);

                return rslt;
            }
            finally
            {
                if (setContext)
                    Context = null;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (DialogOwner != null)
                DialogOwner.OnContainerCreated(this);
        }

        protected virtual void OnBeforeShowDialog(EventArgs e)
        {
        }

        protected virtual void OnAfterShowDialog(EventArgs e)
        {

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

            if(DialogOwner == null)
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

        #region IAnkhServiceProvider Members

        [DebuggerStepThrough]
        T IAnkhServiceProvider.GetService<T>()
        {
            if (Context != null)
                return Context.GetService<T>();
            
            return null;
        }

        [DebuggerStepThrough]
        T IAnkhServiceProvider.GetService<T>(Type serviceType)
        {
            if (Context != null)
                return Context.GetService<T>(serviceType);

            return null;
        }

        #endregion

        #region IServiceProvider Members

        [DebuggerStepThrough]
        object System.IServiceProvider.GetService(Type serviceType)
        {
            if (Context != null)
                return Context.GetService(serviceType);

            return null;
        }

        #endregion

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
