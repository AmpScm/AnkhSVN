using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.VSSelectionControls;
using SharpSvn;
using Ankh.Scc;
using System.ComponentModel;

namespace Ankh.UI.SvnLog
{
    class LogChangedPathsView : ListViewWithSelection<PathListViewItem>
    {

        public LogChangedPathsView()
        {
        }

        public LogChangedPathsView(IContainer container)
        {
            container.Add(this);
        }

        protected override void OnRetrieveSelection(ListViewWithSelection<PathListViewItem>.RetrieveSelectionEventArgs e)
        {
            e.SelectionItem = new PathItem((PathListViewItem)e.Item);
            base.OnRetrieveSelection(e);
        }

        protected override void OnResolveItem(ListViewWithSelection<PathListViewItem>.ResolveItemEventArgs e)
        {
            e.Item = ((PathItem)e.SelectionItem).ListViewItem;
            base.OnResolveItem(e);
        }
    }

    sealed class PathListViewItem : ListViewItem
    {
        readonly SvnChangeItem _change;
        public PathListViewItem(SvnChangeItem change)
        {
            if (change == null)
                throw new ArgumentNullException("change");

            _change = change;
            RefreshText();
        }

        void RefreshText()
        {
            Text = _change.Path;
            SubItems.Add(new ListViewSubItem(this, _change.Action.ToString()));
            if (_change.CopyFromRevision != -1)
                SubItems.Add(new ListViewSubItem(this, "Copied"));
        }

        internal SvnChangeAction Action
        {
            get { return _change.Action; }
        }

        internal string Path
        {
            get { return _change.Path; }
        }

        internal string CopyFromPath
        {
            get { return _change.CopyFromPath; }
        }

        internal long CopyFromRevision
        {
            get { return _change.CopyFromRevision; }
        }
    }

    sealed class PathItem : ISvnLogChangedPathItem
    {
        readonly PathListViewItem _lvi;
        public PathItem(PathListViewItem lvi)
        {
            if (lvi == null)
                throw new ArgumentNullException("lvi");
            _lvi = lvi;
        }

        internal PathListViewItem ListViewItem
        {
            get { return _lvi; }
        }

        [Category("Subversion")]
        [DisplayName("Action")]
        public SvnChangeAction Action
        {
            get { return _lvi.Action; ; }
        }

        [Category("Subversion")]
        [DisplayName("Copy from path")]
        public string CopyFromPath
        {
            get { return _lvi.CopyFromPath; }
        }

        [Category("Subversion")]
        [DisplayName("Copy from revision")]
        public long CopyFromRevision
        {
            get { return _lvi.CopyFromRevision; }
        }

        [Category("Subversion")]
        [DisplayName("Path")]
        public string Path
        {
            get { return _lvi.Path; }
        }
    }
}
