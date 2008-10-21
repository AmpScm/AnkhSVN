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
using Ankh.Scc;

namespace Ankh.UI.SvnLog
{
    public partial class LogToolWindowControl : AnkhToolWindowControl, ILogControl
    {
        string _originalText;
        IList<SvnOrigin> _origins;
        public LogToolWindowControl()
        {
            InitializeComponent();
            LogSource = logControl.LogSource;
        }

        public LogToolWindowControl(IContainer container)
            : this()
        {
            container.Add(this);
        }

        LogDataSource _dataSource;
        internal LogDataSource LogSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        protected override void OnFrameCreated(EventArgs e)
        {
            base.OnFrameCreated(e);

            _originalText = Text;

            ToolWindowHost.CommandContext = AnkhId.LogContextGuid;
            ToolWindowHost.KeyboardContext = AnkhId.LogContextGuid;
            logControl.Context = Context;
        }

        public IList<SvnOrigin> Origins
        {
            get
            {
                if (_origins == null)
                    return null;

                return new List<SvnOrigin>(_origins);
            }
        }

        void UpdateTitle()
        {
            Text = _originalText;
            if (_origins != null || _origins.Count != 1)
            {
                return;
            }

            foreach (SvnOrigin origin in _origins)
            {
                Text += string.Format(" - {0}", origin.Target.TargetName);
                break;
            }
        }

        public void StartLog(IAnkhServiceProvider context, SvnOrigin target, SvnRevision start, SvnRevision end)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            StartLog(context, new SvnOrigin[] { target }, start, end);
        }

        public void StartLog(IAnkhServiceProvider context, ICollection<SvnOrigin> targets, SvnRevision start, SvnRevision end)
        {
            if (targets == null)
                throw new ArgumentNullException("targets");

            _origins = new List<SvnOrigin>(targets);

            UpdateTitle();

            logControl.StartLog(context, _origins, start, end);
        }

        [Obsolete("Pass SvnOrigins")]
        public void StartLocalLog(IAnkhServiceProvider context, ICollection<SvnItem> targets)
        {
            StartLocalLog(context, targets, null, null);
        }

        [Obsolete("Pass SvnOrigins")]
        public void StartLocalLog(IAnkhServiceProvider context, ICollection<SvnItem> targets, SvnRevision start, SvnRevision end)
        {
            if (targets == null)
                throw new ArgumentNullException("targets");

            List<SvnOrigin> t = new List<SvnOrigin>();
            foreach (SvnItem item in targets)
            {
                t.Add(new SvnOrigin(item));
            }

            StartLog(context, t, start, end);
        }

        public void StartMergesEligible(IAnkhServiceProvider context, SvnItem target, Uri source)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            SvnOrigin origin = new SvnOrigin(target);
            _origins = new SvnOrigin[] { origin };
            UpdateTitle();
            logControl.StartMergesEligible(context, origin, source);
        }

        public void StartMergesMerged(IAnkhServiceProvider context, SvnItem target, Uri source)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            SvnOrigin origin = new SvnOrigin(target);
            _origins = new SvnOrigin[] { origin };
            UpdateTitle();
            logControl.StartMergesMerged(context, origin, source);
        }

        [Obsolete("Please pass origin directly")]
        public void StartRemoteLog(IAnkhServiceProvider context, ISvnRepositoryItem remoteTarget)
        {
            StartLog(context, remoteTarget.Origin, null, null);
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
