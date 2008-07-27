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

        public void StartLocalLog(IAnkhServiceProvider context, ICollection<string> targets)
        {
            logControl.StartLocalLog(context, targets);
        }

        public void StartMergesEligible(IAnkhServiceProvider context, string target, Uri source)
        {
            logControl.StartMergesEligible(context, target, source);
        }

        public void StartMergesMerged(IAnkhServiceProvider context, string target, Uri source)
        {
            logControl.StartMergesMerged(context, target, source);
        }

        public void StartRemoteLog(IAnkhServiceProvider context, Uri remoteTarget)
        {
            logControl.StartRemoteLog(context, remoteTarget);
        }

        IAnkhUISite _site;
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
