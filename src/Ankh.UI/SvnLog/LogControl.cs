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
    sealed partial class LogControl : UserControl, ICurrentItemSource<ISvnLogItem>, ICurrentItemDestination<ISvnLogItem>
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
            ItemSource = revisionBox;
            revisionBox.BatchDone += logRevisionControl1_BatchDone;

            LogSource = new LogDataSource();
            LogSource.Synchronizer = this;

            changedPathBox.LogSource = LogSource;
            changedPathBox.ItemSource = revisionBox;
            revisionBox.LogSource = LogSource;
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

        public event EventHandler<CurrentItemEventArgs<ISvnLogItem>> SelectionChanged;

        public event EventHandler<CurrentItemEventArgs<ISvnLogItem>> FocusChanged;

        public ISvnLogItem FocusedItem
        {
            get { return ItemSource == null ? null : ItemSource.FocusedItem; }
        }

        public IList<ISvnLogItem> SelectedItems
        {
            get { return ItemSource == null ? null : ItemSource.SelectedItems; }
        }

        #region ICurrentItemDestination<ISvnLogItem> Members

        ICurrentItemSource<ISvnLogItem> _itemSource;
        public ICurrentItemSource<ISvnLogItem> ItemSource
        {
            [DebuggerStepThrough]
            get { return _itemSource; }
            set
            {
                _itemSource = value;
                value.FocusChanged += OnFocusChanged;
                value.SelectionChanged += OnSelectionChanged;
            }
        }

        void OnFocusChanged(object sender, CurrentItemEventArgs<ISvnLogItem> e)
        {
            if (FocusChanged != null)
                FocusChanged(sender, e);

            string text = "";
            if (ItemSource.FocusedItem != null)
                text = ItemSource.FocusedItem.LogMessage;

            logBox.Text = text;
        }

        void OnSelectionChanged(object sender, CurrentItemEventArgs<ISvnLogItem> e)
        {
            if (SelectionChanged != null)
                SelectionChanged(sender, e);
        }

        #endregion
    }
}
