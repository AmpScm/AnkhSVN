using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI;
using System.Diagnostics;
using Ankh.VS;
using System.Windows.Forms;
using Ankh.UI.RepositoryExplorer;

namespace Ankh.WorkingCopyExplorer
{
    public class ExplorersShell : AnkhService, IExplorersShell
    {
        readonly WorkingCopyExplorer _workingCopyExplorer;
        readonly RepositoryExplorer.Controller _repositoryController;

        RepositoryExplorerControl _reposControl;
        WorkingCopyExplorerControl _wcControl;

        public ExplorersShell(IAnkhServiceProvider context)
            : base(context)
        {
            this._repositoryController =
                new RepositoryExplorer.Controller(this);
            this._workingCopyExplorer =
                new WorkingCopyExplorer(this);
        }

        public RepositoryExplorerControl RepositoryExplorer
        {
            get
            {
                if (_reposControl == null)
                    this.CreateRepositoryExplorer();
                return _reposControl;
            }
            set
            {
                Debug.Assert(_reposControl == null);
                _reposControl = value;

                RepositoryExplorerService.SetControl(value);
            }
        }

        public WorkingCopyExplorerControl WorkingCopyExplorer
        {
            get
            {
                return _wcControl;
            }
            set 
            {
                Debug.Assert(_wcControl == null);
                _wcControl = value;

                if (value != null)
                    WorkingCopyExplorerService.SetControl(value);
            }
        }

        public System.ComponentModel.ISynchronizeInvoke SynchronizingObject
        {
            get
            {
                // TODO: Fix someway; probably just removing
                return this.RepositoryExplorer ?? null;
            }
        }

        #region IExplorersShell Members

        public void SetRepositoryExplorerSelection(object[] selection)
        {
            //this.repositoryExplorerWindow.SetSelectionContainer( ref selection );
        }

        public bool RepositoryExplorerHasFocus()
        {
            // The new command routing should make this method obsolete
            if (_reposControl != null)
                return _reposControl.ContainsFocus;
            else
                return false;
        }

        public bool WorkingCopyExplorerHasFocus()
        {
            // The new command routing should make this method obsolete
            if (_wcControl != null)
                return _wcControl.ContainsFocus;
            else
                return false;
        }

        [Obsolete]
        public bool SolutionExplorerHasFocus()
        {
            // The new command routing should make this method obsolete
            return false;// this.Context.DTE.ActiveWindow.Type == vsWindowType.vsWindowTypeSolutionExplorer;
        }

        public Uri ShowAddRepositoryRootDialog()
        {
            using (AddRepositoryRootDialog dlg = new AddRepositoryRootDialog())
            {
                if (dlg.ShowDialog(Context.GetService<IAnkhDialogOwner>().DialogOwner) != DialogResult.OK)
                    return null;

                return new Uri(dlg.Url);
            }
        }

        public string ShowAddWorkingCopyExplorerRootDialog()
        {
            using (AddWorkingCopyExplorerRootDialog dlg = new AddWorkingCopyExplorerRootDialog())
            {
                if (dlg.ShowDialog(Context.GetService<IAnkhDialogOwner>().DialogOwner) != DialogResult.OK)
                {
                    return null;
                }
                return dlg.NewRoot;
            }
        }

        #endregion

        #region IExplorersShell Members


        public IWorkingCopyExplorer WorkingCopyExplorerService
        {
            get { return _workingCopyExplorer; }
        }

        #endregion

        #region IExplorersShell Members


        public Ankh.RepositoryExplorer.Controller RepositoryExplorerService
        {
            get { return _repositoryController; }
        }

        #endregion

        private void CreateRepositoryExplorer()
        {
            // BH: Moved creating to the package to allow VS to manage all state associated with the window
            Debug.WriteLine("Previously precreated Repository Explorer here");
        }

        private void CreateCommitDialog()
        {
            Debug.WriteLine("Previously precreated Commit Window here");
        }

        private void CreateWorkingCopyExplorer()
        {
            // BH: Moved creating to the package to allow VS to manage all state associated with the window
            Debug.WriteLine("Previously precreated Working Copy Explorer here");
        }
    }
}
