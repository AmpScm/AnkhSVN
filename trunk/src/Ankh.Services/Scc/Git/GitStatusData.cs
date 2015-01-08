using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using SharpGit;
using SharpSvn;

namespace Ankh.Scc
{
    //[DebuggerDisplay("Content={LocalContentStatus}, Property={LocalPropertyStatus}, Uri={Uri}")]
    public sealed class GitStatusData
    {
        readonly bool _conflicted;
        readonly GitStatus _indexStatus;
        readonly GitStatus _workStatus;
        readonly bool _ignored;
        readonly SvnNodeKind _kind;
        readonly bool _modified;

        public GitStatusData(GitStatusEventArgs status)
        {
            if (status == null)
                throw new ArgumentNullException("status");

            _conflicted = status.Conflicted;
            _indexStatus = status.IndexStatus;
            _workStatus = status.WorkingDirectoryStatus;
            _ignored = status.Ignored;
            _conflicted = status.Conflicted;
            _kind = (SvnNodeKind)(int)status.NodeKind;
            _modified = status.IndexModified || status.WorkingDirectoryModified;
        }

        GitStatusData(NoSccStatus noSccStatus)
        {
            // TODO: Complete member initialization
            //this.noSccStatus = noSccStatus;
            _indexStatus = GitStatus.Normal;
            _workStatus = GitStatus.Normal;
            _kind = SvnNodeKind.Unknown;
        }

        #region Static instances
        readonly static GitStatusData _notVersioned = new GitStatusData(NoSccStatus.NotVersioned);
        readonly static GitStatusData _none = new GitStatusData(NoSccStatus.NotExisting);
        /// <summary>
        /// Default status for nodes which do exist but are not managed
        /// </summary>
        internal static GitStatusData NotVersioned
        {
            get { return _notVersioned; }
        }

        /// <summary>
        /// Default status for nodes which don't exist and are not managed
        /// </summary>
        internal static GitStatusData NotExisting
        {
            get { return _none; }
        }

        #endregion

        public GitStatus IndexStatus
        {
            get { return _indexStatus; }
        }

        /// <summary>
        /// Content status in working copy
        /// </summary>
        public GitStatus WorkingStatus
        {
            get { return _workStatus; }
        }

        public bool Modified
        {
            get { return _modified; }
        }

        public SvnNodeKind NodeKind
        {
            get { return _kind; }
        }

/*        public GitStatus CombinedStatus
        {
            get
            {
            }
        }*/

        public bool IsConflicted
        {
            get { return _conflicted; }
        }

        public bool IsIgnored
        {
            get { return _ignored; }
        }

    }

}
