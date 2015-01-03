using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SharpSvn;

namespace Ankh.Scc.Engine
{
    public abstract partial class SccItem
    {
        readonly string _fullPath;

        [CLSCompliant(false)]
        protected enum XBool : sbyte
        {
            None = 0, // The three fastest values to check for most CPU's
            True = -1,
            False = 1
        }


        protected SccItem(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentNullException("fullPath");

            _fullPath = fullPath;
        }

        /// <summary>
        /// Gets the full normalized path of the item
        /// </summary>
        public string FullPath
        {
            [DebuggerStepThrough]
            get { return _fullPath; }
        }

        string _name;
        /// <summary>
        /// Gets the filename (including extension) of the item
        /// </summary>
        public string Name
        {
            [DebuggerStepThrough]
            get { return _name ?? (_name = Path.GetFileName(FullPath)); }
        }

        /// <summary>
        /// Gets the node kind of the file in subversion
        /// </summary>
        public abstract SvnNodeKind NodeKind
        {
            get;
        }

        public abstract bool IsFile
        {
            get;
        }

        public abstract bool IsDirectory
        {
            get;
        }

        public abstract bool IsVersioned
        {
            get;
        }

        /// <summary>
        /// Is this resource modified; implies the item is versioned
        /// </summary>
        public abstract bool IsModified
        {
            get;
        }

        /// <summary>
        /// Whether the item is potentially versionable.
        /// </summary>
        public abstract bool IsVersionable
        {
            get;
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> exists on disk
        /// </summary>
        public abstract bool Exists
        {
            get;
        }

        static int _globalCookieBox = 0;

        /// <summary>
        /// Gets a new unique cookie
        /// </summary>
        /// <returns></returns>
        /// <remarks>Threadsafe provider of cookie values</remarks>
        internal static int NextCookie()
        {
            int n = System.Threading.Interlocked.Increment(ref _globalCookieBox); // Wraps on Int32.MaxValue

            if (n != 0)
                return n;
            else
                return NextCookie(); // 1 in 4 billion times
        }


        public static bool IsValidPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            int lc = path.LastIndexOf(':');
            if (lc > 1)
                return false;
            else if (lc == 1 && path.IndexOf('\\') == 2)
                return true;
            else if (lc < 0 && path.StartsWith(@"\\", StringComparison.Ordinal))
                return true;

            // TODO: Add more checks. This code is called from the OpenDocumentTracker, Filestatus cache and selection provider

            return false;
        }
    }

    public abstract partial class SccItem<T> : SccItem, IEquatable<T>
        where T : SccItem<T>
    {
        protected SccItem(string fullPath)
            : base(fullPath)
        {
        }

        public bool Equals(T other)
        {
            if ((object)other == null)
                return false;

            return StringComparer.OrdinalIgnoreCase.Equals(other.FullPath, FullPath);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as T);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(FullPath);
        }
    }
}
