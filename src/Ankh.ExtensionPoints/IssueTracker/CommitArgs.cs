using System;
using System.Collections.Generic;

namespace Ankh.ExtensionPoints.IssueTracker
{
    public abstract class CommitArgs
    {
        private ICollection<Uri> _paths;
        private long _revision;
        protected string _commitMessage;

        public CommitArgs(ICollection<Uri> paths, long revision, string commitMessage)
        {
            _paths = paths;
            _revision = revision;
            _commitMessage = commitMessage;
        }

        public ICollection<Uri> Paths
        {
            get { return _paths; }
        }

        public long Revision
        {
            get { return _revision; }
        }

        protected string InternalCommitMessage
        {
            get { return _commitMessage; }
            set { _commitMessage = value; }
        }
    }

    [System.Runtime.InteropServices.Guid("CC144F45-AFF0-47b3-B7B0-3DBB48F7D477")]
    public class PreCommitArgs : CommitArgs
    {
        bool _cancel = false;

        public PreCommitArgs(ICollection<Uri> paths, long revision)
            : this(paths, revision, string.Empty)
        {
        }

        public PreCommitArgs(ICollection<Uri> paths, long revision, string commitMessage)
            : base(paths, revision, commitMessage)
        {
        }

        public bool Cancel
        {
            get { return _cancel; }
            set { _cancel = value; }
        }

        public string CommitMessage
        {
            get { return base.InternalCommitMessage; }
            set { base.InternalCommitMessage = value; }
        }
    }

    [System.Runtime.InteropServices.Guid("A1977B0F-4A9F-4ce2-A937-BEF01057D2BF")]
    public class PostCommitArgs : CommitArgs
    {
        public PostCommitArgs(ICollection<Uri> paths, long revision)
            : this(paths, revision, string.Empty)
        {
        }

        public PostCommitArgs(ICollection<Uri> paths, long revision, string commitMessage)
            : base(paths, revision, commitMessage)
        {
        }

        public string CommitMessage
        {
            get { return base.InternalCommitMessage; }
        }
    }
}
