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
        readonly bool _versionInfo;

        public GitStatusData(GitStatusEventArgs status)
        {
            if (status == null)
                throw new ArgumentNullException("status");

            _conflicted = status.Conflicted;
            _indexStatus = status.IndexStatus;
            _ignored = status.Ignored;
            _conflicted = status.Conflicted;
            _versionInfo = true;
        }

        GitStatusData(NoSccStatus noSccStatus)
        {
            // TODO: Complete member initialization
            //this.noSccStatus = noSccStatus;
            _indexStatus = GitStatus.Normal;
            _workStatus = GitStatus.Normal;
            _versionInfo = (noSccStatus == (NoSccStatus)999);
        }

        #region Static instances
        readonly static GitStatusData _notVersioned = new GitStatusData(NoSccStatus.NotVersioned);
        readonly static GitStatusData _none = new GitStatusData(NoSccStatus.NotExisting);
        readonly static GitStatusData _root = new GitStatusData((NoSccStatus)999);
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

        public static GitStatusData Root
        {
            get { return _root;  }
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

        public bool HasVersionInfo
        {
            get { return _versionInfo; }
        }

    }

}
