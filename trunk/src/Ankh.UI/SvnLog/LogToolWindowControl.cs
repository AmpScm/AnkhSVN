using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.Services;
using System.Diagnostics;
using Ankh.Scc.UI;
using Ankh.Ids;
using SharpSvn;

namespace Ankh.UI.SvnLog
{
    public partial class LogToolWindowControl : AnkhToolWindowControl, ILogControl
    {
        public LogToolWindowControl()
        {
            InitializeComponent();
        }

        public LogToolWindowControl(IContainer container)
            :this()
        {
            container.Add(this);
        }

        protected override void OnFrameCreated(EventArgs e)
        {
            base.OnFrameCreated(e);

            ToolWindowHost.CommandContext = AnkhId.LogContextGuid;
            ToolWindowHost.KeyboardContext = AnkhId.LogContextGuid;
			logControl.Context = Context;
        }

        SvnItem[] _localItems;

        public bool HasWorkingCopyItems
        {
            get { return _localItems != null && _localItems.Length > 0; }
        }

        public SvnItem[] WorkingCopyItems
        {
            get
            {
                if (_localItems != null)
                    return (SvnItem[])_localItems.Clone();
                else
                    return new SvnItem[0];
            }
        }

        public void StartLocalLog(IAnkhServiceProvider context, ICollection<SvnItem> targets)
        {
            StartLocalLog(context, targets, null, null);
        }
        public void StartLocalLog(IAnkhServiceProvider context, ICollection<SvnItem> targets, SvnRevision start, SvnRevision end)
        {
            if (targets == null)
                throw new ArgumentNullException("targets");

            _localItems = new SvnItem[targets.Count];
            targets.CopyTo(_localItems, 0);

            logControl.StartLocalLog(context, SvnItem.GetPaths(targets), start, end);
        }

        public void StartMergesEligible(IAnkhServiceProvider context, SvnItem target, Uri source)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            _localItems = new SvnItem[] { target };
            logControl.StartMergesEligible(context, target.FullPath, source);
        }

        public void StartMergesMerged(IAnkhServiceProvider context, SvnItem target, Uri source)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            _localItems = new SvnItem[] { target };
            logControl.StartMergesMerged(context, target.FullPath, source);
        }

        public void StartRemoteLog(IAnkhServiceProvider context, Uri remoteTarget)
        {
            _localItems = null;
            logControl.StartRemoteLog(context, remoteTarget);
        }

        public override ISite Site
        {
            get { return base.Site; }
            set
            {
                base.Site = value;
                logControl.Site = value;
            }
        }

        public bool LogMessageVisible
        {
            [DebuggerStepThrough]
            get { return logControl.LogMessageVisible; }
            [DebuggerStepThrough]
            set { logControl.LogMessageVisible = value; }
        }

        public bool ChangedPathsVisible
        {
            [DebuggerStepThrough]
            get { return logControl.ChangedPathsVisible; }
            [DebuggerStepThrough]
            set { logControl.ChangedPathsVisible = value; }
        }        

        public bool IncludeMerged
        {
            [DebuggerStepThrough]
            get { return logControl.IncludeMerged; }
            [DebuggerStepThrough]
            set { logControl.IncludeMerged = value; }
        }

        [DebuggerStepThrough]
        public void FetchAll()
        {
            logControl.FetchAll();
        }

        [DebuggerStepThrough]
        public void Restart()
        {
            logControl.Restart();
        }

        #region ILogControl Members

        public bool ShowChangedPaths
        {
            get
            {
                return logControl.ChangedPathsVisible;
            }
            set
            {
                logControl.ChangedPathsVisible = value;
            }
        }

        public bool ShowLogMessage
        {
            get
            {
                return logControl.LogMessageVisible;
            }
            set
            {
                logControl.LogMessageVisible = value;
            }
        }

        public bool IncludeMergedRevisions
        {
            get { return logControl.IncludeMerged; }
            set
            {
                if (value != logControl.IncludeMerged)
                {
                    logControl.IncludeMerged = value;
                    logControl.Restart();
                }
            }
        }

        public bool StrictNodeHistory
        {
            [DebuggerStepThrough]
            get { return logControl.StrictNodeHistory; }
            set
            {
                if (value != StrictNodeHistory)
                {
                    logControl.StrictNodeHistory = value;
                    logControl.Restart();
                }
            }
        }

        #endregion     
    }
}
