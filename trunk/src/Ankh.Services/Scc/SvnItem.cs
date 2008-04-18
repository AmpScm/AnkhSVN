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
using Ankh.Commands;
using Ankh.Ids;

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

        List<SvnItem> GetUpdateQueueAndClearScheduled();
    }

    /// <summary>
    /// Represents a version controlled path on disk, caching its status
    /// </summary>
    [DebuggerDisplay("Path={FullPath}")]
    public sealed class SvnItem : LocalSvnItem, ISvnItemUpdate
    {
        readonly IAnkhServiceProvider _context;
        readonly string _fullPath;

        enum XBool : sbyte
        {
            None = 0, // The three fastest values to check for most CPU's
            True = -1,
            False = 1
        }

        AnkhStatus _status;

        SvnItemState _currentState;
        SvnItemState _validState;
        SvnItemState _onceValid;
        bool _enqueued;

        static readonly Queue<SvnItem> _stateChanged = new Queue<SvnItem>();
        static bool _scheduled;

        XBool _statusDirty; // updating, dirty, dirty 
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

            _enqueued = true;
            RefreshTo(status);
            _enqueued = false;
        }

        public SvnItem(IAnkhServiceProvider context, string fullPath, AnkhStatus status, SvnNodeKind nodeKind)
            : this(context, fullPath, status)
        {
            switch (nodeKind) // We assume the caller checked this for us
            {
                case SvnNodeKind.File:
                    SetState(SvnItemState.IsDiskFile | SvnItemState.Exists, SvnItemState.None);
                    break;
                case SvnNodeKind.Directory:
                    SetState(SvnItemState.Exists, SvnItemState.IsDiskFile | SvnItemState.ReadOnly | SvnItemState.MustLock);
                    break;
            }
        }

        public SvnItem(IAnkhServiceProvider context, string fullPath, SvnNodeKind nodeKind)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentNullException("fullPath");

            _statusDirty = XBool.True;
            _context = context;
            _fullPath = fullPath;

            switch (nodeKind) // We assume the caller checked this for us
            {
                case SvnNodeKind.File:
                    SetState(SvnItemState.IsDiskFile | SvnItemState.Exists, SvnItemState.None);
                    break;
                case SvnNodeKind.Directory:
                    SetState(SvnItemState.Exists, SvnItemState.IsDiskFile | SvnItemState.ReadOnly | SvnItemState.MustLock);
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

                SetState(SvnItemState.Exists | SvnItemState.Versionable,
                            SvnItemState.IsDiskFile | SvnItemState.ReadOnly | SvnItemState.MustLock);

                if (_statusDirty != XBool.False)
                    _statusDirty = XBool.True; // Walk the path itself to get the data you want

                return;
            }
            else if (
                (((status.LocalContentStatus == SvnStatus.NotVersioned) && status != AnkhStatus.NotVersioned)
                || (status.LocalContentStatus == SvnStatus.Ignored)) && IsDirectory)
            {
                // A not versioned directory might be a working copy by itself!

                if (_statusDirty == XBool.False)
                    return; // No need to remove valid cache entries

                if (SvnTools.IsManagedPath(FullPath))
                {
                    _statusDirty = XBool.True; // Walk the path itself to get the data

                    // Extract usefull information we got anyay

                    SetState(SvnItemState.Exists | SvnItemState.Versionable,
                                SvnItemState.IsDiskFile | SvnItemState.ReadOnly | SvnItemState.MustLock);

                    return;
                }
                // Fall through
            }

            _cookie = NextCookie();
            _statusDirty = XBool.False;
            _status = status;


            const SvnItemState unset = SvnItemState.Modified | SvnItemState.Added |
                SvnItemState.HasCopyOrigin | SvnItemState.Deleted | SvnItemState.ContentConflicted | SvnItemState.PropertiesConflicted | SvnItemState.InTreeConflict | SvnItemState.Ignored | SvnItemState.Obstructed;

            const SvnItemState managed = SvnItemState.Versionable | SvnItemState.Versioned;

            // Let's assume status is more recent than our internal property cache
            // Set all caching properties we can

            switch (status.LocalContentStatus)
            {
                case SvnStatus.None:
                    SetState(SvnItemState.None, SvnItemState.Exists);
                    break;
                case SvnStatus.NotVersioned:
                    // Node exists but is not managed by us in this directory
                    // (Might be from an other location as in the nested case)
                    SetState(SvnItemState.Exists, unset | managed);
                    break;
                case SvnStatus.Ignored:
                    // Node exists but is not managed by us in this directory
                    // (Might be from an other location as in the nested case)
                    SetState(SvnItemState.Exists | SvnItemState.Ignored, (unset & ~SvnItemState.Ignored) | managed);
                    break;
                case SvnStatus.Normal:
                    SetState(managed | SvnItemState.Exists, unset);
                    break;
                case SvnStatus.Modified:
                    SetState(managed | SvnItemState.Exists | SvnItemState.Modified, unset);
                    break;
                case SvnStatus.Added:
                    if (status.IsCopied)
                        SetState(managed | SvnItemState.Exists | SvnItemState.Added | SvnItemState.HasCopyOrigin, unset);
                    else
                        SetState(managed | SvnItemState.Exists | SvnItemState.Added, unset);
                    break;
                case SvnStatus.Replaced:
                    if (status.IsCopied)
                        SetState(managed | SvnItemState.Exists | SvnItemState.Replaced | SvnItemState.HasCopyOrigin, unset);
                    else
                        SetState(managed | SvnItemState.Exists | SvnItemState.Replaced, unset);
                    break;
                case SvnStatus.Conflicted:
                    SetState(managed | SvnItemState.Exists | SvnItemState.ContentConflicted, unset);
                    break;
                case SvnStatus.Obstructed: // node exists but is of the wrong type
                    SetState(SvnItemState.Exists, SvnItemState.None);
                    SetDirty(SvnItemState.ReadOnly | SvnItemState.IsDiskFile | SvnItemState.MustLock);
                    return;

                case SvnStatus.Missing:
                    SetState(managed, unset | SvnItemState.Exists);
                    break;
                case SvnStatus.Deleted:
                    // We don't unsed exists here; as the file may still exist
                    SetState(managed | SvnItemState.Deleted, unset);
                    break;
                case SvnStatus.External:
                    // Should be handled above
                    throw new InvalidOperationException();
                case SvnStatus.Incomplete:
                    break;
                default:
                    Trace.WriteLine(string.Format("Ignoring undefined status {0} in SvnItem.Refresh()", status.LocalContentStatus));
                    break;
            }

            switch (status.LocalPropertyStatus)
            {
                case SvnStatus.None:
                    SetState(SvnItemState.None,
                             SvnItemState.MustLock | SvnItemState.PropertiesConflicted | SvnItemState.PropertyModified);
                    break;
                case SvnStatus.Normal:
                    SetState(SvnItemState.None,
                             SvnItemState.PropertiesConflicted | SvnItemState.PropertyModified);
                    break;
                case SvnStatus.Modified:
                    SetState(SvnItemState.PropertyModified,
                             SvnItemState.PropertiesConflicted);
                    break;
                case SvnStatus.Conflicted:
                    SetState(SvnItemState.PropertyModified | SvnItemState.PropertiesConflicted,
                             SvnItemState.None);
                    break;
            }

            switch (status.NodeKind)
            {
                case SvnNodeKind.Directory:
                    SetState(SvnItemState.None | SvnItemState.Versioned | SvnItemState.Versionable,
                                SvnItemState.IsDiskFile | SvnItemState.ReadOnly | SvnItemState.MustLock);
                    break;
                case SvnNodeKind.File:
                    SetState(SvnItemState.IsDiskFile, SvnItemState.None);
                    break;
            }

            if (status.IsLockedLocal)
                SetState(SvnItemState.HasLockToken, SvnItemState.None);
            else
                SetState(SvnItemState.None, SvnItemState.HasLockToken);
        }

        private void SetDirty(SvnItemState p)
        {
            _validState &= ~p;
        }

        void SetState(SvnItemState set, SvnItemState unset)
        {
            SvnItemState st = (_currentState & ~unset) | set;
            _validState |= (set | unset);

            if (st != _currentState)
            {
                // Calculate whether we have a change or just new information
                bool changed = (st & _onceValid) != _currentState;
                _currentState = st;

                if (changed)
                {
                    if (!_enqueued)
                    {
                        _enqueued = true;

                        // Schedule a stat changed broadcast
                        lock (_stateChanged)
                        {
                            _stateChanged.Enqueue(this);

                            ScheduleUpdateNotify();
                        }
                    }
                }
            }
            _onceValid |= _validState;
        }

        void ScheduleUpdateNotify()
        {
            if (_scheduled)
                return;

            IAnkhCommandService cs = _context.GetService<IAnkhCommandService>();

            if (cs != null)
                _scheduled = cs.PostExecCommand(AnkhCommand.TickRefreshSvnItems);
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

            SetState(lead._currentState, ~lead._currentState);
            _ticked = false;
            _cookie = NextCookie(); // Status 100% the same, but changed... Cookies are free ;)
        }

        public void MarkDirty()
        {
            Debug.Assert(_statusDirty != XBool.None, "MarkDirty called while updating status");

            _statusDirty = XBool.True;

            _validState = SvnItemState.None;
            _cookie = NextCookie();
        }

        void ISvnItemUpdate.MarkStatusDirty()
        {
            _statusDirty = XBool.True;
            _validState &= ~SvnItemState.MaskSvnState;
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

                SetState(SvnItemState.IsDiskFile, // File allows more optimizations in the svn case
                    SvnItemState.Exists | SvnItemState.ReadOnly | SvnItemState.MustLock | SvnItemState.Versionable);

                return;
            }

            SvnItemState set = SvnItemState.Exists;
            SvnItemState unset = SvnItemState.None;

            if ((value & NativeMethods.FILE_ATTRIBUTE_READONLY) != 0)
                set = SvnItemState.ReadOnly;
            else
                unset = SvnItemState.ReadOnly;

            if ((value & NativeMethods.FILE_ATTRIBUTE_DIRECTORY) != 0)
            {
                unset = SvnItemState.IsDiskFile | SvnItemState.ReadOnly;
                set &= ~SvnItemState.ReadOnly;
            }
            else
                set = SvnItemState.IsDiskFile;

            SetState(set, unset);
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

        public SvnItemState GetState(SvnItemState flagsToGet)
        {
            SvnItemState unavailable = flagsToGet & ~_validState;

            if (unavailable == 0)
                return _currentState & flagsToGet; // We have everything we need

            if (0 != (unavailable & SvnItemState.MaskSvnStatusOnly))
            {
                Debug.Assert(_statusDirty != XBool.False);
                RefreshMe();

                unavailable = flagsToGet & ~_validState;
            }

            if (0 != (unavailable & SvnItemState.MaskGetAttributes))
            {
                UpdateAttributeInfo();

                unavailable = flagsToGet & ~_validState;
            }

/*            if (0 != (unavailable & (SvnItemState.OpenDocument | SvnItemState.DirtyDocument)))
            {
                UpdateDocumentInfo();

                unavailable = flagsToGet & ~_validState;
            }*/

            if (0 != (unavailable & SvnItemState.Versionable))
            {
                CheckVersionable();

                unavailable = flagsToGet & ~_validState;
            }

            if (0 != (unavailable & SvnItemState.MustLock))
            {
                CheckMustLock();

                unavailable = flagsToGet & ~_validState;
            }

            if (unavailable != 0)
            {
                Trace.WriteLine(string.Format("Don't know how to retrieve {0:X} state; clearing dirty flag", (int)unavailable));

                _validState |= unavailable;
            }

            return _currentState & flagsToGet;
        }

        private void CheckVersionable()
        {
            bool versionable;

            SvnItemState state;

            if (TryGetState(SvnItemState.Versioned, out state) && state != 0)
                versionable = true;
            else if (Exists && SvnTools.IsBelowManagedPath(FullPath)) // Will call GetState again!
                versionable = true;
            else
                versionable = false;

            if (versionable)
                SetState(SvnItemState.Versionable, SvnItemState.None);
            else
                SetState(SvnItemState.None, SvnItemState.Versionable);
        }

        /*void UpdateDocumentInfo()
        {
            IAnkhOpenDocumentTracker dt = _context.GetService<IAnkhOpenDocumentTracker>();

            if (dt == null)
            {
                // We /must/ make the state not dirty
                SetState(SvnItemState.None, SvnItemState.DirtyDocument | SvnItemState.OpenDocument);
                return;
            }

            if (dt.IsDocumentDirty(FullPath))
                SetState(SvnItemState.OpenDocument | SvnItemState.DirtyDocument, SvnItemState.None);
            else if (dt.IsDocumentOpen(FullPath))
                SetState(SvnItemState.OpenDocument, SvnItemState.DirtyDocument);
            else
                SetState(SvnItemState.None, SvnItemState.DirtyDocument | SvnItemState.OpenDocument);
        }*/

        bool TryGetState(SvnItemState mask, out SvnItemState result)
        {
            if ((mask & _validState) != mask)
            {
                result = SvnItemState.None;
                return false;
            }

            result = _currentState & mask;
            return true;
        }

        void CheckMustLock()
        {
            SvnItemState value = SvnItemState.IsDiskFile | SvnItemState.ReadOnly | SvnItemState.Versioned;

            bool mustLock;

            if (GetState(value) != value)
                mustLock = false;
            else if (TryGetState(SvnItemState.HasProperties, out value) && value == 0)
                mustLock = false; // File has no properties
            else
            {
                using (SvnClient client = _context.GetService<ISvnClientPool>().GetNoUIClient())
                {
                    string propVal;

                    if (client.TryGetProperty(new SvnPathTarget(_fullPath), SvnPropertyNames.SvnNeedsLock, out propVal))
                    {
                        mustLock = (propVal == SvnPropertyNames.SvnBooleanValue);
                    }
                    else
                        mustLock = false;
                }
            }

            if (mustLock)
                SetState(SvnItemState.MustLock, SvnItemState.None);
            else
                SetState(SvnItemState.None, SvnItemState.MustLock);
        }

        /// <summary>
        /// Gets a boolean indicating whether the item (on disk) is a file
        /// </summary>
        /// <remarks>Use <see cref="Status"/>.<see cref="AnkhStatus.SvnNodeKind"/> to retrieve the svn type</remarks>
        public bool IsFile
        {
            get { return GetState(SvnItemState.IsDiskFile) == SvnItemState.IsDiskFile; }
        }

        /// <summary>
        /// Gets a boolean indicating whether the item (on disk) is a directory
        /// </summary>
        /// <remarks>Use <see cref="Status"/>.<see cref="AnkhStatus.SvnNodeKind"/> to retrieve the svn type</remarks>
        public bool IsDirectory
        {
            get { return GetState(SvnItemState.IsDiskFile) == 0; }
        }

        public bool HasCopyableHistory
        {
            get
            {
                if (!IsVersioned)
                    return false;

                if (GetState(SvnItemState.Added) != 0)
                    return GetState(SvnItemState.HasCopyOrigin) != 0;
                else
                    return true;
            }
        }

        /// <summary>
        /// Gets the <see cref="SvnNodeKind"/> of the item (on disk)
        /// </summary>
        /// <value><see cref="SvnNodeKind.Directory"/> if <see cref="IsDirectory"/> otherwise <see cref="SvnNodeKind.File"/></value>
        public SvnNodeKind NodeKind
        {
            get { return (GetState(SvnItemState.IsDiskFile) == 0) ? SvnNodeKind.Directory : SvnNodeKind.File; }
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
            get { return 0 != GetState(SvnItemState.Versioned); }
        }

        /// <summary>
        /// Is this resource modified; implies the item is versioned
        /// </summary>
        public bool IsModified
        {
            get { return 0 != GetState(SvnItemState.Modified); }
        }

        /// <summary>
        /// Whether the item is potentially versionable.
        /// </summary>
        public bool IsVersionable
        {
            get { return GetState(SvnItemState.Versionable) != 0; }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> specifies a readonly file
        /// </summary>
        public bool IsReadOnly
        {
            get { return GetState(SvnItemState.ReadOnly) != 0; }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> exists on disk
        /// </summary>
        public bool Exists
        {
            get { return GetState(SvnItemState.Exists) != 0; }
        }

        /// <summary>
        /// Gets a boolean indicating whether you must lock the <see cref="SvnItem"/> before editting
        /// </summary>
        /// <remarks>Assumes a mustlock file is readonly</remarks>
        public bool ReadOnlyMustLock
        {
            get
            {
                // Pass all required values to calculate to optimize the query
                return (GetState(SvnItemState.MustLock | SvnItemState.ReadOnly | SvnItemState.Versioned) & SvnItemState.MustLock) != 0;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> is locally locked
        /// </summary>
        /// <returns></returns>
        public bool IsLocked
        {
            get { return GetState(SvnItemState.HasLockToken) != 0; }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> is obstructed by an invalid node
        /// </summary>
        public bool IsObstructed
        {
            get { return GetState(SvnItemState.Obstructed) != 0; }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> is in conflict state
        /// </summary>
        public bool IsConflicted
        {
            get { return 0 != GetState(SvnItemState.ContentConflicted | SvnItemState.PropertiesConflicted | SvnItemState.InTreeConflict); }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> is scheduled for delete
        /// </summary>
        public bool IsDeleteScheduled
        {
            get { return 0 != GetState(SvnItemState.Deleted); }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> is explicitly ignored
        /// </summary>
        public bool IsIgnored
        {
            get
            {
                if (GetState(SvnItemState.Ignored) != 0)
                    return true;
                else if (IsVersioned)
                    return false;
                else if (!Exists)
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

                if (string.IsNullOrEmpty(parentDir) || parentDir.Length >= FullPath.Length)
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
                    return false;
                default:
                    return true;
            }
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

        List<SvnItem> ISvnItemUpdate.GetUpdateQueueAndClearScheduled()
        {
            lock (_stateChanged)
            {
                _scheduled = false;

                if(_stateChanged.Count == 0)
                    return null;

                List<SvnItem> modified = new List<SvnItem>(_stateChanged.Count);
                modified.AddRange(_stateChanged);
                _stateChanged.Clear();

                foreach (SvnItem i in modified)
                    i._enqueued = false;

                
                return modified;
            }
        }                
    }
}
