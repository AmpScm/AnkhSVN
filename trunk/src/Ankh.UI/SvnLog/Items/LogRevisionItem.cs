// $Id$
//
// Copyright 2009 The AnkhSVN Project
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
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using SharpSvn;
using Ankh.UI.VSSelectionControls;
using System.Globalization;
using System.Drawing;
using Ankh.Scc;
using SharpSvn.Implementation;
using System.Collections.ObjectModel;
using Ankh.VS;
using System.Collections.Generic;

namespace Ankh.UI.SvnLog
{
    class LogRevisionItem : SmartListViewItem
    {
        readonly IAnkhServiceProvider _context;
        readonly SvnLoggingEventArgs _args;
        public LogRevisionItem(LogRevisionControl listView, IAnkhServiceProvider context, SvnLoggingEventArgs e)
            : base(listView)
        {
            if (listView == null)
                throw new ArgumentNullException("listView");
            else if (context == null)
                throw new ArgumentNullException("listView");
            else if (e == null)
                throw new ArgumentNullException("e");

            _args = e;
            _context = context;
            RefreshText();
            UpdateColors();
        }

        [Browsable(false)]
        internal string RevisionText
        {
            get { return Revision.ToString(CultureInfo.CurrentCulture); }
        }

        void RefreshText()
        {
            SetValues(
                "", // First column must be "" to work around owner draw issues!
                RevisionText,
                Author,
                Date.ToString(CultureInfo.CurrentCulture),
                GetIssueText(),
                GetLogMessageLine());
        }

        private string GetIssueText()
        {
            StringBuilder sb = null;
            ICollection<string> issueList = new List<string>();
            foreach (TextMarker issue in Issues)
            {
                if (!issueList.Contains(issue.Value))
                {
                    if (sb == null)
                        sb = new StringBuilder();
                    else
                        sb.Append(',');

                    sb.Append(issue.Value);
                    issueList.Add(issue.Value);
                }
            }
            return sb != null ? sb.ToString() : "";
        }

        void UpdateColors()
        {
            if (SystemInformation.HighContrast)
                return;

            if (_args.ChangedPaths == null)
                return;

            foreach (SvnChangeItem ci in _args.ChangedPaths)
            {
                if (ci.CopyFromRevision >= 0)
                    ForeColor = Color.DarkBlue;
            }
        }

        internal DateTime Date
        {
            get { return _args.Time.ToLocalTime(); }
        }

        internal string Author
        {
            get { return _args.Author; }
        }

        internal string GetLogMessageLine()
        {
            return LogMessage.Trim().Replace("\r", "").Replace("\n", "\x23CE");
        }

        internal string LogMessage
        {
            get
            {
                return _args.LogMessage ?? "";
            }
        }

        internal long Revision
        {
            get { return _args.Revision; }
        }

        internal KeyedCollection<string, SvnChangeItem> ChangedPaths
        {
            get { return _args.ChangedPaths; }
        }

        internal SvnLoggingEventArgs RawData
        {
            get { return _args; }
        }

        internal IAnkhServiceProvider Context
        {
            get { return _context; }
        }

        /// <summary>
        /// Returns IEnumerable for issue ids combining the issues found via associated issue repository and project commit settings.
        /// </summary>
        internal IEnumerable<TextMarker> Issues
        {
            get
            {
                string logMessage = LogMessage;

                if (string.IsNullOrEmpty(logMessage))
                    yield break;

                IAnkhIssueService iService = Context.GetService<IAnkhIssueService>();
                if (iService != null)
                {
                    IEnumerable<TextMarker> issues;
                    if (iService.TryGetIssues(logMessage, out issues))
                    {
                        foreach (TextMarker issue in issues)
                        {
                            yield return issue;
                        }
                    }
                }
                else
                {
                    yield break;
                }
            }
        }
    }

    sealed class LogItem : AnkhPropertyGridItem, ISvnLogItem
    {
        readonly LogRevisionItem _lvi;
        public Uri _repositoryRoot;

        public LogItem(LogRevisionItem lvi, Uri repositoryRoot)
        {
            if (lvi == null)
                throw new ArgumentNullException("lvi");

            _lvi = lvi;
            _index = lvi.Index;
            _repositoryRoot = repositoryRoot;
        }

        internal LogRevisionItem ListViewItem
        {
            get { return _lvi; }
        }

        /// <summary>
        /// Gets the repository root.
        /// </summary>
        /// <value>The repository root.</value>
        [Browsable(false)]
        public Uri RepositoryRoot
        {
            get { return _repositoryRoot; }
        }

        readonly int _index;
        [Browsable(false)]
        public int Index
        {
            get 
            {
                return _index; 
            }
        }

        [Category("Subversion")]
        [DisplayName("Commit date")]
        public DateTime Time
        {
            get { return _lvi.Date.ToLocalTime(); }
        }

        [Category("Subversion")]
        public string Author
        {
            get { return _lvi.Author; }
        }

        [Category("Subversion")]
        [DisplayName("Log message")]
        public string LogMessage
        {
            get { return _lvi.RawData.LogMessage; }
        }

        [Category("Subversion")]
        public long Revision
        {
            get { return _lvi.Revision; }
        }

        [Browsable(false)]
        public KeyedCollection<string, SvnChangeItem> ChangedPaths
        {
            get { return _lvi.RawData.ChangedPaths; }
        }

        /// <summary>
        /// Gets the light/second name shown in the gridview header
        /// </summary>
        /// <value></value>
        protected override string ClassName
        {
            get { return "Revision Information"; }
        }

        /// <summary>
        /// Gets the bold/first name shown in the gridview header
        /// </summary>
        /// <value></value>
        protected override string ComponentName
        {
            get { return string.Format("r{0}", Revision); }
        }

        /// <summary>
        /// Returns IEnumerable combining the issues found via associated issue repository and project commit settings.
        /// </summary>
        public System.Collections.Generic.IEnumerable<TextMarker> Issues
        {
            get 
            {
                return _lvi.Issues;
            }
        }
    }
}
