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

namespace Ankh
{
    public interface ISvnItemUpdate
    {
        void RefreshTo(AnkhStatus status);
        void RefreshTo(SvnItem lead);
        void TickItem();
        bool IsItemTicked();
        bool IsStatusClean();
    }

    /// <summary>
    /// Represents a version controlled path on disk, caching its status
    /// </summary>
    public sealed class SvnItem : LocalSvnItem, ISvnItemUpdate
    {
        enum XBool : sbyte
        {
            None = 0, // The three fastest values to check for X86 CPU's
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

            _ticked = false;
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool ISvnItemUpdate.IsItemTicked()
        {
            return _ticked;
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

            if((value & NativeMethods.FILE_ATTRIBUTE_READONLY) != 0)
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
            get { return this._fullPath; }
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
        /// </summsary>
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
                Debug.Assert(_statusDirty == XBool.False, "No longer dirty after refresh");
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
        /// Is this resource modified(implies that it is versioned)?
        /// </summary>
        public bool IsModified
        {
            get
            {
                EnsureClean();
                
                AnkhStatus status = _status;

                SvnStatus t = status.LocalContentStatus;
                SvnStatus p = status.LocalPropertyStatus;
                return GetIsVersioned(status) &&
                    (t != SvnStatus.Normal ||
                      (p != SvnStatus.None && p != SvnStatus.Normal));
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

                    if(IsFile && IsReadOnly && IsVersioned)
                    {
                        using (SvnClient client = _context.GetService<ISvnClientPool>().GetNoUIClient())
                        {
                            string propVal;
                            if (client.TryGetProperty(new SvnPathTarget(_fullPath), SvnPropertyNames.SvnNeedsLock, out propVal))
                            {
                                if(propVal == SvnPropertyNames.SvnBooleanValue)
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
                
                return (_status.LocalContentStatus == SvnStatus.Ignored);
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
        /// Retrieves the paths from an IList of SvnItem instances.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string[] GetPaths(System.Collections.IList items)
        {
            string[] paths = new string[items.Count];
            int i = 0;
            foreach (SvnItem item in items)
            {
                Debug.Assert(item != null, "SvnItem should not be null");

                if (item != null)
                {
                    paths[i++] = item.FullPath;
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

        static class NativeMethods
        {
            /// <summary>
            /// Gets the fileattributes of the specified file without going through the .Net normalization rules
            /// </summary>
            /// <param name="filename"></param>
            /// <returns></returns>
            public static uint GetFileAttributes(string filename)
            {
                if(string.IsNullOrEmpty(filename))
                    throw new ArgumentNullException("filename");

                if(filename.Length < 160)
                    return GetFileAttributesW(filename);
                else
                    return GetFileAttributesW("\\\\?\\" + filename); // Documented method of allowing paths over 160 characters (APR+SharpSvn use this too!)
            }

            [DllImport("kernel32.dll", ExactSpelling=true)]
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
    }
}
