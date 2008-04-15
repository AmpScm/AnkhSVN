// $Id$
using System;
using Ankh.UI;
using System.Windows.Forms;

namespace Ankh
{
	/// <summary>
	/// Represents the UI of the addin.
	/// </summary>
	public interface IUIShell
	{
       
        /// <summary>
        /// The repository explorer UI.
        /// </summary>
        RepositoryExplorerControl RepositoryExplorer
        {
            get;
            set;
        }

        /// <summary>
        /// This object is used to synchronize notification callbacks, among other things.
        /// </summary>
        System.ComponentModel.ISynchronizeInvoke SynchronizingObject
        {
            get;
        }

        Ankh.UI.WorkingCopyExplorerControl WorkingCopyExplorer
        {
            get;
            set;
        }

        /// <summary>
        /// Set the selection for the repository explorer.
        /// </summary>
        /// <param name="selection"></param>
        void SetRepositoryExplorerSelection( object[] selection );

        /// <summary>
        /// Whether the Repository Explorer window has focus
        /// </summary>
        /// <returns></returns>
        bool RepositoryExplorerHasFocus();

        /// <summary>
        /// Whether the Repository Explorer window has focus
        /// </summary>
        /// <returns></returns>
        bool WorkingCopyExplorerHasFocus();

        /// <summary>
        /// Whether the Repository Explorer window has focus
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        bool SolutionExplorerHasFocus();


        /// <summary>
        /// Display a message box.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="text"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        DialogResult ShowMessageBox( string text, string caption, 
            MessageBoxButtons buttons );

        /// <summary>
        /// Display a message box.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="text"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        DialogResult ShowMessageBox( string text, string caption, 
            MessageBoxButtons buttons, MessageBoxIcon icon );

        /// <summary>
        /// Display a message box.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="text"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        DialogResult ShowMessageBox( string text, string caption,
            MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton );

        /// <summary>
        /// Displays HTML in some suitable view.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="html"></param>
        void DisplayHtml( string caption, string html, bool reuse );

        /// <summary>
        /// Show a "path selector dialog".
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        PathSelectorResult ShowPathSelector( PathSelectorInfo info );

        /// <summary>
        /// Shows the lock dialog.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        LockDialogInfo ShowLockDialog( LockDialogInfo info );

        /// <summary>
        /// Shows the log dialog.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        LogDialogInfo ShowLogDialog( LogDialogInfo info );


        /// <summary>
        /// Shows the switch dialog.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        SwitchDialogInfo ShowSwitchDialog( SwitchDialogInfo info );

        /// <summary>
        /// Shows the dialog for adding a new root to the repository.
        /// </summary>
        /// <returns></returns>
        RepositoryRootInfo ShowAddRepositoryRootDialog();

        string ShowAddWorkingCopyExplorerRootDialog();
    }
}
