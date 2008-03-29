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

        readonly Dictionary<string, PendingCommitItem> _listItems = new Dictionary<string,PendingCommitItem>(StringComparer.OrdinalIgnoreCase);
        readonly SortedList<string, PendingChange> _pendingChanges = new SortedList<string, PendingChange>(StringComparer.OrdinalIgnoreCase);

        internal void RefreshList()
        {
            if (UISite == null)
                return;

            ICollection<string> allFiles = UISite.GetService<IProjectFileMapper>().GetAllFilesOfAllProjects();
            IFileStatusCache cache = UISite.GetService<IFileStatusCache>();            

            SortedList<string, SvnItem> items = new SortedList<string, SvnItem>(StringComparer.OrdinalIgnoreCase);

            foreach (string file in allFiles)
            {
                if (items.ContainsKey(file))
                    continue;

                SvnItem item = cache[file];

                if (item.IsModified || (!item.IsVersioned && !item.IsIgnored))
                    items.Add(item.FullPath, item);
            }

            foreach(SvnItem item in items.Values)
            {
                if(!_pendingChanges.ContainsKey(item.FullPath))
                    _pendingChanges.Add(item.FullPath, new PendingChange(UISite, item));
            }

            foreach(PendingChange pc in new List<PendingChange>(_pendingChanges.Values))
            {
                if(!items.ContainsKey(pc.FullPath))
                {
                    _pendingChanges.Remove(pc.FullPath);

                    PendingCommitItem item;

                    if(_listItems.TryGetValue(pc.FullPath, out item))
                    {
                        _listItems.Remove(pc.FullPath);
                        pendingCommits.Items.Remove(item);
                    }
                }
                else if(!_listItems.ContainsKey(pc.FullPath))
                {
                    PendingCommitItem lvi = new PendingCommitItem(pc);
                    _listItems.Add(pc.FullPath, lvi);
                    pendingCommits.Items.Add(lvi);
                }
            }
        }

        private void pendingCommits_ResolveItem(object sender, ListViewWithSelection.ResolveItemEventArgs e)
        {
            PendingChange pc = e.SelectionItem as PendingChange;

            PendingCommitItem pci;
            if (pc != null && this._listItems.TryGetValue(pc.FullPath, out pci))
            {
                e.Item = pci;
            }            
        }

        private void pendingCommits_RetrieveSelection(object sender, ListViewWithSelection.RetrieveSelectionEventArgs e)
        {
            PendingCommitItem pi = (PendingCommitItem)e.Item;

            e.SelectionItem = pi.PendingChange;            
        }
    }

    interface IPendingChangeManager
    {
    }
}
