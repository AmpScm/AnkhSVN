using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI;
using System.Diagnostics;
using Ankh.VS;
using System.Windows.Forms;
using Ankh.UI.RepositoryExplorer;
using Ankh.UI.WorkingCopyExplorer;

namespace Ankh.WorkingCopyExplorer
{
    public class ExplorersShell : AnkhService, IExplorersShell
    {
        readonly WorkingCopyExplorer _workingCopyExplorer;

        RepositoryExplorerControl _reposControl;
        WorkingCopyExplorerControl _wcControl;

        public ExplorersShell(IAnkhServiceProvider context)
            : base(context)
        {
            _workingCopyExplorer = new WorkingCopyExplorer(this);
        }

        public RepositoryExplorerControl RepositoryExplorer
        {
            get { return _reposControl; }
            set
            {
                Debug.Assert(_reposControl == null);
                _reposControl = value;
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

        public bool WorkingCopyExplorerHasFocus()
        {
            // The new command routing should make this method obsolete
            if (_wcControl != null)
                return _wcControl.ContainsFocus;
            else
                return false;
        }
        
        public Uri ShowAddRepositoryRootDialog()
        {
            using (AddRepositoryRootDialog dlg = new AddRepositoryRootDialog())
            {
                if (dlg.ShowDialog(Context.GetService<IAnkhDialogOwner>().DialogOwner) != DialogResult.OK)
                    return null;

                return dlg.Uri;
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

        private void CreateWorkingCopyExplorer()
        {
            // BH: Moved creating to the package to allow VS to manage all state associated with the window
            Debug.WriteLine("Previously precreated Working Copy Explorer here");
        }

        #region IExplorersShell Members


        public void AddRepositoryRoot(Uri info)
        {
            RepositoryExplorer.AddRoot(info);
        }

        #endregion
    }
}
