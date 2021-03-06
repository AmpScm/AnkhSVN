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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Ankh.Commands;
using Ankh.Scc.Engine;
using Ankh.Scc.Svn;
using SharpSvn;

namespace Ankh.Scc.StatusCache
{
    /// <summary>
    /// Maintains path->SvnItem mappings.
    /// </summary>
    [GlobalService(typeof(ISvnStatusCache), AllowPreRegistered = true)]
    [GlobalService(typeof(ISvnItemChange), AllowPreRegistered = true)]
    sealed partial class SvnStatusCache : AnkhService, Ankh.Scc.ISvnStatusCache, ISvnItemChange
    {
        readonly object _lock = new object();
        readonly SvnClient _client;
        readonly Dictionary<string, SvnItem> _map; // Maps from full-normalized paths to SvnItems
        readonly Dictionary<string, SvnDirectory> _dirMap;
        IAnkhCommandService _commandService;
        bool _enableUpgrade;

        public SvnStatusCache(IAnkhServiceProvider context)
            : base(context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _client = new SvnClient();
            _map = new Dictionary<string, SvnItem>(StringComparer.OrdinalIgnoreCase);
            _dirMap = new Dictionary<string, SvnDirectory>(StringComparer.OrdinalIgnoreCase);
            InitializeShellMonitor();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                ReleaseShellMonitor(disposing);
                _client.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Gets the command service.
        /// </summary>
        /// <value>The command service.</value>
        IAnkhCommandService CommandService
        {
            get { return _commandService ?? (_commandService = Context.GetService<IAnkhCommandService>()); }
        }

        void Ankh.Scc.ISvnStatusCache.RefreshItem(SvnItem item, SvnNodeKind nodeKind)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            RefreshPath(item.FullPath, nodeKind);

            ISvnItemUpdate updateItem = (ISvnItemUpdate)item;

            if (!updateItem.IsStatusClean())
            {
                // Ok, the status update did not refresh the item requesting to be refreshed
                // That means the item is not here or RefreshPath would have added it

                SvnItem other;
                if (_map.TryGetValue(item.FullPath, out other) && other != item)
                {
                    updateItem.RefreshTo(other); // This item is no longer current; but we have the status anyway
                }
                else
                {
                    Debug.Assert(false, "RefreshPath did not deliver up to date information",
                        "The RefreshPath public api promises delivering up to date data, but none was received");

                    updateItem.RefreshTo(item.Exists ? NoSccStatus.NotVersioned : NoSccStatus.NotExisting, SvnNodeKind.Unknown);
                }
            }

            Debug.Assert(updateItem.IsStatusClean(), "The item requesting to be updated is updated");
        }

        void Ankh.Scc.ISvnStatusCache.RefreshWCRoot(SvnItem svnItem)
        {
            if (svnItem == null)
                throw new ArgumentNullException("svnItem");

            Debug.Assert(svnItem.IsDirectory);

            // We retrieve nesting information by walking the entry data of the parent directory
            lock (_lock)
            {
                string root;

                try
                {
                    root = _client.GetWorkingCopyRoot(svnItem.FullPath);
                }
                catch { root = null; }

                if (root == null)
                {
                    ((ISvnItemUpdate)svnItem).SetState(SvnItemState.None, SvnItemState.IsWCRoot);
                    return;
                }

                // Set all nodes between this node and the root to not-a-wcroot
                ISvnItemUpdate oi;
                while (root.Length < svnItem.FullPath.Length)
                {
                    oi = (ISvnItemUpdate)svnItem;

                    oi.SetState(SvnItemState.None, SvnItemState.IsWCRoot);
                    svnItem = svnItem.Parent;

                    if (svnItem == null)
                        return;
                }

                oi = svnItem;

                if (svnItem.FullPath == root)
                    oi.SetState(SvnItemState.IsWCRoot, SvnItemState.None); 
                else
                    oi.SetState(SvnItemState.None, SvnItemState.IsWCRoot);
            }
        }

        SvnItem CreateItem(string fullPath, SvnStatusData status)
        {
            return new SvnItem(this, fullPath, status);
        }

        SvnItem CreateItem(string fullPath, NoSccStatus status, SvnNodeKind nodeKind)
        {
            return new SvnItem(this, fullPath, status, nodeKind);
        }

        SvnItem CreateItem(string fullPath, NoSccStatus status)
        {
            return CreateItem(fullPath, status, SvnNodeKind.Unknown);
        }

        /// <summary>
        /// Stores the item in the caching dictionary/ies
        /// </summary>
        /// <param name="item"></param>
        void StoreItem(SvnItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _map[item.FullPath] = item;

            SvnDirectory dir;
            if (_dirMap.TryGetValue(item.FullPath, out dir))
            {
                if (item.IsDirectory)
                {
                    ((ISvnDirectoryUpdate)dir).Store(item);
                }
                else
                    ScheduleForCleanup(dir);
            }

            string parentDir = SvnTools.GetNormalizedDirectoryName(item.FullPath);

            if (string.IsNullOrEmpty(parentDir) || parentDir == item.FullPath)
                return; // Skip root directory

            if (_dirMap.TryGetValue(item.FullPath, out dir))
            {
                ((ISvnDirectoryUpdate)dir).Store(item);
            }
        }

        void RemoveItem(SvnItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            bool deleted = false;
            SvnDirectory dir;
            if (_dirMap.TryGetValue(item.FullPath, out dir))
            {
                // The item is a directory itself.. remove it's map
                if (dir.Directory == item)
                {
                    _dirMap.Remove(item.FullPath);
                    deleted = true;
                }
            }

            SvnItem other;
            if (_map.TryGetValue(item.FullPath, out other))
            {
                if (item == other)
                    _map.Remove(item.FullPath);
            }

            if (!deleted)
                return;

            string parentDir = SvnTools.GetNormalizedDirectoryName(item.FullPath);

            if (string.IsNullOrEmpty(parentDir) || parentDir == item.FullPath)
                return; // Skip root directory

            if (_dirMap.TryGetValue(item.FullPath, out dir))
            {
                dir.Remove(item.FullPath);
            }
        }

        /// <summary>
        /// Refreshes the specified path using the specified depth
        /// </summary>
        /// <param name="path">A normalized path</param>
        /// <param name="pathKind"></param>
        /// <param name="depth"></param>
        /// <remarks>
        /// If the path is a file and depth is greater that <see cref="SvnDepth.Empty"/> the parent folder is walked instead.
        /// 
        /// <para>This method guarantees that after calling it at least one up-to-date item exists 
        /// in the statusmap for <paramref name="path"/>. If we can not find information we create
        /// an unspecified item
        /// </para>
        /// </remarks>
        void RefreshPath(string path, SvnNodeKind pathKind)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            string walkPath = path;
            bool walkingDirectory = false;

            switch (pathKind)
            {
                case SvnNodeKind.Directory:
                    walkingDirectory = true;
                    break;
                case SvnNodeKind.File:
                    walkPath = SvnTools.GetNormalizedDirectoryName(path);
                    walkingDirectory = true;
                    break;
                default:
                    try
                    {
                        if (File.Exists(path)) // ### Not long path safe
                        {
                            pathKind = SvnNodeKind.File;
                            goto case SvnNodeKind.File;
                        }
                    }
                    catch (PathTooLongException)
                    { /* Fall through */ }
                    break;
            }

            SvnStatusArgs args = new SvnStatusArgs();
            args.Depth = SvnDepth.Children;
            args.RetrieveAllEntries = true;
            args.RetrieveIgnoredEntries = true;
            args.ThrowOnError = false;

            lock (_lock)
            {
                SvnDirectory directory;
                ISvnDirectoryUpdate updateDir;
                SvnItem walkItem;

                // We get more information for free, lets use that to update other items
                if (_dirMap.TryGetValue(walkPath, out directory))
                {
                    updateDir = directory;
                    updateDir.TickAll();
                }
                else
                {
                    // No existing directory instance, let's create one
                    updateDir = directory = new SvnDirectory(Context, walkPath);
                    _dirMap[walkPath] = directory;
                }

                walkItem = directory.Directory;

                bool ok;
                bool statSelf = false;
                bool noWcAtAll = false;

                // Don't retry file open/read operations on failure. These would only delay the result 
                // (default number of delays = 100)
                using (new SharpSvn.Implementation.SvnFsOperationRetryOverride(0))
                {
                    ok = _client.Status(walkPath, args, RefreshCallback);
                }

                if (!ok)
                {
                    if (args.LastException != null)
                    {
                        switch (args.LastException.SvnErrorCode)
                        {
                            case SvnErrorCode.SVN_ERR_WC_UNSUPPORTED_FORMAT:
                                if (CommandService != null)
                                    CommandService.PostExecCommand(AnkhCommand.NotifyWcToNew, walkPath);
                                break;
                            case SvnErrorCode.SVN_ERR_WC_UPGRADE_REQUIRED:
                                _enableUpgrade = true;

                                if (updateDir != null)
                                    updateDir.SetNeedsUpgrade();

                                if (!_sendUpgrade && CommandService != null)
                                {
                                    _sendUpgrade = true;
                                    CommandService.PostExecCommand(AnkhCommand.NotifyUpgradeRequired, walkPath);
                                }
                                break;
                            case SvnErrorCode.SVN_ERR_WC_NOT_WORKING_COPY:
                                // Status only reports this error if there is no ancestor working copy
                                // We should avoid statting all parent directories again for .IsVersionable
                                noWcAtAll = true;
                                break;
                            case SvnErrorCode.SVN_ERR_WC_CLEANUP_REQUIRED:
                                if (updateDir != null)
                                    updateDir.SetNeedsCleanup();
                                break;
                        }
                    }
                    statSelf = true;
                }
                else if (directory != null)
                    walkItem = directory.Directory; // Might have changed via casing

                if (!statSelf)
                {
                    if (((ISvnItemUpdate)walkItem).ShouldRefresh())
                        statSelf = true;
                    else if (walkingDirectory && !walkItem.IsVersioned)
                        statSelf = true;
                }

                if (statSelf)
                {
                    // Svn did not stat the items for us.. Let's make something up

                    if (walkingDirectory)
                        StatDirectory(walkPath, directory, noWcAtAll);
                    else
                    {
                        // Just stat the item passed and nothing else in the Depth.Empty case

                        if (walkItem == null)
                        {
                            string truepath = SvnTools.GetTruePath(walkPath); // Gets the on-disk casing if it exists

                            StoreItem(walkItem = CreateItem(truepath ?? walkPath,
                                (truepath != null) ? NoSccStatus.NotVersioned : NoSccStatus.NotExisting, SvnNodeKind.Unknown));
                        }
                        else
                        {
                            ((ISvnItemUpdate)walkItem).RefreshTo(walkItem.Exists ? NoSccStatus.NotVersioned : NoSccStatus.NotExisting, SvnNodeKind.Unknown);
                        }
                    }
                }

                if (directory != null)
                {
                    foreach (ISvnItemUpdate item in directory)
                    {
                        if (item.IsItemTicked()) // These items were not found in the stat calls
                            item.RefreshTo(NoSccStatus.NotExisting, SvnNodeKind.Unknown);
                    }

                    if (updateDir.ScheduleForCleanup)
                        ScheduleForCleanup(directory); // Handles removing already deleted items
                    // We keep them cached for the current command only
                }


                SvnItem pathItem; // We promissed to return an updated item for the specified path; check if we updated it

                if (!_map.TryGetValue(path, out pathItem))
                {
                    // We did not; it does not even exist in the cache
                    StoreItem(pathItem = CreateItem(path, NoSccStatus.NotExisting));

                    if (directory != null)
                    {
                        updateDir.Store(pathItem);
                        ScheduleForCleanup(directory);
                    }
                }
                else
                {
                    ISvnItemUpdate update = pathItem;

                    if (!update.IsStatusClean())
                    {
                        update.RefreshTo(NoSccStatus.NotExisting, SvnNodeKind.Unknown); // We did not see it in the walker

                        if (directory != null)
                        {
                            ((ISvnDirectoryUpdate)directory).Store(pathItem);
                            ScheduleForCleanup(directory);
                        }
                    }
                }
            }
        }

        private void StatDirectory(string walkPath, SvnDirectory directory, bool noWcAtAll)
        {
            // Note: There is a lock(_lock) around this in our caller

            bool canRead;
            string adminName = SvnClient.AdministrativeDirectoryName;
            NoSccStatus noSccStatus = noWcAtAll ? NoSccStatus.NotVersionable : NoSccStatus.NotVersioned;
            foreach (SccFileSystemNode node in SccFileSystemNode.GetDirectoryNodes(walkPath, out canRead))
            {
                if (string.Equals(node.Name, adminName, StringComparison.OrdinalIgnoreCase) || node.IsHiddenOrSystem)
                    continue;

                SvnItem item;
                if (node.IsFile)
                {
                    if (!_map.TryGetValue(node.FullPath, out item))
                        StoreItem(CreateItem(node.FullPath, noSccStatus, SvnNodeKind.File));
                    else
                    {
                        ISvnItemUpdate updateItem = item;
                        if (updateItem.ShouldRefresh())
                            updateItem.RefreshTo(noSccStatus, SvnNodeKind.File);
                    }
                }
                else
                {
                    if (!_map.TryGetValue(node.FullPath, out item))
                        StoreItem(CreateItem(node.FullPath, noSccStatus, SvnNodeKind.Directory));
                    // Don't clear state of a possible working copy
                }
            }

            if (canRead) // The directory exists
            {
                SvnItem item;

                if (!_map.TryGetValue(walkPath, out item))
                {
                    StoreItem(CreateItem(walkPath, NoSccStatus.NotVersioned, SvnNodeKind.Directory));
                    // Mark it as existing if we are sure 
                }
                else
                {
                    ISvnItemUpdate updateItem = item;
                    if (updateItem.ShouldRefresh())
                        updateItem.RefreshTo(NoSccStatus.NotVersioned, SvnNodeKind.Directory);
                }
            }

            // Note: There is a lock(_lock) around this in our caller
        }

        bool _sendUpgrade;
        public void ResetUpgradeWarning()
        {
            _sendUpgrade = false;
        }

        bool _postedCleanup;
        List<SvnDirectory> _cleanup = new List<SvnDirectory>();
        private void ScheduleForCleanup(SvnDirectory directory)
        {
            lock (_lock)
            {
                if (!_cleanup.Contains(directory))
                    _cleanup.Add(directory);

                if (!_postedCleanup)
                    CommandService.SafePostTickCommand(ref _postedCleanup, AnkhCommand.SvnCacheFinishTasks);
            }
        }

        internal void OnCleanup()
        {
            lock (_lock)
            {
                _postedCleanup = false;

                while (_cleanup.Count > 0)
                {
                    SvnDirectory dir = _cleanup[0];
                    string path = dir.FullPath;

                    _cleanup.RemoveAt(0);

                    for (int i = 0; i < dir.Count; i++)
                    {
                        SvnItem item = dir[i];
                        if (((ISvnItemUpdate)item).ShouldClean())
                        {
                            RemoveItem(item);
                            dir.RemoveAt(i--);
                        }
                    }

                    if (dir.Count == 0)
                    {
                        // We cache the path before.. as we don't want the svnitem to be generated again
                        _dirMap.Remove(path);
                    }
                }
            }
        }

        static bool NewFullPathOk(SvnItem item, string fullPath, SvnStatusData status)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            else if (status == null)
                throw new ArgumentNullException("status");

            if (fullPath == item.FullPath)
                return true;

            switch (status.LocalNodeStatus)
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

        /// <summary>
        /// Called from RefreshPath's call to <see cref="SvnClient::Status"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// All information we receive here is live from SVN and Disk and is therefore propagated
        /// in all SvnItems wishing information
        /// </remarks>
        void RefreshCallback(object sender, SvnStatusEventArgs e)
        {
            // Note: There is a lock(_lock) around this in our caller

            SvnStatusData status = new SvnStatusData(e);
            string path = e.FullPath; // Fully normalized

            SvnItem item;
            if (!_map.TryGetValue(path, out item) || !NewFullPathOk(item, path, status))
            {
                // We only create an item if we don't have an existing
                // with a valid path. (No casing changes allowed!)

                SvnItem newItem = CreateItem(path, status);
                StoreItem(newItem);

                if (item != null)
                {
                    ((ISvnItemUpdate)item).RefreshTo(newItem);
                    item.Dispose();
                }

                item = newItem;
            }
            else
                ((ISvnItemUpdate)item).RefreshTo(status);

            // Note: There is a lock(_lock) around this in our caller
        }

        /// <summary>
        /// Marks the specified file dirty
        /// </summary>
        /// <param name="file"></param>
        void ISccStatusCache.MarkDirty(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            string normPath = SvnTools.GetNormalizedFullPath(path);

            lock (_lock)
            {
                SvnItem item;

                if (_map.TryGetValue(normPath, out item))
                {
                    item.MarkDirty();
                }
            }
        }

        void ISccStatusCache.MarkDirtyRecursive(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            lock (_lock)
            {
                List<string> names = new List<string>();

                foreach (SvnItem v in _map.Values)
                {
                    string name = v.FullPath;
                    if (v.IsBelowPath(path))
                    {
                        v.MarkDirty();
                    }
                }
            }
        }

        public IEnumerable<string> GetCachedBelow(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            lock (_lock)
            {
                List<string> items = new List<string>();

                foreach (SvnItem v in _map.Values)
                {
                    if (v.IsBelowPath(path))
                        items.Add(v.FullPath);
                }

                return items;
            }
        }

        public IEnumerable<string> GetCachedBelow(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("path");

            lock (_lock)
            {
                SortedList<string, SvnItem> items = new SortedList<string, SvnItem>(StringComparer.OrdinalIgnoreCase);

                foreach (string path in paths)
                {
                    foreach (SvnItem v in _map.Values)
                    {
                        if (v.IsBelowPath(path))
                            items[v.FullPath] = v;
                    }
                }

                return new List<string>(items.Keys);
            }
        }

        /// <summary>
        /// Marks the specified file dirty
        /// </summary>
        /// <param name="file"></param>
        void ISccStatusCache.MarkDirty(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            lock (_lock)
            {
                foreach (string path in paths)
                {
                    string normPath = SvnTools.GetNormalizedFullPath(path);
                    SvnItem item;

                    if (_map.TryGetValue(normPath, out item))
                    {
                        item.MarkDirty();
                    }
                }
            }
        }


        public SvnItem this[string path]
        {
            get
            {
                if (string.IsNullOrEmpty(path))
                    throw new ArgumentNullException("path");

                path = SvnTools.GetNormalizedFullPath(path);

                return GetAlreadyNormalizedItem(path);
            }
        }

        public SvnItem GetAlreadyNormalizedItem(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            lock (_lock)
            {
                SvnItem item;

                if (!_map.TryGetValue(path, out item))
                {
                    string truePath = SvnTools.GetTruePath(path, true);

                    // Just create an item based on his name. Delay the svn calls as long as we can
                    StoreItem(item = new SvnItem(this, truePath ?? path, NoSccStatus.Unknown, SvnNodeKind.Unknown));

                    //item.MarkDirty(); // Load status on first access
                }

                return item;
            }
        }

        /// <summary>
        /// Clears the whole cache; called from solution closing (Scc)
        /// </summary>
        public void ClearCache()
        {
            lock (_lock)
            {
                this._dirMap.Clear();
                this._map.Clear();
            }
        }

        void ISvnStatusCache.SetSolutionContained(string path, bool inSolution, bool sccExcluded)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SvnItem item;
            if (_map.TryGetValue(path, out item))
                ((ISvnItemStateUpdate)item).SetSolutionContained(inSolution, sccExcluded);
        }

        #region IFileStatusCache Members


        public SvnDirectory GetDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            lock (_lock)
            {
                SvnDirectory dir;

                if (_dirMap.TryGetValue(path, out dir))
                    return dir;

                SvnItem item = this[path];

                if (item.IsDirectory)
                {
                    dir = new SvnDirectory(Context, path);
                    dir.Add(item);
                    return dir;
                }
                else
                    return null;
            }
        }

        #endregion

        internal void BroadcastChanges()
        {
            ISvnItemStateUpdate update;
            if (_map.Count > 0)
                update = EnumTools.GetFirst(_map.Values);
            else
                update = this["C:\\"]; // Just give me a SvnItem instance to access the interface

            IList<SvnItem> updates = update.GetUpdateQueueAndClearScheduled();

            if (updates != null)
                OnSvnItemsChanged(new SvnItemsEventArgs(updates));
        }

        public event EventHandler<SvnItemsEventArgs> SvnItemsChanged;

        public void OnSvnItemsChanged(SvnItemsEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            if (SvnItemsChanged != null)
                SvnItemsChanged(this, e);
        }

        public bool EnableUpgradeCommand
        {
            get { return _enableUpgrade; }
        }

        SccItem ISccStatusCache.this[string path]
        {
            get { return this[path]; }
        }
    }
}
