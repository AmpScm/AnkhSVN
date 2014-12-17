using System;
using System.Diagnostics;
using Ankh.Scc;

namespace Ankh.Scc
{
    public interface IGitItemUpdate
    {
        //void RefreshTo(EventArgs e);
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
        int _cookie;

        public GitItem(IGitStatusCache context, string fullPath)
        {
            _context = context;
            _fullPath = fullPath;

            RefreshTo(null);
        }

        void RefreshTo(EventArgs e)
        {
            _cookie = NextCookie();
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
    }
}
