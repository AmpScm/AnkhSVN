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
    partial class PendingCommitsPage : PendingChangesPage, IPendingChangeManager
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
        readonly SortedList<string, PendingChange> _pendingChanges = new SortedList<string, PendingChange>(StringComparer.OrdinalIgnoreCase);

        IFileIconMapper _iconMap;

        internal void RefreshList()
        {
            if (UISite == null)
                return;

            ICollection<string> allFiles = UISite.GetService<IProjectFileMapper>().GetAllFilesOfAllProjects();
            IFileStatusCache cache = UISite.GetService<IFileStatusCache>();
            IAnkhOpenDocumentTracker tracker = UISite.GetService<IAnkhOpenDocumentTracker>();

            SortedList<string, SvnItem> items = new SortedList<string, SvnItem>(StringComparer.OrdinalIgnoreCase);

            if (_iconMap == null)
            {
                _iconMap = UISite.GetService<IFileIconMapper>();

                pendingCommits.SmallImageList = _iconMap.ImageList;
            }

            if ((ModifierKeys & Keys.Control) != 0)
                cache.ClearCache(); // Rebuild all status!

            foreach (string file in allFiles)
            {
                if (items.ContainsKey(file))
                    continue;

                SvnItem item = cache[file];

                bool add = false;
                if (item.IsVersioned)
                {
                    add = item.IsModified || tracker.IsDocumentDirty(item.FullPath);
                }
                else if (!item.IsIgnored && item.Exists)
                    add = true;

                if (add)
                    items.Add(item.FullPath, item);
            }

            foreach (SvnItem item in items.Values)
            {
                if (!_pendingChanges.ContainsKey(item.FullPath))
                    _pendingChanges.Add(item.FullPath, new PendingChange(UISite, item));
            }

            foreach (PendingChange pc in new List<PendingChange>(_pendingChanges.Values))
            {
                if (!items.ContainsKey(pc.FullPath))
                {
                    _pendingChanges.Remove(pc.FullPath);

                    PendingCommitItem item;

                    if (_listItems.TryGetValue(pc.FullPath, out item))
                    {
                        _listItems.Remove(pc.FullPath);
                        pendingCommits.Items.Remove(item);
                    }
                }
                else if (!_listItems.ContainsKey(pc.FullPath))
                {
                    PendingCommitItem lvi = new PendingCommitItem(UISite, pc, _iconMap);
                    _listItems.Add(pc.FullPath, lvi);
                    pendingCommits.Items.Add(lvi);
                }
            }

            foreach (PendingCommitItem pci in _listItems.Values)
            {
                pci.RefreshText(UISite, _iconMap);
            }
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

            if(info != null && info.Location != ListViewHitTestLocations.None)
            {
                IAnkhCommandService cmd = UISite.GetService<IAnkhCommandService>();

                if (cmd != null)
                    cmd.ExecCommand(AnkhCommand.ItemOpenVisualStudio);
            }            
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

    interface IPendingChangeManager
    {
    }
}
