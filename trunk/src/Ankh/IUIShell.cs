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
	}
}
