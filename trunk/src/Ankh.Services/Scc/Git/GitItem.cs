using System;
using System.Collections.Generic;
using System.Diagnostics;
using Ankh.Scc;
using Ankh.Scc.Git;
using SharpSvn;

namespace Ankh.Scc.Git
{
    public interface IGitItemUpdate
    {
        //void RefreshTo(EventArgs e);
        bool IsItemTicked();

        void TickItem();

        bool IsStatusClean();

        bool ShouldRefresh();

        void RefreshTo(NoSccStatus noSccStatus, SvnNodeKind svnNodeKind);

        bool ShouldClean();

        void RefreshTo(GitItem newItem);

        void RefreshTo(GitStatusData status);

        void SetState(GitItemState set, GitItemState unset);

        void SetDirty(GitItemState dirty);

        bool TryGetState(GitItemState get, out GitItemState value);
    }
}

namespace Ankh
{
    /// <summary>
    /// Represents a version controlled path on disk, caching its status
    /// </summary>
    [DebuggerDisplay("Path={FullPath}")]
    public sealed partial class GitItem : Ankh.Scc.Engine.SccItem<GitItem>, IGitItemUpdate
    {
        readonly IGitStatusCache _context;

        IGitStatusCache StatusCache
        {
            [DebuggerStepThrough]
            get { return _context; }
        }

        GitStatusData _status;
        bool _enqueued;

        static readonly Queue<GitItem> _stateChanged = new Queue<GitItem>();
        static bool _scheduled;

        XBool _statusDirty; // updating, dirty, dirty 
        bool _ticked;
        int _cookie;
        DateTime _modified;
        bool _sccExcluded;

        public GitItem(IGitStatusCache context, string fullPath, GitStatusData status)
            : base(fullPath)
        {
            _context = context;
            _status = status;

            _enqueued = true;
            RefreshTo(status);
            _enqueued = false;
        }

        public GitItem(IGitStatusCache context, string fullPath, NoSccStatus status, SvnNodeKind nodeKind)
            : base(fullPath)
        {
            _context = context;

            RefreshTo(status, nodeKind);
        }

        private void RefreshTo(NoSccStatus status, SvnNodeKind nodeKind)
        {
            _cookie = NextCookie();
            _statusDirty = XBool.False;

            GitItemState set = GitItemState.None;
            GitItemState unset = GitItemState.Modified | GitItemState.Added
                | GitItemState.Deleted | GitItemState.Conflicted | GitItemState.Ignored | GitItemState.Versioned | GitItemState.IsWCRoot | GitItemState.GitDirty | GitItemState.Ignored;

            switch (status)
            {
                case NoSccStatus.NotExisting:
                    SetState(set, GitItemState.Exists | GitItemState.ReadOnly | GitItemState.IsDiskFile | GitItemState.IsDiskFolder | GitItemState.Versionable | unset);
                    _status = GitStatusData.NotExisting;
                    break;
                case NoSccStatus.NotVersionable:
                    unset |= GitItemState.Versionable;
                    goto case NoSccStatus.NotVersioned; // fall through
                case NoSccStatus.NotVersioned:
                    SetState(GitItemState.Exists | set, GitItemState.None | unset);
                    _status = GitStatusData.NotVersioned;
                    break;
                case NoSccStatus.Unknown:
                default:
                    SetDirty(set | unset);
                    _statusDirty = XBool.True;
                    break;
            }

            //InitializeFromKind(nodeKind);
        }


        private void RefreshTo(GitStatusData status)
        {
            _cookie = NextCookie();
            _statusDirty = XBool.False;
            _status = status;

            GitItemState set = GitItemState.None;
            GitItemState unset = GitItemState.None;

            if (status.IsConflicted)
                set |= GitItemState.Conflicted;
            else
                unset |= GitItemState.Conflicted;

            if (status.IndexStatus == SharpGit.GitStatus.Normal
                && status.WorkingStatus == SharpGit.GitStatus.Normal)
            {
                // We don't know if the node is a file or directory yet
                set |= GitItemState.Versioned | GitItemState.Versionable | GitItemState.Exists;
                unset |= GitItemState.Added | GitItemState.Deleted | GitItemState.Modified | GitItemState.Ignored | GitItemState.GitDirty;
            }
            else
            {
                throw new NotImplementedException();
            }

            SetState(set, unset);
        }

        IGitStatusCache GitCache
        {
            [DebuggerStepThrough]
            get { return _context; }
        }

        #region Comparison
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="one">The one.</param>
        /// <param name="other">The other.</param>
        /// <returns>The result of the operator.</returns>
        [DebuggerStepThrough]
        public static bool operator ==(GitItem one, GitItem other)
        {
            bool n1 = (object)one == null;
            bool n2 = (object)other == null;

            if (n1 || n2)
                return n1 && n2;

            return StringComparer.OrdinalIgnoreCase.Equals(one.FullPath, other.FullPath);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="one">The one.</param>
        /// <param name="other">The other.</param>
        /// <returns>The result of the operator.</returns>
        [DebuggerStepThrough]
        public static bool operator !=(GitItem one, GitItem other)
        {
            return !(one == other);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        /// <summary>
        /// Gets a value which is incremented everytime the status was changed.
        /// </summary>
        /// <remarks>The cookie is provided globally over all <see cref="SvnItem"/> instances.  External users can be sure
        /// the status is 100% the same if the cookie did not change</remarks>
        public int ChangeCookie
        {
            get { return _cookie; }
        }

        bool IGitItemUpdate.IsItemTicked()
        {
            return _ticked;
        }


        void IGitItemUpdate.TickItem()
        {
            _ticked = true; // Will be updated soon
        }


        public bool IsStatusClean()
        {
            return _statusDirty == XBool.False;
        }

        void RefreshStatus()
        {
            _statusDirty = XBool.None;
            IGitStatusCache statusCache = StatusCache;

            try
            {
                statusCache.RefreshItem(this, IsFile ? SvnNodeKind.File : SvnNodeKind.Directory); // We can check this less expensive than the statuscache!
            }
            finally
            {
                Debug.Assert(_statusDirty == XBool.False, "No longer dirty after refresh", string.Format("Path = {0}", FullPath));
                _statusDirty = XBool.False;
            }
        }

        bool IGitItemUpdate.IsStatusClean()
        {
            return _statusDirty == XBool.False;
        }

        bool IGitItemUpdate.ShouldRefresh()
        {
            return _ticked || _statusDirty != XBool.False;
        }

        
        void IGitItemUpdate.RefreshTo(NoSccStatus status, SvnNodeKind nodeKind)
        {
            RefreshTo(status, nodeKind);
        }


        bool IGitItemUpdate.ShouldClean()
        {
            return _ticked || (_statusDirty == XBool.False && _status == GitStatusData.NotExisting);
        }

        void IGitItemUpdate.RefreshTo(GitItem lead)
        {
            if (lead == null)
                throw new ArgumentNullException("lead");
            else if (lead._status == null)
                throw new InvalidOperationException("Lead status = null");

            _status = lead._status;
            _statusDirty = lead._statusDirty;

            GitItemState current = lead._currentState;
            GitItemState valid = lead._validState;

            SetState(current & valid, (~current) & valid);
            _ticked = false;
            _modified = lead._modified;
            _cookie = NextCookie(); // Status 100% the same, but changed... Cookies are free ;)
        }

        void IGitItemUpdate.RefreshTo(GitStatusData newData)
        {
            RefreshTo(newData);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void MarkDirty()
        {
            Debug.Assert(_statusDirty != XBool.None, "MarkDirty called while updating status");

            _statusDirty = XBool.True;

            _validState = GitItemState.None;
            _cookie = NextCookie();
            //_workingCopy = null;
            _modified = new DateTime();
            //_conflicts = null;
        }

        public bool IsBelowPath(string root)
        {
            if (string.IsNullOrEmpty(root))
                throw new ArgumentNullException("root");

            return SvnItem.IsBelowRoot(FullPath, root);
        }

        bool TryGetState(GitItemState mask, out GitItemState result)
        {
            if ((mask & _validState) != mask)
            {
                result = GitItemState.None;
                return false;
            }

            result = _currentState & mask;
            return true;
        }

        public override SvnNodeKind NodeKind
        {
            get { return IsFile ? SvnNodeKind.File : (IsDirectory ? SvnNodeKind.Directory : SvnNodeKind.None); }
        }

        /// <summary>
        /// Is this item versioned?
        /// </summary>
        public override bool IsVersioned
        {
            get { return 0 != GetState(GitItemState.Versioned); }
        }

        /// <summary>
        /// Is this resource modified; implies the item is versioned
        /// </summary>
        public override bool IsModified
        {
            get
            {
                return GetState(GitItemState.SvnDirty) != 0;
            }
        }

        public override bool IsVersionable
        {
            get { return GetState(GitItemState.Versionable) != 0; }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> exists on disk
        /// </summary>
        public override bool Exists
        {
            get { return GetState(GitItemState.Exists) != 0; }
        }

        /// <summary>
        /// Gets a boolean indicating whether the item (on disk) is a directory
        /// </summary>
        /// <remarks>Use <see cref="Status"/>.<see cref="SvnStatusData.SvnNodeKind"/> to retrieve the svn type</remarks>
        public override bool IsDirectory
        {
            get { return GetState(GitItemState.IsDiskFolder) == GitItemState.IsDiskFolder; }
        }

        /// <summary>
        /// Gets a boolean indicating whether the item (on disk) is a file
        /// </summary>
        /// <remarks>Use <see cref="Status"/>.<see cref="SvnStatusData.SvnNodeKind"/> to retrieve the svn type</remarks>
        public override bool IsFile
        {
            get { return GetState(GitItemState.IsDiskFile) == GitItemState.IsDiskFile; }
        }

        public override GitItem Parent
        { 
            get
            {
                string parentDir = Directory;

                if (string.IsNullOrEmpty(parentDir))
                    return null; // We are the root folder!

                IGitStatusCache cache = StatusCache;

                if (cache != null)
                    return cache.GetAlreadyNormalizedItem(parentDir);
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> is in conflict state
        /// </summary>
        public bool IsConflicted
        {
            get { return 0 != GetState(GitItemState.Conflicted); }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="SvnItem"/> is in one of the projects in the solution
        /// </summary>
        /// <value><c>true</c> if the file is in one of the projects of the solution; otherwise, <c>false</c>.</value>
        public override bool InSolution
        {
            get { return GetState(GitItemState.InSolution) != 0; }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="ScnItem"/> is explicitly Scc Excluded
        /// </summary>
        public override bool IsSccExcluded
        {
            get { return InSolution && _sccExcluded; }
        }

        public override bool IsIgnored
        {
            get
            {
                GitItemState state;

                if (TryGetState(GitItemState.Versioned, out state) && state != 0)
                    return false;
                else if (TryGetState(GitItemState.Versionable, out state) && state == 0)
                    return false;
                else if (GetState(GitItemState.Ignored) != 0)
                    return true;
                else if (IsVersioned)
                    return false;
                else if (!Exists)
                    return false;

                GitItem parent = Parent;
                if (parent != null)
                    return parent.IsIgnored;
                else
                    return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this file is dirty in an open editor
        /// </summary>
        public override bool IsDocumentDirty
        {
            get { return GetState(GitItemState.DocumentDirty) != 0; }
        }

        /// <summary>
        /// Gets a value indicating whether this node is a nested working copy.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is nested working copy; otherwise, <c>false</c>.
        /// </value>
        public bool IsWCRoot
        {
            get { return GetState(GitItemState.IsWCRoot) != 0; }
        }
    }
}
