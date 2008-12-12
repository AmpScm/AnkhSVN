// $Id$
//
// Copyright 2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

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

        LogDataSource _dataSource;
        public LogDataSource DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
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
        readonly SvnOrigin _origin;

        public PathListViewItem(LogChangedPathsView view, ISvnLogItem logItem, SvnChangeItem change, Uri reposRoot, bool isInSelection)
            : base(view)
        {
            if(logItem == null)
                throw new ArgumentNullException("logItem");
            if (change == null)
                throw new ArgumentNullException("change");
            _logItem = logItem;
            _change = change;
            _isInSelection = isInSelection;
            _origin = new SvnOrigin(new SvnUriTarget(SvnTools.AppendPathSuffix(reposRoot, change.Path.TrimStart('/')), logItem.Revision), reposRoot);

            RefreshText();
            UpdateColors();
        }

        public SvnOrigin Origin
        {
            get { return _origin; }
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
            if (SystemInformation.HighContrast)
                return;
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

    sealed class PathItem : AnkhPropertyGridItem, ISvnLogChangedPathItem
    {
        readonly PathListViewItem _lvi;
        public PathItem(PathListViewItem lvi)
        {
            if (lvi == null)
                throw new ArgumentNullException("lvi");
            _lvi = lvi;
        }

        [Browsable(false)]
        public SvnOrigin Origin
        {
            get { return _lvi.Origin; }
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

        [Category("Origin")]
        [DisplayName("Copied from path")]
        public string CopyFromPath
        {
            get { return _lvi.CopyFromPath; }
        }

        [Category("Origin")]
        [DisplayName("Copied from revision")]
        public long CopyFromRevision
        {
            get { return _lvi.CopyFromRevision; }
        }

        [DisplayName("Name")]
        public string Name
        {
            get { return _lvi.Origin.Target.FileName; }
        }
        
        [DisplayName("Path")]
        public string Path
        {
            get { return _lvi.Path; }
        }

        [Category("Subversion")]
        [DisplayName("Url")]
        public Uri Uri
        {
            get { return _lvi.Origin.Uri; }
        }

        [Category("Subversion")]
        [DisplayName("Last Revision")]
        public long Revision
        {
            get { return _lvi.LogItem.Revision; }
        }

        [Category("Subversion")]
        [DisplayName("Last Author")]
        public string Author
        {
            get { return _lvi.LogItem.Author; }
        }

        [Category("Subversion")]
        [DisplayName("Last Committed")]
        public DateTime LastCommitted
        {
            get { return _lvi.LogItem.CommitDate.ToLocalTime(); }
        }

        /// <summary>
        /// Gets the light/second name shown in the gridview header
        /// </summary>
        /// <value></value>
        protected override string ClassName
        {
            get { return "Changed Path"; }
        }

        /// <summary>
        /// Gets the bold/first name shown in the gridview header
        /// </summary>
        /// <value></value>
        protected override string ComponentName
        {
            get { return Origin.Target.FileName; }
        }
    }
}
