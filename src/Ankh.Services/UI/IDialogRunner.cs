using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using AnkhSvn.Ids;
using Microsoft.VisualStudio.Shell.Interop;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Ankh.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class DialogRunnerArgs
    {
        IWin32Window _owner;
        AnkhCommandMenu _toolbar;
        VSTWT_LOCATION _location;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogRunnerArgs"/> class.
        /// </summary>
        public DialogRunnerArgs()
        {
            _location = VSTWT_LOCATION.VSTWT_TOP;
        }

        /// <summary>
        /// Gets or sets the tool bar.
        /// </summary>
        /// <value>The tool bar.</value>
        public AnkhCommandMenu ToolBar
        {
            get { return _toolbar; }
            set { _toolbar = value; }
        }

        /// <summary>
        /// Gets or sets the tool bar location.
        /// </summary>
        /// <value>The tool bar location.</value>
        [CLSCompliant(false)]
        public VSTWT_LOCATION ToolBarLocation
        {
            get { return _location; }
            set { _location = value; }
        }

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>The owner.</value>
        public IWin32Window Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }
    }

    public interface IDialogRunner
    {
        /// <summary>
        /// Shows the specified form while continuing routing VS commands
        /// </summary>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        DialogResult ShowDialog(Form form);
        /// <summary>
        /// Shows the specified form while continuing routing VS commands with the specified arguments
        /// </summary>
        /// <param name="form">The form.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        DialogResult ShowDialog(Form form, DialogRunnerArgs args);
    }
}
