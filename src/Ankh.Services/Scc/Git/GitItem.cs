using System;
using System.Collections.Generic;
using System.Diagnostics;
using Ankh.Scc;
using SharpSvn;

namespace Ankh.Scc
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
    }
}

namespace Ankh
{
    /// <summary>
    /// Represents a version controlled path on disk, caching its status
    /// </summary>
    [DebuggerDisplay("Path={FullPath}")]
    public sealed partial class GitItem : IGitItemUpdate, IEquatable<GitItem>
    {
        readonly IGitStatusCache _context;
        readonly string _fullPath;

        enum XBool : sbyte
        {
            None = 0, // The three fastest values to check for most CPU's
            True = -1,
            False = 1
        }

        GitStatusData _status;
        bool _enqueued;

        static readonly Queue<GitItem> _stateChanged = new Queue<GitItem>();
        static bool _scheduled;

        ISvnWcReference _workingCopy;
        XBool _statusDirty; // updating, dirty, dirty 
        bool _ticked;
        int _cookie;
        DateTime _modified;
        bool _sccExcluded;

        public GitItem(IGitStatusCache context, string fullPath)
        {
            _context = context;
            _fullPath = fullPath;
        }

        public GitItem(IGitStatusCache context, string fullPath, GitStatusData status)
        {
            _context = context;
            _fullPath = fullPath;

            RefreshTo(status);
        }

        public GitItem(IGitStatusCache context, string fullPath, NoSccStatus status, SvnNodeKind nodeKind)
        {
            _context = context;
            _fullPath = fullPath;

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

        public string FullPath
        {
            [DebuggerStepThrough]
            get { return _fullPath; }
        }
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

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            return Equals(obj as GitItem);
        }

        /// <summary>
        /// Equalses the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public bool Equals(GitItem obj)
        {
            if ((object)obj == null)
                return false;

            return StringComparer.OrdinalIgnoreCase.Equals(obj.FullPath, FullPath);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(FullPath);
        }

        /// <summary>
        /// Gets a value which is incremented everytime the status was changed.
        /// </summary>
        /// <remarks>The cookie is provided globally over all <see cref="SvnItem"/> instances.  External users can be sure
        /// the status is 100% the same if the cookie did not change</remarks>
        public int ChangeCookie
        {
            get { return _cookie; }
        }

        static int _globalCookieBox = 0;

        /// <summary>
        /// Gets a new unique cookie
        /// </summary>
        /// <returns></returns>
        /// <remarks>Threadsafe provider of cookie values</remarks>
        static int NextCookie()
        {
            int n = System.Threading.Interlocked.Increment(ref _globalCookieBox); // Wraps on Int32.MaxValue

            if (n != 0)
                return n;
            else
                return NextCookie(); // 1 in 4 billion times
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

        public bool IsDirectory
        {
            get { return false; }
        }

        public bool IsVersioned
        {
            get { return false; }
        }

        public bool Exists
        {
            get { return false; }
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

            //SvnItemState current = lead._currentState;
            //SvnItemState valid = lead._validState;

            //SetState(current & valid, (~current) & valid);
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

        public bool IsBelowPath(string path)
        {
            throw new NotImplementedException();
        }
    }
}
