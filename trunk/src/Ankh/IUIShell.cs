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
        }

        /// <summary>
        /// This object is used to synchronize notification callbacks, among other things.
        /// </summary>
        System.ComponentModel.ISynchronizeInvoke SynchronizingObject
        {
            get;
        }

        /// <summary>
        /// An IContext.
        /// </summary>
        IContext Context
        {
            get;
            set;
        }

        /// <summary>
        /// Ask the user whether Ankh should load for a given solution.
        /// </summary>
        /// <returns></returns>
        DialogResult QueryWhetherAnkhShouldLoad();

        /// <summary>
        /// Whether to show the repository explorer tool window.
        /// </summary>
        /// <param name="show"></param>
        void ShowRepositoryExplorer(bool show);

        /// <summary>
        /// Set the selection for the repository explorer.
        /// </summary>
        /// <param name="selection"></param>
        void SetRepositoryExplorerSelection( object[] selection );

        /// <summary>
        /// Displays the commit dialog modally.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        CommitContext ShowCommitDialogModal( CommitContext ctx );


        /// <summary>
        /// Shows the "New directory" dialog.
        /// </summary>
        /// <returns>The name of the new dialog, or null.</returns>
        string ShowNewDirectoryDialog();

        /// <summary>
        /// Shows/hides the commit dialog
        /// </summary>
        void ToggleCommitDialog( bool show );

        /// <summary>
        /// Resets the commit dialog.
        /// </summary>
        void ResetCommitDialog();


        /// <summary>
        /// Whether the Repository Explorer window has focus
        /// </summary>
        /// <returns></returns>
        bool RepositoryExplorerHasFocus();

        /// <summary>
        /// Executes the worker.Work method while displaying a progress dialog.
        /// </summary>
        /// <param name="worker"></param>
        /// <returns>True if the operation completed successfully without user cancellation.</returns>
        bool RunWithProgressDialog( IProgressWorker worker, string caption );

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
        PathSelectorInfo ShowPathSelector( PathSelectorInfo info );

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
	}
}
