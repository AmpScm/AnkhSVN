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
        /// Shows the log dialog.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        LogDialogInfo ShowLogDialog( LogDialogInfo info );
	}
}
