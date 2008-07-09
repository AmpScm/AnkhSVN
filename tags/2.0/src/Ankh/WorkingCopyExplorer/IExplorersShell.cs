using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI;
using Ankh.UI.RepositoryExplorer;

namespace Ankh.WorkingCopyExplorer
{
    public interface IExplorersShell
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

        IWorkingCopyExplorer WorkingCopyExplorerService
        {
            get;
        }

        Ankh.UI.WorkingCopyExplorerControl WorkingCopyExplorer
        {
            get;
            set;
        }

        /// <summary>
        /// Whether the Repository Explorer window has focus
        /// </summary>
        /// <returns></returns>
        bool WorkingCopyExplorerHasFocus();

        /// <summary>
        /// Shows the dialog for adding a new root to the repository.
        /// </summary>
        /// <returns></returns>
        Uri ShowAddRepositoryRootDialog();

        string ShowAddWorkingCopyExplorerRootDialog();

        void AddRepositoryRoot(Uri info);
    }
}
