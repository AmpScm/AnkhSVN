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

namespace Ankh.UI.SvnLog
{
    class LogRevisionItem : SmartListViewItem
    {
        readonly IAnkhServiceProvider _context;
        readonly SvnLoggingEventArgs _args;
        public LogRevisionItem(LogRevisionView listView, IAnkhServiceProvider context, SvnLoggingEventArgs e)
            : base(listView)
        {
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
                Author,
                Date.ToString(CultureInfo.CurrentCulture),
                GetIssueText(),
                LogMessage);
        }

        private string GetIssueText()
        {
            StringBuilder sb = null;
            
            foreach(IssueMarker issue in _context.GetService<IProjectCommitSettings>().GetIssues(LogMessage))
            {
                if (sb == null)
                    sb = new StringBuilder();
                else
                    sb.Append(",");

                sb.Append(issue.Value);
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

        string _logMessage;
        internal string LogMessage
        {
            get
            {
                if (_logMessage == null && _args.LogMessage != null)
                {
                    _logMessage = _args.LogMessage.Trim().Replace("\r", "").Replace("\n", "\x23CE");
                }
                return _logMessage;
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
        public DateTime CommitDate
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
        public SvnChangeItemCollection ChangedPaths
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

        public System.Collections.Generic.IEnumerable<string> Issues
        {
            get 
            {
                string logMessage = LogMessage;

                if (string.IsNullOrEmpty(logMessage))
                    yield break;

                foreach (IssueMarker issue in _lvi.Context.GetService<IProjectCommitSettings>().GetIssues(logMessage))
                {
                    yield return issue.Value;
                }
            }
        }
    }
}
