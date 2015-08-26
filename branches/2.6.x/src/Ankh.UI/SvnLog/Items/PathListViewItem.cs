// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
using System.Windows.Forms;
using Ankh.UI.VSSelectionControls;
using SharpSvn;
using Ankh.Scc;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using Ankh.Commands;

namespace Ankh.UI.SvnLog
{
    sealed class PathListViewItem : SmartListViewItem
    {
        readonly ISvnLogItem _logItem;
        readonly SvnChangeItem _change;
        readonly bool _isInSelection;
        readonly SvnOrigin _origin;

        public PathListViewItem(LogChangedPathsView view, ISvnLogItem logItem, SvnChangeItem change, Uri reposRoot, bool isInSelection, Color[] colorInfo)
            : base(view)
        {
            if (logItem == null)
                throw new ArgumentNullException("logItem");
            if (change == null)
                throw new ArgumentNullException("change");
            _logItem = logItem;
            _change = change;
            _isInSelection = isInSelection;
            Uri uri;

            string path = change.Path.TrimStart('/');

            if (string.IsNullOrEmpty(path))
                uri = reposRoot;
            else
                uri = SvnTools.AppendPathSuffix(reposRoot, path);

            _origin = new SvnOrigin(new SvnUriTarget(uri, logItem.Revision), reposRoot);

            RefreshText();
            UpdateColors(colorInfo);
        }

        public SvnOrigin Origin
        {
            get { return _origin; }
        }

        void RefreshText()
        {
            SetValues(
                ChangeText(),
                NodeKind == SvnNodeKind.Directory ? EnsureEndSlash(_change.Path) : _change.Path,
                _change.CopyFromPath ?? "",
                _change.CopyFromPath != null ? _change.CopyFromRevision.ToString() : ""
            );
        }

        private string ChangeText()
        {
            switch (_change.Action)
            {
                case SvnChangeAction.Add:
                    return LogStrings.Added;
                case SvnChangeAction.Replace:
                    return LogStrings.Replaced;
                case SvnChangeAction.Delete:
                    return LogStrings.Deleted;
                case SvnChangeAction.Modify:
                    if (!_change.PropertiesModified.HasValue)
                        return LogStrings.Modified;
                    else if (!_change.PropertiesModified.Value)
                        return LogStrings.ModifiedTextOnly;
                    else if (_change.PropertiesModified.Value && !_change.ContentModified.HasValue || !_change.ContentModified.Value)
                        return LogStrings.PropertiesModified;
                    else
                        return LogStrings.Modified;
                default:
                    return _change.Action.ToString();
            }
        }

        private string EnsureEndSlash(string p)
        {
            if (!p.EndsWith("/", StringComparison.Ordinal))
                return p + "/";

            return p;
        }

        void UpdateColors(Color[] colorInfo)
        {
            if (colorInfo == null || colorInfo.Length < 4)
                return;

            if (!_isInSelection)
                ForeColor = colorInfo[0];
            else
            {
                switch (_change.Action)
                {
                    case SvnChangeAction.Add:
                        ForeColor = colorInfo[1];
                        break;
                    case SvnChangeAction.Delete:
                        ForeColor = colorInfo[2];
                        break;
                    case SvnChangeAction.Modify:
                        ForeColor = colorInfo[3];
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
            get { return _logItem; }
        }

        internal SvnNodeKind NodeKind
        {
            get { return _change.NodeKind; }
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
            get { return _lvi.Action; }
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
        [Description("Revision number of the Last Commit")]
        public long Revision
        {
            get { return _lvi.LogItem.Revision; }
        }

        [Category("Subversion")]
        [DisplayName("Last Author")]
        [Description("Author of the Last Commit")]
        public string Author
        {
            get { return _lvi.LogItem.Author; }
        }

        [Category("Subversion")]
        [DisplayName("Last Committed")]
        [Description("Time of the Last Commit")]
        public DateTime LastCommitted
        {
            get { return _lvi.LogItem.Time.ToLocalTime(); }
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

        SvnRevision ISvnRepositoryItem.Revision
        {
            get { return Revision; }
        }

        SvnNodeKind ISvnRepositoryItem.NodeKind
        {
            get { return _lvi.NodeKind; }
        }

        SvnOrigin ISvnRepositoryItem.Origin
        {
            // We don't have a repository item when we are deleted!
            get { return (Action != SvnChangeAction.Delete) ? Origin : null; }
        }

        void ISvnRepositoryItem.RefreshItem(bool refreshParent)
        {
        }
    }
}
