using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.VSSelectionControls;
using SharpSvn;
using Ankh.Scc;
using System.ComponentModel;
using System.Drawing;

namespace Ankh.UI.SvnLog
{
    class LogChangedPathsView : ListViewWithSelection<PathListViewItem>
    {

        public LogChangedPathsView()
        {
            Init();
        }

        public LogChangedPathsView(IContainer container)
            : this()
        {

            container.Add(this);
        }

        void Init()
        {
            SmartColumn action = new SmartColumn(this, "&Action", 60);
            SmartColumn path = new SmartColumn(this, "&Path", 342);
            SmartColumn copy = new SmartColumn(this, "&Copy", 60);
            SmartColumn copyRev = new SmartColumn(this, "Copy &Revision", 60);

            AllColumns.Add(action);
            AllColumns.Add(path);
            AllColumns.Add(copy);
            AllColumns.Add(copyRev);

            Columns.AddRange(
                new ColumnHeader[]
                {
                    action,
                    path,
                    copy,
                    copyRev
                });

            SortColumns.Add(path);
            FinalSortColumn = path;            
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

    sealed class PathListViewItem : SmartListViewItem
    {
        readonly ISvnLogItem _logItem;
        readonly SvnChangeItem _change;
        readonly bool _isInSelection;

        public PathListViewItem(LogChangedPathsView view, ISvnLogItem logItem, SvnChangeItem change, bool isInSelection)
            : base(view)
        {
            if(logItem == null)
                throw new ArgumentNullException("logItem");
            if (change == null)
                throw new ArgumentNullException("change");
            _logItem = logItem;
            _change = change;
            _isInSelection = isInSelection;

            RefreshText();
            UpdateColors();
        }

        void RefreshText()
        {
            SetValues(
                _change.Action.ToString(),
                _change.Path,
                _change.CopyFromPath ?? "",
                _change.CopyFromPath != null ? _change.CopyFromRevision.ToString() : ""
            );

            
        }

        void UpdateColors()
        {
            if (!_isInSelection)
                ForeColor = Color.Gray;
            else
            {
                switch (_change.Action)
                {
                    case SvnChangeAction.Add:
                        ForeColor = Color.FromArgb(100, 0, 100);
                        break;
                    case SvnChangeAction.Delete:
                        ForeColor = Color.DarkRed;
                        break;
                    case SvnChangeAction.Modify:
                        ForeColor = Color.DarkBlue;
                        break;
                }
            }
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

        internal ISvnLogItem LogItem
        {
            get { return _logItem;}
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
     
        [Browsable(false)]
        public long Revision
        {
            get { return _lvi.LogItem.Revision; }
        }
    }
}
