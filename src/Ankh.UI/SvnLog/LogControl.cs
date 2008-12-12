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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.Services;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.Selection;
using SharpSvn;
using System.Diagnostics;
using Ankh.Scc;

namespace Ankh.UI.SvnLog
{
    partial class LogControl : UserControl, ICurrentItemSource<ISvnLogItem>, ICurrentItemDestination<ISvnLogItem>
    {
        public LogControl()
        {
            InitializeComponent();
            ItemSource = logRevisionControl1;
            logRevisionControl1.BatchDone += new EventHandler<BatchFinishedEventArgs>(logRevisionControl1_BatchDone);

            LogSource = new LogDataSource();
            LogSource.Synchronizer = this;

            logChangedPaths1.LogSource = LogSource;
            logRevisionControl1.LogSource = LogSource;
        }

        LogDataSource _dataSource;
        public LogDataSource LogSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        void logRevisionControl1_BatchDone(object sender, BatchFinishedEventArgs e)
        {
            if (BatchFinished != null)
                BatchFinished(sender, e);
        }


        public LogControl(IContainer container)
            :this()
        {
            container.Add(this);
        }

		IAnkhServiceProvider _context;
		public IAnkhServiceProvider Context
		{
			get { return _context; }
			set
			{
				_context = value;

				logRevisionControl1.Context = value;
                logChangedPaths1.Context = value;
			}
		}

        LogMode _mode;
        //[Obsolete]
        public LogMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public void StartLog(IAnkhServiceProvider context, ICollection<SvnOrigin> targets, SvnRevision start, SvnRevision end)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (targets == null)
                throw new ArgumentNullException("targets");

            LogSource.Targets = targets;
            LogSource.Start = start;
            LogSource.End = end;

            logRevisionControl1.Reset();
            logChangedPaths1.Reset();
            logMessageView1.Reset();
            logRevisionControl1.Start(context, LogMode.Log);
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
            else if (target == null)
                throw new ArgumentNullException("target");
            else if (source == null)
                throw new ArgumentNullException("source");

            LogSource.Targets = new SvnOrigin[]
            { new SvnOrigin(context, source, target.RepositoryRoot) }; // Must be from the same repository!
            LogSource.MergeTarget = target;
            logRevisionControl1.Reset();
            logChangedPaths1.Reset();
            logMessageView1.Reset();
            logRevisionControl1.Start(context, LogMode.MergesEligible);
        }

        public void StartMergesMerged(IAnkhServiceProvider context, SvnOrigin target, SvnTarget source)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else  if (target == null)
                throw new ArgumentNullException("target");
            else if (source == null)
                throw new ArgumentNullException("source");

            LogSource.Targets = new SvnOrigin[] 
            { new SvnOrigin(context, source, target.RepositoryRoot) }; // Must be from the same repository!
            LogSource.MergeTarget = target;
            logRevisionControl1.Reset();
            logChangedPaths1.Reset();
            logMessageView1.Reset();
            logRevisionControl1.Start(context, LogMode.MergesMerged);
        }

        internal void FetchAll()
        {
            logRevisionControl1.FetchAll();
        }

        public void Restart()
        {
            logRevisionControl1.Reset();
            logChangedPaths1.Reset();
            logMessageView1.Reset();
            logRevisionControl1.Start(Context.GetService<IAnkhServiceProvider>(), Mode);
        }

        public bool IncludeMerged
        {
            get { return LogSource.IncludeMergedRevisions; }
            set { LogSource.IncludeMergedRevisions = value; }
        }

        public bool StrictNodeHistory
        {
            get { return LogSource.StrictNodeHistory; }
            set { LogSource.StrictNodeHistory = value; }
        }

        bool _logMessageVisible = true;
        public bool LogMessageVisible
        {
            get { return _logMessageVisible; }
            set 
            {
                _logMessageVisible = value;
                UpdateSplitPanels();
            }
        }
        bool _changedPathsVisible = true;
        public bool ChangedPathsVisible
        {
            get { return _changedPathsVisible; }
            set 
            {
                _changedPathsVisible = value;
                UpdateSplitPanels();
            }
        }

        void UpdateSplitPanels()
        {
            splitContainer1.Panel2Collapsed = !_changedPathsVisible && !_logMessageVisible;
            splitContainer2.Panel1Collapsed = !_changedPathsVisible;
            splitContainer2.Panel2Collapsed = !_logMessageVisible;
        }

        public event EventHandler<BatchFinishedEventArgs> BatchFinished;

        public event SelectionChangedEventHandler<ISvnLogItem> SelectionChanged;

        public event FocusChangedEventHandler<ISvnLogItem> FocusChanged;

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

        void OnFocusChanged(object sender, ISvnLogItem e)
        {
            if(FocusChanged != null)
                FocusChanged(sender, e);
        }

        void OnSelectionChanged(object sender, IList<ISvnLogItem> e)
        {
            if (SelectionChanged != null)
                SelectionChanged(sender, e);
        }

        #endregion
    }
}
