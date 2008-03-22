using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using IMessageFiler = System.Windows.Forms.IMessageFilter;
using IServiceProvider = System.IServiceProvider;
using IOleCommandTarget = Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget;

namespace Ankh.VS.Dialogs
{
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class AnkhDialogContainerWithToolbar : DialogContainerWithToolbar
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="AnkhDialogContainerWithToolbar"/> class.
        /// </summary>
        /// <param name="sp">The sp.</param>
        /// <param name="contained">The contained.</param>
        /// <param name="parentCommandTarget">The parent command target.</param>
        public AnkhDialogContainerWithToolbar(IServiceProvider sp, Control contained, IOleCommandTarget parentCommandTarget)
            : base(sp, contained, parentCommandTarget)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnkhDialogContainerWithToolbar"/> class.
        /// </summary>
        /// <param name="sp">The sp.</param>
        /// <param name="contained">The contained.</param>
        public AnkhDialogContainerWithToolbar(IServiceProvider sp, Control contained)
            : base(sp, contained)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnkhDialogContainerWithToolbar"/> class.
        /// </summary>
        /// <param name="sp">The sp.</param>
        public AnkhDialogContainerWithToolbar(IServiceProvider sp)
            : base(sp)
        {
        }

        /// <include file='doc\DialogContainerWithToolbar.uex' path='docs/doc[@for="DialogContainerWithToolbar.DialogContainerWithToolbar3"]/*' />
        /// <devdoc>
        /// Constructor of the DialogContainerWithToolbar.
        /// </devdoc>
        public AnkhDialogContainerWithToolbar()
        {
        }

        /// <summary>
        /// Shows the form as a modal dialog box with the specified owner.
        /// </summary>
        /// <param name="owner">Any object that implements <see cref="T:System.Windows.Forms.IWin32Window"/> that represents the top-level window that will own the modal dialog box.</param>
        /// <returns>
        /// One of the <see cref="T:System.Windows.Forms.DialogResult"/> values.
        /// </returns>      
        public new DialogResult ShowDialog(IWin32Window owner)
        {
            DialogResult result;
            IMessageFilter filter = this as IMessageFilter;

            // Make sure that there is non visual containment for this form
            ShowDialogContainer dialogContainer = null;
            if (this.Site == null)
            {
                dialogContainer = new ShowDialogContainer((IServiceProvider)this);
                dialogContainer.Add(this);
            }

            try
            {
                // This form needs to install its message filter in order to
                // let the toolbar process the mouse events.
                Application.AddMessageFilter(filter);

                // Show the modal dialog
                result = base.ShowDialog(owner);
            }
            finally
            {
                if (dialogContainer != null)
                    dialogContainer.Remove(this);
                Application.RemoveMessageFilter(filter);
            }

            return result;
        }

        private class ShowDialogContainer : Container
        {
            readonly IServiceProvider _provider;
            public ShowDialogContainer(IServiceProvider sp)
            {
                _provider = sp;
            }

            protected override object GetService(Type serviceType)
            {
                if (_provider != null)
                {
                    object service = _provider.GetService(serviceType);
                    if (null != service)
                        return service;
                }
                return base.GetService(serviceType);
            }
        }
    }
}
