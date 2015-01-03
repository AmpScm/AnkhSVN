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

            RefreshTo(status);
        }

        public GitItem(IGitStatusCache context, string fullPath, NoSccStatus status, SvnNodeKind nodeKind)
            : base(fullPath)
        {
            _context = context;

            if (status != NoSccStatus.Unknown)
                RefreshTo(status, nodeKind);
        }

        private void RefreshTo(NoSccStatus status, SvnNodeKind nodeKind)
        {
            _cookie = NextCookie();
            throw new NotImplementedException();
        }


        private void RefreshTo(GitStatusData status)
        {
            _cookie = NextCookie();
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }


        void IGitItemUpdate.TickItem()
        {
            throw new NotImplementedException();
        }


        public bool IsStatusClean()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        bool IGitItemUpdate.ShouldRefresh()
        {
            throw new NotImplementedException();
        }

        
        void IGitItemUpdate.RefreshTo(NoSccStatus noSccStatus, SvnNodeKind svnNodeKind)
        {
            throw new NotImplementedException();
        }


        bool IGitItemUpdate.ShouldClean()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            get { throw new NotImplementedException(); }
        }

        public override bool Exists
        {
            get { return true; }
        }

        public override bool IsDirectory
        {
            get { return true; }
        }

        public override bool IsFile
        {
            get { return true; }
        }
    }
}
