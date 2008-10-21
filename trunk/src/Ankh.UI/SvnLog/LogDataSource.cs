using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Ankh.Scc;
using SharpSvn;

namespace Ankh.UI.SvnLog
{
    internal partial class LogDataSource : Component
    {
        public LogDataSource()
        {
            InitializeComponent();
        }

        public LogDataSource(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        ISynchronizeInvoke _synchronizer;
        public ISynchronizeInvoke Synchronizer
        {
            get { return _synchronizer; }
            set { _synchronizer = value; }
        }

        ICollection<SvnOrigin> _targets;
        SvnOrigin _mergeTarget;
        public ICollection<SvnOrigin> Targets
        {
            get { return _targets; }
            set { _targets = value; }
        }

        public SvnOrigin MergeTarget
        {
            get { return _mergeTarget; }
            set { _mergeTarget = value; }
        }

        public Uri RepositoryRoot
        {
            get
            {
                SvnOrigin o = EnumTools.GetFirst(Targets);
                if (o != null)
                    return o.RepositoryRoot;
                else
                    return null;
            }
        }

        SvnRevision _start, _end;
        public SvnRevision Start
        {
            get { return _start; }
            set { _start = value; }
        }

        public SvnRevision End
        {
            get { return _end; }
            set { _end = value; }
        }

        int _limit = -1;
        public int Limit
        {
            get { return _limit; }
            set { _limit = value; }
        }

        bool _strictNodeHistory, _includeMerged;
        public bool StrictNodeHistory
        {
            get { return _strictNodeHistory; }
            set { _strictNodeHistory = value; }
        }

        public bool IncludeMergedRevisions
        {
            get { return _includeMerged; }
            set { _includeMerged = value; }
        }
    }
}
