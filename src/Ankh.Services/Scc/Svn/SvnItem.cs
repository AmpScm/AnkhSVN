// $Id$
//
// Copyright 2004-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

using SharpSvn;

using Ankh.Commands;
using Ankh.Scc;
using Ankh.Selection;
using System.Text;

namespace Ankh.Scc
{
    public interface ISvnItemUpdate
    {
        void RefreshTo(SvnStatusData status);
        void RefreshTo(NoSccStatus status, SvnNodeKind nodeKind);
        void RefreshTo(SvnItem lead);
        void TickItem();
        void UntickItem();
        bool IsItemTicked();
        bool ShouldRefresh();
        bool IsStatusClean();

        bool ShouldClean();

        void SetState(SvnItemState set, SvnItemState unset);
        void SetDirty(SvnItemState dirty);
        bool TryGetState(SvnItemState get, out SvnItemState value);
    }
}

namespace Ankh
{
    /// <summary>
    /// Represents a version controlled path on disk, caching its status
    /// </summary>
    [DebuggerDisplay("Path={FullPath}")]
    public sealed partial class SvnItem : Ankh.Scc.Engine.SccItem<SvnItem>, ISvnItemUpdate, ISvnWcReference, IEquatable<SvnItem>
    {
        readonly ISvnStatusCache _context;

        SvnStatusData _status;
        bool _enqueued;

        static readonly Queue<SvnItem> _stateChanged = new Queue<SvnItem>();
        static bool _scheduled;

        ISvnWcReference _workingCopy;
        XBool _statusDirty; // updating, dirty, dirty 
        bool _ticked;
        int _cookie;
        DateTime _modified;
        bool _sccExcluded;

        public SvnItem(ISvnStatusCache context, string fullPath, SvnStatusData status)
            : base(fullPath)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (status == null)
                throw new ArgumentNullException("status");

            _context = context;
            _status = status;

            _enqueued = true;
            RefreshTo(status);
            _enqueued = false;
        }

        public SvnItem(ISvnStatusCache context, string fullPath, NoSccStatus status, SvnNodeKind nodeKind)
            : base(fullPath)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            _enqueued = true;
            RefreshTo(status, nodeKind);
            _enqueued = false;
        }

        void InitializeFromKind(SvnNodeKind nodeKind)
        {
            switch (nodeKind) // We assume the caller checked this for us
            {
                case SvnNodeKind.File:
                    SetState(SvnItemState.IsDiskFile | SvnItemState.Exists, SvnItemState.IsDiskFolder);
                    break;
                case SvnNodeKind.Directory:
                    SetState(SvnItemState.IsDiskFolder | SvnItemState.Exists, SvnItemState.IsDiskFile | SvnItemState.ReadOnly | SvnItemState.MustLock);
                    break;
            }
        }

        ISvnStatusCache StatusCache
        {
            [DebuggerStepThrough]
            get { return _context; }
        }

        void RefreshTo(NoSccStatus status, SvnNodeKind nodeKind)
        {
            _cookie = NextCookie();
            _statusDirty = XBool.False;

            SvnItemState set = SvnItemState.None;
            SvnItemState unset = SvnItemState.Modified | SvnItemState.Added | SvnItemState.HasCopyOrigin
                | SvnItemState.Deleted | SvnItemState.ContentConflicted | SvnItemState.Ignored
                | SvnItemState.Obstructed | SvnItemState.Replaced | SvnItemState.Versioned
                | SvnItemState.SvnDirty | SvnItemState.PropertyModified | SvnItemState.PropertiesConflicted | SvnItemState.Conflicted
                | SvnItemState.Obstructed | SvnItemState.MustLock | SvnItemState.IsWCRoot
                | SvnItemState.HasProperties | SvnItemState.HasLockToken | SvnItemState.HasCopyOrigin
                | SvnItemState.MovedHere;

            switch (status)
            {
                case NoSccStatus.NotExisting:
                    SetState(set, SvnItemState.Exists | SvnItemState.ReadOnly | SvnItemState.IsDiskFile | SvnItemState.IsDiskFolder | SvnItemState.Versionable | unset);
                    _status = SvnStatusData.NotExisting;
                    break;
                case NoSccStatus.NotVersionable:
                    unset |= SvnItemState.Versionable;
                    goto case NoSccStatus.NotVersioned; // fall through
                case NoSccStatus.NotVersioned:
                    SetState(SvnItemState.Exists | set, SvnItemState.None | unset);
                    _status = SvnStatusData.NotVersioned;
                    break;
                case NoSccStatus.Unknown:
                default:
                    SetDirty(set | unset);
                    _statusDirty = XBool.True;
                    break;
            }

            InitializeFromKind(nodeKind);
        }

        void ISvnItemUpdate.RefreshTo(NoSccStatus status, SvnNodeKind nodeKind)
        {
            Debug.Assert(status != NoSccStatus.Unknown);
            _ticked = false;
            RefreshTo(status, nodeKind);
        }

        void RefreshTo(SvnStatusData status)
        {
            if (status == null)
                throw new ArgumentNullException("status");

            if (status.LocalNodeStatus == SvnStatus.External)
            {
                // When iterating the status of an external in it's parent directory
                // We get an external status and no really usefull information

                SetState(SvnItemState.Exists | SvnItemState.Versionable | SvnItemState.IsDiskFolder,
                            SvnItemState.IsDiskFile | SvnItemState.ReadOnly | SvnItemState.MustLock | SvnItemState.IsTextFile);

                if (_statusDirty != XBool.False)
                    _statusDirty = XBool.True; // Walk the path itself to get the data you want

                return;
            }
            else if (MightBeNestedWorkingCopy(status) && IsDirectory)
            {
                // A not versioned directory might be a working copy by itself!

                if (_statusDirty == XBool.False)
                    return; // No need to remove valid cache entries

                if (SvnTools.IsManagedPath(FullPath))
                {
                    _statusDirty = XBool.True; // Walk the path itself to get the data

                    // Extract useful information we got anyway

                    SetState(SvnItemState.Exists | SvnItemState.Versionable | SvnItemState.Versioned | SvnItemState.IsWCRoot | SvnItemState.IsDiskFolder,
                             SvnItemState.IsDiskFile | SvnItemState.ReadOnly | SvnItemState.MustLock | SvnItemState.IsTextFile);

                    return;
                }
                else
                {
                    SetState(SvnItemState.None, SvnItemState.IsWCRoot);
                }
                // Fall through
            }

            _cookie = NextCookie();
            _statusDirty = XBool.False;
            _status = status;

            const SvnItemState unset = SvnItemState.Modified | SvnItemState.Added |
                SvnItemState.HasCopyOrigin | SvnItemState.Deleted | SvnItemState.ContentConflicted |
                SvnItemState.Ignored | SvnItemState.Obstructed | SvnItemState.Replaced |
                SvnItemState.MovedHere;

            const SvnItemState managed = SvnItemState.Versioned;


            // Let's assume status is more recent than our internal property cache
            // Set all caching properties we can

            bool svnDirty = true;
            bool exists = true;
            bool provideDiskInfo = true;
            switch (status.LocalNodeStatus)
            {
                case SvnStatus.None:
                    SetState(SvnItemState.None, managed | unset);
                    svnDirty = false;
                    exists = false;
                    provideDiskInfo = false;
                    break;
                case SvnStatus.NotVersioned:
                    // Node exists but is not managed by us in this directory
                    // (Might be from an other location as in the nested case)
                    SetState(SvnItemState.None, unset | managed);
                    svnDirty = false;
                    break;
                case SvnStatus.Ignored:
                    // Node exists but is not managed by us in this directory
                    // (Might be from an other location as in the nested case)
                    SetState(SvnItemState.Ignored, unset | managed);
                    svnDirty = false;
                    break;
                case SvnStatus.Added:
                    if (status.IsMoved && status.IsCopied)
                        SetState(managed | SvnItemState.Added | SvnItemState.HasCopyOrigin | SvnItemState.MovedHere, unset);
                    else if (status.IsCopied)
                        SetState(managed | SvnItemState.Added | SvnItemState.HasCopyOrigin, unset);
                    else
                        SetState(managed | SvnItemState.Added, unset);

                    if (status.LocalTextStatus == SvnStatus.Modified)
                        SetState(SvnItemState.Modified, SvnItemState.None);
                    else if (status.LocalTextStatus == SvnStatus.Normal)
                        SetState(SvnItemState.None, SvnItemState.Modified);
                    break;
                case SvnStatus.Replaced:
                    if (status.IsMoved && status.IsCopied)
                        SetState(managed | SvnItemState.Replaced | SvnItemState.HasCopyOrigin | SvnItemState.MovedHere, unset);
                    else if (status.IsCopied)
                        SetState(managed | SvnItemState.Replaced | SvnItemState.HasCopyOrigin, unset);
                    else
                        SetState(managed | SvnItemState.Replaced, unset);

                    if (status.LocalTextStatus == SvnStatus.Modified)
                        SetState(SvnItemState.Modified, SvnItemState.None);
                    else if (status.LocalTextStatus == SvnStatus.Normal)
                        SetState(SvnItemState.None, SvnItemState.Modified);
                    break;
                case SvnStatus.Modified:
                case SvnStatus.Conflicted:
                    {
                        bool done = false;
                        switch (status.LocalTextStatus)
                        {
                            case SvnStatus.Modified:
                                SetState(managed | SvnItemState.Modified, unset);
                                done = true;
                                break;
                            case SvnStatus.Conflicted:
                                SetState(managed | SvnItemState.ContentConflicted | SvnItemState.Conflicted, unset);
                                done = true;
                                break;
                        }
                        if (!done)
                            goto case SvnStatus.Normal;
                        break;
                    }
                case SvnStatus.Obstructed: // node exists but is of the wrong type
                    SetState(SvnItemState.None, managed | unset);
                    provideDiskInfo = false; // Info is wrong
                    break;
                case SvnStatus.Missing:
                    exists = false;
                    provideDiskInfo = false; // Info is wrong
                    SetState(managed, unset);
                    break;
                case SvnStatus.Deleted:
                    SetState(managed | SvnItemState.Deleted, unset);
                    if (status.LocalFileExists)
                        exists = provideDiskInfo = true;
                    else
                    {
                        exists = false;
                        provideDiskInfo = false; // Folder might still exist
                    }
                    break;
                case SvnStatus.External:
                    // Should be handled above
                    throw new InvalidOperationException();
                case SvnStatus.Incomplete:
                    SetState(managed, unset);
                    break;
                default:
                    Trace.WriteLine(string.Format("Ignoring undefined status {0} in SvnItem.Refresh()", status.LocalNodeStatus));
                    provideDiskInfo = false; // Can't trust an unknown status
                    goto case SvnStatus.Normal;
                case SvnStatus.Normal:
                    SetState(managed | SvnItemState.Exists, unset);
                    svnDirty = false;
                    break;
            }

            if (exists)
                SetState(SvnItemState.Versionable, SvnItemState.None);
            else
                SetState(SvnItemState.None, SvnItemState.Versionable);

            if (status.Conflicted)
                SetState(SvnItemState.Conflicted, SvnItemState.None);
            else
                SetState(SvnItemState.None, SvnItemState.Conflicted);


            bool hasProperties = true;
            switch (status.LocalPropertyStatus)
            {
                case SvnStatus.None:
                    hasProperties = false;
                    SetState(SvnItemState.None, SvnItemState.PropertiesConflicted | SvnItemState.PropertyModified | SvnItemState.HasProperties);
                    break;
                case SvnStatus.Modified:
                    SetState(SvnItemState.PropertyModified | SvnItemState.HasProperties,
                             SvnItemState.PropertiesConflicted);
                    svnDirty = true;
                    break;
                case SvnStatus.Conflicted:
                    SetState(SvnItemState.PropertyModified | SvnItemState.PropertiesConflicted | SvnItemState.HasProperties,
                             SvnItemState.None);
                    svnDirty = true;
                    break;
                case SvnStatus.Normal:
                default:
                    SetState(SvnItemState.HasProperties,
                             SvnItemState.PropertiesConflicted | SvnItemState.PropertyModified);
                    break;
            }

            if (svnDirty)
                SetState(SvnItemState.SvnDirty, SvnItemState.None);
            else
                SetState(SvnItemState.None, SvnItemState.SvnDirty);

            if (!hasProperties)
                SetState(SvnItemState.None, SvnItemState.MustLock);

            if (provideDiskInfo)
            {
                if (exists) // Behaviour must match updating from UpdateAttributeInfo()
                    switch (status.NodeKind)
                    {
                        case SvnNodeKind.Directory:
                            SetState(SvnItemState.IsDiskFolder | SvnItemState.Exists, SvnItemState.ReadOnly | SvnItemState.MustLock | SvnItemState.IsTextFile | SvnItemState.IsDiskFile);
                            break;
                        case SvnNodeKind.File:
                            SetState(SvnItemState.IsDiskFile | SvnItemState.Exists, SvnItemState.IsDiskFolder);
                            break;
                        default:
                            // Handle direct replacement without an additional stat
                            if (status.LocalFileExists)
                                goto case SvnNodeKind.File;
                            break;
                    }
                else
                    SetState(SvnItemState.None, SvnItemState.Exists);
            }

            if (status.IsLockedLocal)
                SetState(SvnItemState.HasLockToken, SvnItemState.None);
            else
                SetState(SvnItemState.None, SvnItemState.HasLockToken);
        }

        static bool MightBeNestedWorkingCopy(SvnStatusData status)
        {
            switch (status.LocalNodeStatus)
            {
                case SvnStatus.NotVersioned:
                case SvnStatus.Ignored:
                case SvnStatus.Obstructed:
                    return true;

                default:
                    return false;
            }
        }

        void ScheduleUpdateNotify()
        {
            if (_scheduled)
                return;

            IAnkhCommandService cs = _context.GetService<IAnkhCommandService>();

            if (cs != null)
                cs.PostTickCommand(ref _scheduled, AnkhCommand.TickRefreshSvnItems);
        }

        void ISvnItemUpdate.RefreshTo(SvnStatusData status)
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
            if (lead == null)
                throw new ArgumentNullException("lead");
            else if (lead._status == null)
                throw new InvalidOperationException("Lead status = null");

            _status = lead._status;
            _statusDirty = lead._statusDirty;

            SvnItemState current = lead._currentState;
            SvnItemState valid = lead._validState;

            SetState(current & valid, (~current) & valid);
            _ticked = false;
            _modified = lead._modified;
            _cookie = NextCookie(); // Status 100% the same, but changed... Cookies are free ;)
        }

        public void MarkDirty()
        {
            Debug.Assert(_statusDirty != XBool.None, "MarkDirty called while updating status");

            _statusDirty = XBool.True;

            _validState = SvnItemState.None;
            _cookie = NextCookie();
            _workingCopy = null;
            _modified = new DateTime();
            _conflicts = null;
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
        void ISvnItemUpdate.UntickItem()
        {
            _ticked = false;
        }

        public bool ShouldClean()
        {
            return _ticked || (_statusDirty == XBool.False && _status == SvnStatusData.NotExisting);
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

        /// <summary>
        /// Gets the (cached) modification time of the file/directory
        /// </summary>
        /// <value>The modified.</value>
        public DateTime Modified
        {
            get
            {
                if (_modified.Ticks == 0 && Exists)
                {
                    try
                    {
                        _modified = File.GetLastWriteTimeUtc(FullPath);
                    }
                    catch { }
                }
                return _modified;
            }
        }

        /// <summary>
        /// Gets the SVN status of the item; retrieves a placeholder if the status is unknown
        /// </summary>
        public SvnStatusData Status
        {
            get
            {
                EnsureClean();

                return _status;
            }
        }

        /// <summary>
        /// Gets the node kind of the file in subversion
        /// </summary>
        public override SvnNodeKind NodeKind
        {
            get { return Status.NodeKind; }
        }

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

        /// <summary>
        /// Gets a boolean indicating whether the item (on disk) is a file
        /// </summary>
        /// <remarks>Use <see cref="Status"/>.<see cref="SvnStatusData.SvnNodeKind"/> to retrieve the svn type</remarks>
        public override bool IsFile
        {
            get { return GetState(SvnItemState.IsDiskFile) == SvnItemState.IsDiskFile; }
        }

        /// <summary>
        /// Gets a boolean indicating whether the item (on disk) is a directory
        /// </summary>
        /// <remarks>Use <see cref="Status"/>.<see cref="SvnStatusData.SvnNodeKind"/> to retrieve the svn type</remarks>
        public override bool IsDirectory
        {
            get { return GetState(SvnItemState.IsDiskFolder) == SvnItemState.IsDiskFolder; }
        }

        public bool IsMoved
        {
            get { return GetState(SvnItemState.MovedHere) == SvnItemState.MovedHere; }
        }


        /// <summary>
        /// Gets a boolean indicating wether a copy of this file has history
        /// </summary>
        public bool HasCopyableHistory
        {
            get
            {
                if (!IsVersioned)
                    return false;

                if (GetState(SvnItemState.Added | SvnItemState.Replaced) != 0)
                    return GetState(SvnItemState.HasCopyOrigin) != 0;
                else
                    return true;
            }
        }

        void RefreshStatus()
        {
            _statusDirty = XBool.None;
            ISvnStatusCache statusCache = StatusCache;

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
        public override bool IsVersioned
        {
            get { return 0 != GetState(SvnItemState.Versioned); }
        }

        /// <summary>
        /// Is this resource modified; implies the item is versioned
        /// </summary>
        public override bool IsModified
        {
            get
            {
                return GetState(SvnItemState.SvnDirty) != 0;
            }
        }

        public bool IsPropertyModified
        {
            get { return GetState(SvnItemState.PropertyModified) != 0; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is diff available.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is diff available; otherwise, <c>false</c>.
        /// </value>
        public bool IsLocalDiffAvailable
        {
            get
            {
                if (!IsFile || !IsVersioned)
                    return false;
                if (!IsModified)
                    return IsDocumentDirty;

                switch (Status.LocalNodeStatus)
                {
                    case SvnStatus.Normal:
                        // Probably property modified
                        return IsDocumentDirty;
                    case SvnStatus.Added:
                    case SvnStatus.Replaced:
                        return HasCopyableHistory && ((Status.LocalTextStatus != SvnStatus.Normal) || IsDocumentDirty);
                    case SvnStatus.Deleted:
                        // To be replaced
                        return Exists;
                    case SvnStatus.NotVersioned:
                        return false;
                    default: // Includes Conflicted
                        return true;
                }
            }
        }

        /// <summary>
        /// Whether the item is potentially versionable.
        /// </summary>
        public override bool IsVersionable
        {
            get { return GetState(SvnItemState.Versionable) != 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the item is (inside) the administrative area.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is administrative area; otherwise, <c>false</c>.
        /// </value>
        public bool IsAdministrativeArea
        {
            get { return GetState(SvnItemState.IsAdministrativeArea) != 0; }
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
        public override bool Exists
        {
            get { return GetState(SvnItemState.Exists) != 0; }
        }

        /// <summary>
        /// Gets a boolean indicating whether you must lock the <see cref="SvnItem"/> before editing
        /// </summary>
        /// <remarks>Assumes a mustlock file is readonly to speed up testing</remarks>
        public bool IsReadOnlyMustLock
        {
            get { return GetState(SvnItemState.MustLock) != 0; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsAdded
        {
            get { return GetState(SvnItemState.Added) != 0; }
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
        /// Gets a value indicating whether this instance is a managed binary file.
        /// </summary>
        public bool IsTextFile
        {
            get { return GetState(SvnItemState.IsTextFile) != 0; }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> is in conflict state
        /// </summary>
        public bool IsConflicted
        {
            get { return 0 != GetState(SvnItemState.Conflicted); }
        }

        /// <summary>
        /// Gets a value indicating whether this node is tree conflicted.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is tree conflicted; otherwise, <c>false</c>.
        /// </value>
        public bool IsTreeConflicted
        {
            get
            {
                if (GetState(SvnItemState.Conflicted) != 0)
                {
                    foreach (SvnConflictData cd in Conflicts)
                    {
                        if (cd.ConflictType == SvnConflictType.Tree)
                            return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is casing conflicted.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is casing conflicted; otherwise, <c>false</c>.
        /// </value>
        public bool IsCasingConflicted
        {
            get { return IsVersioned && Status.LocalNodeStatus == SvnStatus.Missing && Status.NodeKind == SvnNodeKind.File && IsFile && Exists; }
        }

        private void StoreConflicts(object sender, SvnInfoEventArgs e)
        {
            if (e.Conflicts != null && e.Conflicts.Count > 0)
            {
                _conflicts = e.Conflicts;

                foreach (SvnConflictData d in _conflicts)
                {
                    d.Detach();
                }
            }
        }

        IEnumerable<SvnConflictData> _conflicts;
        /// <summary>
        /// Gets the list of conflicts for this node. An empty list if not conflicted.
        /// </summary>
        public IEnumerable<SvnConflictData> Conflicts
        {
            get
            {
                if (_conflicts != null)
                    return _conflicts;

                SvnConflictData[] empty = new SvnConflictData[0];
                _conflicts = empty;
                SvnItemState state;
                if (TryGetState(SvnItemState.Conflicted, out state) && state == 0)
                    return _conflicts;

                using (SvnClient cl = _context.GetService<ISvnClientPool>().GetNoUIClient())
                {
                    SvnInfoArgs ia = new SvnInfoArgs();
                    ia.ThrowOnError = false;
                    ia.Depth = SvnDepth.Empty;

                    cl.Info(FullPath, ia, StoreConflicts); // Ignore errors
                }

                if (_conflicts != empty)
                {
                    SetState(SvnItemState.Conflicted, SvnItemState.None);
                    foreach (SvnConflictData cd in _conflicts)
                    {
                        switch (cd.ConflictType)
                        {
                            case SvnConflictType.Content:
                                SetState(SvnItemState.ContentConflicted, SvnItemState.None);
                                break;
                            case SvnConflictType.Property:
                                SetState(SvnItemState.PropertiesConflicted, SvnItemState.None);
                                break;
                            case SvnConflictType.Tree:
                                break;
                        }
                    }
                }
                else
                    SetState(SvnItemState.None, SvnItemState.Conflicted | SvnItemState.ContentConflicted | SvnItemState.PropertiesConflicted);

                return _conflicts;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> is scheduled for delete
        /// </summary>
        public bool IsDeleteScheduled
        {
            get { return 0 != GetState(SvnItemState.Deleted); }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> is scheduled for replacement
        /// </summary>
        public bool IsReplaced
        {
            get { return 0 != GetState(SvnItemState.Replaced); }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="SvnItem"/> is in one of the projects in the solution
        /// </summary>
        /// <value><c>true</c> if the file is in one of the projects of the solution; otherwise, <c>false</c>.</value>
        public bool InSolution
        {
            get { return GetState(SvnItemState.InSolution) != 0; }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="ScnItem"/> is explicitly Scc Excluded
        /// </summary>
        public bool IsSccExcluded
        {
            get { return InSolution && _sccExcluded; }
        }

        /// <summary>
        /// Gets a value indicating whether this file is dirty in an open editor
        /// </summary>
        public bool IsDocumentDirty
        {
            get { return GetState(SvnItemState.DocumentDirty) != 0; }
        }

        /// <summary>
        /// Gets a value indicating whether this node is a nested working copy.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is nested working copy; otherwise, <c>false</c>.
        /// </value>
        public bool IsWCRoot
        {
            get { return GetState(SvnItemState.IsWCRoot) != 0; }
        }

        /// <summary>
        /// Gets a boolean indicating whether the <see cref="SvnItem"/> is explicitly ignored
        /// </summary>
        public bool IsIgnored
        {
            get
            {
                SvnItemState state;

                if (TryGetState(SvnItemState.Versioned, out state) && state != 0)
                    return false;
                else if (TryGetState(SvnItemState.Versionable, out state) && state == 0)
                    return false;
                else if (GetState(SvnItemState.Ignored) != 0)
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
            if (items == null)
                throw new ArgumentNullException("items");

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

        /// <summary>
        /// Gets the common parent of a list of SvnItems
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static SvnItem GetCommonParent(IEnumerable<SvnItem> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            SvnItem parent = null;

            foreach (SvnItem i in items)
            {
                if (parent == null)
                {
                    parent = i;
                    continue;
                }

                SvnItem j;
                if (i.FullPath.Length < parent.FullPath.Length)
                {
                    j = parent;
                    parent = i;
                }
                else
                {
                    j = i;
                }

                while (parent != null && !j.IsBelowPath(parent.FullPath))
                    parent = parent.Parent;

                if (parent == null)
                    return null;
            }

            return parent;
        }

        void EnsureClean()
        {
            Debug.Assert(_statusDirty != XBool.None, "Recursive refresh call");

            if (_statusDirty == XBool.True)
                this.RefreshStatus();
        }

        public void Dispose()
        {
            // TODO: Mark item as no longer valid
        }

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <value>The directory.</value>
        public string Directory
        {
            get { return SvnTools.GetNormalizedDirectoryName(FullPath); }
        }

        /// <summary>
        /// Gets the extension of the item
        /// </summary>
        /// <value>The extension.</value>
        /// <remarks>By definition directories do not have an extension</remarks>
        public string Extension
        {
            get { return IsDirectory ? "" : Path.GetExtension(Name); }
        }

        /// <summary>
        /// Gets the name of the file without its extension.
        /// </summary>
        /// <value>The name without extension.</value>
        /// <remarks>By definition directories do not have an extension</remarks>
        public string NameWithoutExtension
        {
            get { return IsDirectory ? Name : Path.GetFileNameWithoutExtension(Name); }
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
                string parentDir = Directory;

                if (string.IsNullOrEmpty(parentDir))
                    return null; // We are the root folder!

                ISvnStatusCache cache = StatusCache;

                if (cache != null)
                    return cache.GetAlreadyNormalizedItem(parentDir);
                else
                    return null;
            }
        }

        public SvnDirectory ParentDirectory
        {
            get
            {
                string parentDir = Directory;

                if (string.IsNullOrEmpty(parentDir))
                    return null;

                ISvnStatusCache cache = StatusCache;

                if (cache == null)
                    return null;

                return cache.GetDirectory(parentDir);
            }
        }

        /// <summary>
        /// Gets the working copy containing this <see cref="SvnItem"/>
        /// </summary>
        /// <value>The working copy.</value>
        public SvnWorkingCopy WorkingCopy
        {
            get
            {
                if (_workingCopy == null)
                {
                    if (IsAdministrativeArea || (!Exists && !IsVersioned))
                        return null;
                    else if (!IsDirectory)
                        _workingCopy = Parent;
                    else
                        _workingCopy = SvnWorkingCopy.CalculateWorkingCopy(_context, this);
                }

                if (_workingCopy != null)
                    return _workingCopy.WorkingCopy;
                else
                    return null;
            }
        }

        /// <summary>
        /// Checks if the node is somehow added. In this case its parents are
        /// needed for commits
        /// </summary>
        public bool IsNewAddition
        {
            get { return IsAdded || IsReplaced || Status.IsCopied; }
        }


        public static bool IsValidPath(string path, bool extraChecks)
        {
            if (!IsValidPath(path))
                return false;

            if (extraChecks)
                foreach (char c in path)
                    switch (c)
                    {
                        case '<':
                        case '>':
                        case '|':
                        case '\"':
                        case '*':
                        case '?':
                            return false;
                        default:
                            if (c < 32)
                                return false;
                            break;
                    }

            return true;
        }
        /// <summary>
        /// Determines whether the current instance is below the specified path
        /// </summary>
        /// <param name="root">The path.</param>
        /// <returns>
        /// 	<c>true</c> if the <see cref="SvnItem"/> is below or equal to the specified path; otherwise, <c>false</c>.
        /// </returns>
        public bool IsBelowPath(string root)
        {
            if (string.IsNullOrEmpty(root))
                throw new ArgumentNullException("path");

            return IsBelowRoot(FullPath, root);
        }

        public static bool IsBelowRoot(string path, string root)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            else if (string.IsNullOrEmpty(root))
                throw new ArgumentNullException("root");

            if (!path.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                return false;

            int n = path.Length - path.Length;

            if (n > 0)
                return (path[root.Length] == '\\');

            return (n == 0);
        }

        public static string SubPath(string path, string root)
        {
            return SubPath(path, root, false);
        }

        public static string SubPath(string path, string root, bool fullSelf)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            else if (string.IsNullOrEmpty(root))
                throw new ArgumentNullException("root");

            if (path.Length < root.Length)
                return "";
            else if (path.Length == root.Length)
                return (fullSelf || (path != root)) ? path : ".";
            else if (path[root.Length] == '\\')
                return path.Substring(root.Length + 1);
            else
                return path.Substring(root.Length);
        }

        /// <summary>
        /// Determines whether the current instance is below the specified path
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// 	<c>true</c> if [is below path] [the specified item]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsBelowPath(SvnItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            return IsBelowPath(item.FullPath);
        }

        #region Comparison
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="one">The one.</param>
        /// <param name="other">The other.</param>
        /// <returns>The result of the operator.</returns>
        [DebuggerStepThrough]
        public static bool operator ==(SvnItem one, SvnItem other)
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
        public static bool operator !=(SvnItem one, SvnItem other)
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
        /// Gets the Uri of the node
        /// </summary>
        public Uri Uri
        {
            get
            {
                SvnStatusData status = Status;

                return status.Uri;
            }
        }

        /// <summary>
        /// Gets this node as a directory or NULL if it is no directory on disk
        /// </summary>
        /// <returns></returns>
        public SvnDirectory AsDirectory()
        {
            if (!IsDirectory)
                return null;

            ISvnStatusCache cache = StatusCache;

            if (cache == null)
                return null;

            return cache.GetDirectory(FullPath);
        }

        /// <summary>
        /// Returns a path for the second file that's relative to the path of the first.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string MakeRelative(string root, string filename)
        {
            if (string.IsNullOrEmpty(root) || string.IsNullOrEmpty(filename))
                return filename;

            string[] rootComponents = root.Split(Path.DirectorySeparatorChar);
            string[] filecomponents = filename.Split(Path.DirectorySeparatorChar);

            int num = 1;
            while (num < rootComponents.Length && num < filecomponents.Length && !(rootComponents[num] != filecomponents[num]))
            {
                num++;
            }
            StringBuilder sb = new StringBuilder();
            for (int i = num; i < rootComponents.Length - 1; i++)
            {
                sb.Append("..");
                sb.Append(Path.DirectorySeparatorChar);
            }
            for (int j = num; j < filecomponents.Length; j++)
            {
                sb.Append(filecomponents[j]);
                if (j < filecomponents.Length - 1)
                {
                    sb.Append(Path.DirectorySeparatorChar);
                }
            }
            return sb.ToString();
        }

        public static string MakeRelativeNoCase(string relativeFrom, string path)
        {
            string rp = MakeRelative(relativeFrom.ToUpperInvariant(), path.ToUpperInvariant());

            if (string.IsNullOrEmpty(rp) || IsValidPath(rp))
                return path;

            int back = rp.LastIndexOf("..\\", StringComparison.Ordinal);
            if (back >= 0)
            {
                int rest = rp.Length - back - 3;

                return rp.Substring(0, back + 3) + path.Substring(path.Length - rest, rest);
            }
            else
                return path.Substring(path.Length - rp.Length, rp.Length);
        }

    }
}
