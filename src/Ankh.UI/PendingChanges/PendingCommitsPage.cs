using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using System.ComponentModel.Design;
using Ankh.Scc;
using AnkhSvn.Ids;
using Ankh.Commands;
using Ankh.VS;

namespace Ankh.UI.PendingChanges
{
    partial class PendingCommitsPage : PendingChangesPage
    {
        public PendingCommitsPage()
        {
            InitializeComponent();
        }

        bool _createdEditor;
        protected override void OnUISiteChanged()
        {
            base.OnUISiteChanged();

            if (!_createdEditor)
            {
                //UISite.
                IOleServiceProvider sp = UISite.GetService<IOleServiceProvider>();

                if (sp != null)
                {
                    logMessageEditor.Init(sp);
                    UISite.CommandTarget = logMessageEditor.CommandTarget;
                    _createdEditor = true;
                }
            }

            if (pendingCommits != null)
                pendingCommits.ServiceProvider = UISite;

            HookList();
        }

        IPendingChangesManager _manager;
        private void HookList()
        {
            if(_manager != null || UISite == null)
                return;

            _manager = UISite.GetService<IPendingChangesManager>();

            if (_manager == null)
                return;

            _manager.Added += new EventHandler<PendingChangeEventArgs>(OnPendingChangeAdded);
            _manager.Removed += new EventHandler<PendingChangeEventArgs>(OnPendingChangeRemoved);
            _manager.Changed += new EventHandler<PendingChangeEventArgs>(OnPendingChangesChanged);
            _manager.InitialUpdate += new EventHandler<PendingChangeEventArgs>(OnPendingChangesInitialUpdate);
            _manager.IsActiveChanged += new EventHandler<PendingChangeEventArgs>(OnPendingChangesActiveChanged);
            _manager.ListFlushed += new EventHandler<PendingChangeEventArgs>(OnPendingChangesListFlushed);

            if (!_manager.IsActive)
            {
                _manager.IsActive = true;
                _manager.FullRefresh(false);
            }
        }

        protected IPendingChangesManager Manager
        {
            get
            {
                if (_manager == null)
                    HookList();

                return _manager;
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            _createdEditor = false;
        }

        protected override Type PageType
        {
            get
            {
                return typeof(PendingCommitsPage);
            }
        }

        public bool LogMessageVisible
        {
            get { return !splitContainer.Panel1Collapsed; }
            set { splitContainer.Panel1Collapsed = !value; }
        }

        readonly Dictionary<string, PendingCommitItem> _listItems = new Dictionary<string, PendingCommitItem>(StringComparer.OrdinalIgnoreCase);

        void OnPendingChangeAdded(object sender, PendingChangeEventArgs e)
        {
            PendingCommitItem pci;

            string path = e.Change.FullPath;

            if (_listItems.TryGetValue(path, out pci))
            {
                // Should never happend; will refresh checkbox, etc.
                _listItems.Remove(path);
                pendingCommits.Items.Remove(pci);
            }

            pci = new PendingCommitItem(UISite, e.Change);
            _listItems.Add(path, pci);
            pendingCommits.Items.Add(pci);
        }

        HybridCollection<string> _checkedItems;
        void OnPendingChangesListFlushed(object sender, PendingChangeEventArgs e)
        {
            if (_listItems.Count > 0)
            {
                _checkedItems = new HybridCollection<string>();
                foreach (PendingCommitItem pci in _listItems.Values)
                {
                    if (pci.Checked && !_checkedItems.Contains(pci.FullPath))
                        _checkedItems.Add(pci.FullPath);
                }
                _listItems.Clear();
                pendingCommits.Items.Clear();
            }            
        }

        void OnPendingChangesActiveChanged(object sender, PendingChangeEventArgs e)
        {
            // Just ignore for now
            Enabled = e.Manager.IsActive;
        }

        void OnPendingChangesInitialUpdate(object sender, PendingChangeEventArgs e)
        {
            _listItems.Clear(); // Make sure we are clear
            pendingCommits.Items.Clear();
            pendingCommits.BeginUpdate();
            try
            {
                foreach (PendingChange pc in e.Manager.GetAll())
                {
                    PendingCommitItem pi = new PendingCommitItem(UISite, pc);
                    _listItems.Add(pc.FullPath, pi);

                    if (_checkedItems != null)
                        pi.Checked = _checkedItems.Contains(pc.FullPath);

                    pendingCommits.Items.Add(pi);
                }

                _checkedItems = null;
            }
            finally
            {
                pendingCommits.EndUpdate();
                if (_listItems.Count > 0)
                    pendingCommits.RedrawItems(0, _listItems.Count - 1, true);
            }
        }

        void OnPendingChangesChanged(object sender, PendingChangeEventArgs e)
        {
            PendingCommitItem pci;

            string path = e.Change.FullPath;

            if (!_listItems.TryGetValue(path, out pci))
            {
                pci = new PendingCommitItem(UISite, e.Change);
                _listItems.Add(path, pci);
                pendingCommits.Items.Add(pci);
            }
            else
                pci.RefreshText(UISite);
        }

        void OnPendingChangeRemoved(object sender, PendingChangeEventArgs e)
        {
            PendingCommitItem pci;

            string path = e.Change.FullPath;

            if (_listItems.TryGetValue(path, out pci))
            {
                _listItems.Remove(path);
                pendingCommits.Items.Remove(pci);
            }
        }

        IFileIconMapper _iconMap;

        internal void RefreshList(bool incrementalUpdate)
        {
            Manager.FullRefresh(!incrementalUpdate);
        }

        private void pendingCommits_ResolveItem(object sender, PendingCommitsView.ResolveItemEventArgs e)
        {
            PendingChange pc = e.SelectionItem as PendingChange;

            PendingCommitItem pci;
            if (pc != null && this._listItems.TryGetValue(pc.FullPath, out pci))
            {
                e.Item = pci;
            }
        }

        private void pendingCommits_RetrieveSelection(object sender, PendingCommitsView.RetrieveSelectionEventArgs e)
        {
            PendingCommitItem pi = (PendingCommitItem)e.Item;

            e.SelectionItem = pi.PendingChange;
        }

        private void pendingCommits_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = pendingCommits.HitTest(e.X, e.Y);

            if (info == null || info.Location == ListViewHitTestLocations.None)
                return;

            if (info.Location == ListViewHitTestLocations.StateImage)
                return; // Just check the item

            pendingCommits.RedrawItems(info.Item.Index, info.Item.Index, false);

            IAnkhCommandService cmd = UISite.GetService<IAnkhCommandService>();

            if (cmd != null)
                cmd.ExecCommand(AnkhCommand.ItemOpenVisualStudio);
        }

        private void pendingCommits_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {

        }

        private void pendingCommits_ShowContextMenu(object sender, EventArgs e)
        {
            Point p = MousePosition;
            UISite.ShowContextMenu(AnkhCommandMenu.PendingChangesContextMenu, p.X, p.Y); 
        }
    }
}
