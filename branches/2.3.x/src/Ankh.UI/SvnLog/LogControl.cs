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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using SharpSvn;
using System.Diagnostics;
using Ankh.Scc;

namespace Ankh.UI.SvnLog
{
    sealed partial class LogControl : UserControl, ICurrentItemSource<ISvnLogItem>, ISupportsVSTheming
    {
        public LogControl()
            : this(null)
        {
        }

        public LogControl(IContainer container)
        {
            if (container != null)
                container.Add(this);

            InitializeComponent();
            revisionBox.BatchDone += logRevisionControl1_BatchDone;

            LogSource = new LogDataSource();
            LogSource.Synchronizer = this;

            changedPathBox.LogSource = LogSource;
            changedPathBox.ItemSource = revisionBox;
            revisionBox.LogSource = LogSource;
            revisionBox.FocusChanged += OnFocusChanged;
            revisionBox.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(revisionBox_ItemSelectionChanged);
        }

        LogDataSource _dataSource;
        public LogDataSource LogSource
        {
            get { return _dataSource; }
            private set { _dataSource = value; }
        }

        void logRevisionControl1_BatchDone(object sender, BatchFinishedEventArgs e)
        {
            if (BatchFinished != null)
                BatchFinished(sender, e);
        }

        IAnkhServiceProvider _context;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set
            {
                _context = value;

                revisionBox.Context = value;
                changedPathBox.Context = value;
            }
        }

        LogMode _mode;
        //[Obsolete]
        public LogMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        void Reset()
        {
            revisionBox.Reset();
            changedPathBox.Items.Clear();
            logBox.Text = "";
        }

        public void StartLog(ICollection<SvnOrigin> targets, SvnRevision start, SvnRevision end)
        {
            if (targets == null)
                throw new ArgumentNullException("targets");

            LogSource.Targets = targets;
            LogSource.Start = start;
            LogSource.End = end;

            Reset();
            revisionBox.Start(LogMode.Log);
        }

        /// <summary>
        /// Starts the merges eligible logger. Checking whick revisions of source (Commonly Uri) 
        /// are eligeable for mergeing to target (Commonly path)
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        public void StartMergesEligible(IAnkhServiceProvider context, SvnOrigin target, SvnTarget source)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (target == null)
                throw new ArgumentNullException("target");
            if (source == null)
                throw new ArgumentNullException("source");

            LogSource.Targets = new SvnOrigin[] { new SvnOrigin(context, source, target.RepositoryRoot) }; // Must be from the same repository!
            LogSource.MergeTarget = target;
            Reset();
            revisionBox.Start(LogMode.MergesEligible);
        }

        public void StartMergesMerged(IAnkhServiceProvider context, SvnOrigin target, SvnTarget source)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (target == null)
                throw new ArgumentNullException("target");
            if (source == null)
                throw new ArgumentNullException("source");

            LogSource.Targets = new SvnOrigin[] { new SvnOrigin(context, source, target.RepositoryRoot) }; // Must be from the same repository!
            LogSource.MergeTarget = target;
            Reset();
            revisionBox.Start(LogMode.MergesMerged);
        }

        internal void FetchAll()
        {
            revisionBox.FetchAll();
        }

        public void Restart()
        {
            Reset();
            revisionBox.Start(Mode);
        }

        [DefaultValue(false)]
        public bool IncludeMergedRevisions
        {
            get { return LogSource.IncludeMergedRevisions; }
            set { LogSource.IncludeMergedRevisions = value; }
        }

        [DefaultValue(false)]
        public bool StopOnCopy
        {
            get { return LogSource.StopOnCopy; }
            set { LogSource.StopOnCopy = value; }
        }

        bool _logMessageHidden;
        [DefaultValue(true)]
        public bool ShowLogMessage
        {
            get { return !_logMessageHidden; }
            set
            {
                _logMessageHidden = !value;
                UpdateSplitPanels();
            }
        }
        bool _changedPathsHidden;
        [DefaultValue(true)]
        public bool ShowChangedPaths
        {
            get { return !_changedPathsHidden; }
            set
            {
                _changedPathsHidden = !value;
                UpdateSplitPanels();
            }
        }

        void UpdateSplitPanels()
        {
            splitContainer1.Panel2Collapsed = !ShowChangedPaths && !ShowLogMessage;
            splitContainer2.Panel1Collapsed = !ShowChangedPaths;
            splitContainer2.Panel2Collapsed = !ShowLogMessage;
        }

        public event EventHandler<BatchFinishedEventArgs> BatchFinished;

        public event EventHandler FocusChanged;

        public ISvnLogItem FocusedItem
        {
            get { return revisionBox.FocusedItem as ISvnLogItem; }
        }

        public IList<ISvnLogItem> SelectedItems
        {
            get { return revisionBox.SelectedLogItems; }
        }

        void OnFocusChanged(object sender, EventArgs e)
        {
            if (FocusChanged != null)
                FocusChanged(sender, e);

            string text = "";

            LogRevisionItem lri = revisionBox.FocusedItem as LogRevisionItem;
            if (lri != null)
                text = lri.LogMessage;

            logBox.Text = text;
        }

        void revisionBox_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            OnSelectionChanged(e);
        }

        public event EventHandler SelectionChanged;
        void OnSelectionChanged(EventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(this, e);
        }

        public void OnThemeChange(IAnkhServiceProvider sender, CancelEventArgs e)
        {
            if (VSVersion.VS11OrLater)
            {
                // Remove the chrome
                revisionBox.BorderStyle = BorderStyle.None;
                changedPathBox.BorderStyle = BorderStyle.None;
                logBox.BorderStyle = BorderStyle.None;

                changedPathLabel.Enabled = changedPathLabel.Visible = false;
                logLabel.Enabled = logLabel.Visible = false;
            }
        }
    }
}
