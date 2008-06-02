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
