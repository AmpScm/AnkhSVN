using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.Services;
using System.Diagnostics;

namespace Ankh.UI.SvnLog
{
    public partial class LogToolWindowControl : UserControl
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

        [Obsolete]
        public void Start(IAnkhServiceProvider context, ICollection<string> targets)
        {
            logControl.Start(context, targets);
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
                IAnkhUISite site = value as IAnkhUISite;

                if (site != null)
                {
                    _site = site;

                    //logControl.Site = site;

                    // BH: This makes the control single-instance only
                    if (_site.GetService<LogToolWindowControl>() == null)
                        _site.Package.AddService(typeof(LogToolWindowControl), this);
                }
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

        public bool StrictNodeHistory
        {
            [DebuggerStepThrough]
            get { return logControl.StrictNodeHistory; }
            [DebuggerStepThrough]
            set { logControl.StrictNodeHistory = value; }
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
    }
}
