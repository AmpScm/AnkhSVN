using System;
using System.Collections.Generic;
using System.Text;
using Ankh.ContextServices;
using System.Windows.Forms;
using System.Runtime.Remoting.Contexts;
using Ankh.VS;
using Microsoft.VisualStudio.Shell.Interop;
using System.ComponentModel.Design;
using System.ComponentModel;

namespace Ankh.UI
{
    [CLSCompliant(false)]
    public interface IAnkhVSContainerForm
    {
        IVsToolWindowToolbarHost ToolBarHost { get; }
    }

    /// <summary>
    /// .Net form which when shown modal let's the VS command routing continue
    /// </summary>
    /// <remarks>If the IAnkhDialogOwner service is not available this form behaves like any other form</remarks>
    public class VSContainerForm : System.Windows.Forms.Form, IAnkhVSContainerForm
    {
        IAnkhServiceProvider _context;
        IAnkhDialogOwner _dlgOwner;
        CommandID _toolbarId;
        
        [Browsable(false)]
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set 
            {
                if (_context != value)
                {
                    _context = value;
                    _dlgOwner = null;
                }
            }
        }

        /// <summary>
        /// Gets the dialog owner service
        /// </summary>
        /// <value>The dialog owner.</value>
        [Browsable(false)]
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
        [Obsolete("Always use ShowDialog(Context) even when the context is already set", true)]
        public new DialogResult ShowDialog()
        {
            if (Context != null)
                return ShowDialog(Context);
            else
                return ShowDialog(new AnkhServiceContainer());
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
                        rslt = base.ShowDialog(owner);
                    }
                }
                else
                    rslt = base.ShowDialog(owner);

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

        public CommandID ToolBarId
        {
            get { return _toolbarId; }
            set { _toolbarId = value; }
        }

        #region Infrastructure for hosting in VS        

        IVsToolWindowToolbarHost __toolBarHost;

        IVsToolWindowToolbarHost IAnkhVSContainerForm.ToolBarHost
        {
            get { return __toolBarHost; }
        }

        #endregion
    }    
}
