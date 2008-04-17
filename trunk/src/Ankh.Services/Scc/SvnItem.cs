// $Id$
using System;


using System.IO;
using System.Collections;
using Ankh.UI;
using System.Diagnostics;
using SharpSvn;
using System.Collections.ObjectModel;
using Ankh.Selection;
using Ankh.Scc;
using SharpSvn.Implementation;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Ankh
{
    public interface ISvnItemUpdate
    {
        void RefreshTo(AnkhStatus status);
        void RefreshTo(SvnItem lead);
        void TickItem();
        bool IsItemTicked();
        bool ShouldRefresh();
        bool IsStatusClean();

        void MarkStatusDirty();

        bool ShouldClean();
    }

    /// <summary>
    /// Represents a version controlled path on disk, caching its status
    /// </summary>
    [DebuggerDisplay("Path={FullPath}")]
    public sealed class SvnItem : LocalSvnItem, ISvnItemUpdate
    {
        enum XBool : sbyte
        {
            None = 0, // The three fastest values to check for most CPU's
            True = -1,
            False = 1
        }

        readonly IAnkhServiceProvider _context;
        readonly string _fullPath;

        AnkhStatus _status;
        string _name;
        XBool _statusDirty; // updating, dirty, dirty
        XBool _isFile; // Unknown, file, directory  // On disk status; Can conflict with SVN type!
        XBool _exists; // Unknown, Yes, No
        XBool _readOnly; // Unknown, Yes, No
        XBool _mustLock; // Unknown, Yes, No
        XBool _isVersionable; // Unknown, Yes, No
        bool _ticked;
        int _cookie;

        public SvnItem(IAnkhServiceProvider context, string fullPath, AnkhStatus status)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentNullException("fullPath");
            else if (status == null)
                throw new ArgumentNullException("status");

            _context = context;
            _fullPath = fullPath;
            _status = status;

            RefreshTo(status);
        }

        public SvnItem(IAnkhServiceProvider context, string fullPath, AnkhStatus status, SvnNodeKind nodeKind)
            : this(context, fullPath, status)
        {
            switch (nodeKind) // We assume the caller checked this for us
            {
                case SvnNodeKind.File:
                    _isFile = XBool.True;
                    _exists = XBool.True;
                    break;
                case SvnNodeKind.Directory:
                    _isFile = XBool.False;
                    _readOnly = XBool.False; // Useless info if directory
                    _mustLock = XBool.False;
                    break;
            }
        }


        void RefreshTo(AnkhStatus status)
        {
            if (status == null)
                throw new ArgumentNullException("status");            

            if (status.LocalContentStatus == SvnStatus.External)
            {
                // When iterating the status of an external in it's parent directory
                // We get an external status and no really usefull information

                _exists = XBool.True;
                _isFile = XBool.False; // An external is a directory in Subversion -1.5
                _readOnly = XBool.False; // A directory can't be read only on windows
                _isVersionable = XBool.True; // External WC, or my WC.. 

                if (_statusDirty == XBool.False)
                    return; // Just skip the rest.. Fill it when iterating the directory itself

                _statusDirty = XBool.True; // Walk the path itself to get the data

                return;
            }
            else if (!ReferenceEquals(status, AnkhStatus.NotVersioned) &&
                (status.LocalContentStatus == SvnStatus.NotVersioned) &&
                 IsDirectory)
            {
                // A not versioned directory might be a working copy by itself!

                if (_statusDirty == XBool.False)
                    return; // No need to remove valid cache entries

                if(SvnTools.IsManagedPath(FullPath))
                {
                    _statusDirty = XBool.True; // Walk the path itself to get the data

                    // Extract usefull information we got anyay
                    _exists = XBool.True;
                    _isVersionable = XBool.True;
                    _isFile = XBool.False;
                    _readOnly = XBool.False;

                    return;
                }
                // Fall through
            }

            _cookie = NextCookie();            
            _statusDirty = XBool.False;
            _status = status;

            // Let's assume status is more recent than our internal property cache
            // Set all caching properties we can

            switch (status.LocalContentStatus)
            {
                case SvnStatus.NotVersioned: // Node exists
                case SvnStatus.Normal:
                case SvnStatus.Added:
                case SvnStatus.Replaced:
                case SvnStatus.Modified:
                case SvnStatus.Ignored:
                case SvnStatus.Conflicted:

                case SvnStatus.Obstructed: // node exists but is of the wrong type

                    _exists = XBool.True; // SVN checked this for us
                    break;
                case SvnStatus.None:
                case SvnStatus.Missing:
                    _exists = XBool.False;
                    break;
                default:
                    // case SvnStatus.Deleted: // The file is scheduled for delete; but can exist locally
                    break;

            }

            // In the obstructed case the NodeKind is incorrect!
            switch (status.NodeKind)
            {
                case SvnNodeKind.Directory:
                    _isFile = XBool.False;
                    _readOnly = XBool.False; // Never set for directory
                    _mustLock = XBool.False; // Not available on directory
                    break;
                case SvnNodeKind.File:
                    _isFile = XBool.True;
                    break;
            }
        }

        void ISvnItemUpdate.RefreshTo(AnkhStatus status)
        {
            _ticked = false;
            RefreshTo(status);
        }

        /// <summary>
        /// Copies all information from other.
        /// </summary>
        /// <param name="other"></param>
        /// <remarks>When this method is called the other item will eventually replace this item</remarks>
        void ISvnItemUpdate.RefreshTo(SvnItem lead)
        {
            _status = lead._status;
            _statusDirty = lead._statusDirty;
            _isFile = lead._isFile;
            _exists = lead._exists;
            _readOnly = lead._readOnly;
            _mustLock = lead._mustLock;
            _isVersionable = lead._isVersionable;
            _ticked = false;
            _cookie = NextCookie(); // Status 100% the same, but changed... Cookies are free ;)
        }

        public void MarkDirty()
        {
            Debug.Assert(_statusDirty != XBool.None, "MarkDirty called while updating status");

            _statusDirty = XBool.True;
            _isFile = XBool.None;
            _exists = XBool.None;
            _readOnly = XBool.None;
            _mustLock = XBool.None;
            _isVersionable = XBool.None;
            _cookie = NextCookie();
        }

        void ISvnItemUpdate.MarkStatusDirty()
        {
            _statusDirty = XBool.True;
            _cookie = NextCookie();
        }

        bool ISvnItemUpdate.IsStatusClean()
        {
            return _statusDirty == XBool.False;
        }

        /// <summary>
        /// 
        /// </summary>
        void ISvnItemUpdate.TickItem()
        {
            _ticked = true; // Will be updated soon
        }

        public bool ShouldClean()
        {
            return _ticked || (_statusDirty == XBool.False && _status == AnkhStatus.NotExisting);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool ISvnItemUpdate.IsItemTicked()
        {
            return _ticked;
        }

        bool ISvnItemUpdate.ShouldRefresh()
        {
            return _ticked || _statusDirty != XBool.False;
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

        void UpdateAttributeInfo()
        {
            // One call of the kernel's GetFileAttributesW() gives us most info we need

            uint value = NativeMethods.GetFileAttributes(FullPath);

            if (value == NativeMethods.INVALID_FILE_ATTRIBUTES)
            {
                // File does not exist / no rights, etc.

                _isFile = XBool.True; // File allows more optimizations in the svn case
                _exists = XBool.False; // Set all checks to false
                _readOnly = XBool.False;
                _mustLock = XBool.False;
                _isVersionable = XBool.False;
                return;
            }

            _exists = XBool.True;

            if ((value & NativeMethods.FILE_ATTRIBUTE_READONLY) != 0)
                _readOnly = XBool.True;

            if ((value & NativeMethods.FILE_ATTRIBUTE_DIRECTORY) != 0)
            {
                _isFile = XBool.False;
                _readOnly = XBool.False; // No meaning on directories; ignore                
            }
            else
            {
                _isFile = XBool.True;
            }
        }

        /// <summary>
        /// Gets the full normalized path of the item
        /// </summary>
        public string FullPath
        {
            [DebuggerStepThrough]
            get { return _fullPath; }
        }

        /// <summary>
        /// Gets the filename (including extension) of the item
        /// </summary>
        public string Name
        {
            get { return _name ?? (_name = Path.GetFileName(FullPath)); }
        }

        /// <summary>
        /// Gets the SVN status of the item; retrieves a placeholder if the status is unknown
        /// </summary>
        public AnkhStatus Status
        {
            get
            {
                EnsureClean();

                return _status;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the item (on disk) is a file
        /// </summary>
        /// <remarks>Use <see cref="Status"/>.<see cref="AnkhStatus.SvnNodeKind"/> to retrieve the svn type</remarks>
        public bool IsFile
        {
            get
            {
                if (_isFile == XBool.None)
                    UpdateAttributeInfo();

                return _isFile == XBool.True;
            }
        }

        public bool HasHistory
        {
            get
            {
                AnkhStatus status = Status;
                return GetIsVersioned(status) && (status.LocalContentStatus != SvnStatus.Added || status.IsCopied);
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the item (on disk) is a directory
        /// </summary>
        /// <remarks>Use <see cref="Status"/>.<see cref="AnkhStatus.SvnNodeKind"/> to retrieve the svn type</remarks>
        public bool IsDirectory
        {
            get
            {
                if (_isFile == XBool.None)
                    UpdateAttributeInfo();

                return _isFile == XBool.False;
            }
        }

        /// <summary>
        /// Gets the <see cref="SvnNodeKind"/> of the item (on disk)
        /// </summary>
        /// <value><see cref="SvnNodeKind.Directory"/> if <see cref="IsDirectory"/> otherwise <see cref="SvnNodeKind.File"/></value>
        public SvnNodeKind NodeKind
        {
            get { return IsDirectory ? SvnNodeKind.Directory : SvnNodeKind.File; }
        }

        void RefreshMe()
        {
            _statusDirty = XBool.None;
            IFileStatusCache statusCache = _context.GetService<IFileStatusCache>();

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

        /// <summary>
        /// Is this item versioned?
        /// </summary>
        public bool IsVersioned
        {
            get
            {
                EnsureClean();

                return GetIsVersioned(_status);
            }
        }

        static bool GetIsVersioned(AnkhStatus status)
        {
            switch (status.LocalContentStatus)
            {
                case SvnStatus.Added:
                case SvnStatus.Conflicted:
                case SvnStatus.Merged:
                case SvnStatus.Modified:
                case SvnStatus.Normal:
                case SvnStatus.Replaced:
                case SvnStatus.Deleted:
                case SvnStatus.Incomplete:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Is this resource modified; implies the item is versioned
        /// </summary>
        public bool IsModified
        {
            get
            {
                EnsureClean();

                AnkhStatus status = _status;

                return GetIsVersioned(status) && (status.CombinedStatus != SvnStatus.Normal);
            }
        }

        /// <summary>
        /// Whether the item is potentially versionable.
        /// </summary>
        public bool IsVersionable
        {
            get
            {
                if (_isVersionable == XBool.None)
                {
                    if ((_statusDirty == XBool.False) && GetIsVersioned(_status))
                        _isVersionable = XBool.True; // File is versioned; sure it is versionable too
                    else if (Exists && SvnTools.IsBelowManagedPath(FullPath))
                        _isVersionable = XBool.True;
                    else
                        _isVersionable = XBool.False;
                }

                return _isVersionable == XBool.True;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> specifies a readonly file
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                if (_readOnly == XBool.None)
                    UpdateAttributeInfo();

                return _readOnly == XBool.True;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> exists on disk
        /// </summary>
        public bool Exists
        {
            get
            {
                if (_exists == XBool.None)
                    UpdateAttributeInfo();

                return _exists == XBool.True;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether you must lock the <see cref="SvnItem"/> before editting
        /// </summary>
        /// <remarks>Assumes a mustlock file is readonly</remarks>
        public bool ReadOnlyMustLock
        {
            get
            {
                if (_mustLock == XBool.None)
                {
                    _mustLock = XBool.False;

                    if (IsFile && IsReadOnly && IsVersioned)
                    {
                        using (SvnClient client = _context.GetService<ISvnClientPool>().GetNoUIClient())
                        {
                            string propVal;
                            if (client.TryGetProperty(new SvnPathTarget(_fullPath), SvnPropertyNames.SvnNeedsLock, out propVal))
                            {
                                if (propVal == SvnPropertyNames.SvnBooleanValue)
                                    _mustLock = XBool.True;
                            }
                        }
                    }
                }

                return _mustLock == XBool.True;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> is locally locked
        /// </summary>
        /// <returns></returns>
        public bool IsLocked
        {
            get
            {
                EnsureClean();

                return _status.IsLockedLocal;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> is obstructed by an invalid node
        /// </summary>
        public bool IsObstructed
        {
            get
            {
                EnsureClean();

                return _status.LocalContentStatus == SvnStatus.Obstructed;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> is in conflict state
        /// </summary>
        public bool IsConflicted
        {
            get
            {
                EnsureClean();

                return _status.CombinedStatus == SvnStatus.Conflicted;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> is scheduled for delete
        /// </summary>
        public bool IsDeleteScheduled
        {
            get
            {
                EnsureClean();

                return _status.LocalContentStatus == SvnStatus.Deleted;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> is explicitly ignored
        /// </summary>
        public bool IsIgnored
        {
            get
            {
                EnsureClean();

                AnkhStatus status = _status;
                if (GetIsVersioned(status))
                    return false;
                else if (status.LocalContentStatus == SvnStatus.Ignored)
                    return true;

                // Ok. If a folder is ignored everything in it is ignored too

                if (!Exists) // This check is cheap compared to checking the parent
                    return false;

                SvnItem parent = Parent;
                if (parent != null)
                    return parent.IsIgnored;
                else
                    return false;
            }
        }

        /// <summary>
        /// Gets the full path
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FullPath;
        }

        /// <summary>
        /// Retrieves a collection of paths from all provided SvnItems.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static ICollection<string> GetPaths(IEnumerable<SvnItem> items)
		{
			List<String> paths = new List<string>();
			foreach (SvnItem item in items)
			{
				Debug.Assert(item != null, "SvnItem should not be null");

				if (item != null)
				{
					paths.Add(item.FullPath);
				}
			}

			return paths;
		}

        void EnsureClean()
        {
            Debug.Assert(_statusDirty != XBool.None, "Recursive refresh call");

            if (_statusDirty == XBool.True)
                this.RefreshMe();
        }

        public void Dispose()
        {
            // TODO: Mark item as no longer valid
        }

        /// <summary>
        /// Gets the <see cref="SvnItem"/> of this instances parent (the directory it is in)
        /// </summary>
        /// <value>The parent directory or <c>null</c> if this instance is the root directory 
        /// or the cache can not be contacted</value>
        public SvnItem Parent
        {
            get
            {
                string parentDir = Path.GetDirectoryName(FullPath);

                if(string.IsNullOrEmpty(parentDir) || parentDir.Length >= FullPath.Length)
                    return null; // We are the root folder!

                IFileStatusCache cache = _context.GetService<IFileStatusCache>();

                if (cache != null)
                    return cache[parentDir];
                else
                    return null;
            }
        }

        public SvnDirectory ParentDirectory
        {
            get
            {
                string parentDir = Path.GetDirectoryName(FullPath);

                if (string.IsNullOrEmpty(parentDir))
                    return null;

                IFileStatusCache cache = _context.GetService<IFileStatusCache>();

                if (cache == null)
                    return null;
                    
                return cache.GetDirectory(parentDir);
            }
        }

        static class NativeMethods
        {
            /// <summary>
            /// Gets the fileattributes of the specified file without going through the .Net normalization rules
            /// </summary>
            /// <param name="filename"></param>
            /// <returns></returns>
            public static uint GetFileAttributes(string filename)
            {
                if (string.IsNullOrEmpty(filename))
                    throw new ArgumentNullException("filename");

                if (filename.Length < 160)
                    return GetFileAttributesW(filename);
                else
                    return GetFileAttributesW("\\\\?\\" + filename); // Documented method of allowing paths over 160 characters (APR+SharpSvn use this too!)
            }

            [DllImport("kernel32.dll", ExactSpelling = true)]
            extern static uint GetFileAttributesW([MarshalAs(UnmanagedType.LPWStr)]string filename);

            public const uint INVALID_FILE_ATTRIBUTES = 0xFFFFFFFF;
            public const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;
            public const uint FILE_ATTRIBUTE_READONLY = 0x1;
        }

        public bool NewFullPathOk(string fullPath, AnkhStatus status)
        {
            if (fullPath == FullPath)
                return true;
            else if (!GetIsVersioned(status))
                return true;

            return false;
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
