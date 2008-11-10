// $Id$
using System;
using Ankh.UI;
using System.Windows.Forms;
using Ankh.Scc;

namespace Ankh
{
    /// <summary>
    /// Represents the UI of the addin.
    /// </summary>
    public interface IUIShell
    {
        /// <summary>
        /// Displays HTML in some suitable view.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="html"></param>
        void DisplayHtml(string caption, string html, bool reuse);

        /// <summary>
        /// Show a "path selector dialog".
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        PathSelectorResult ShowPathSelector(PathSelectorInfo info);

        /// <summary>
        /// Edits the state of the enlistment.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        bool EditEnlistmentState(EnlistmentState state);

        /// <summary>
        /// Shows the dialog for adding a new root to the repository.
        /// </summary>
        /// <returns></returns>
        Uri ShowAddRepositoryRootDialog();

        /// <summary>
        /// Shows the add working copy explorer root dialog.
        /// </summary>
        /// <returns></returns>
        string ShowAddWorkingCopyExplorerRootDialog();
    }
}
