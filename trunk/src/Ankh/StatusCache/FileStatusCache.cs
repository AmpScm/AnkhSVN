// $Id$
using System;

using System.Collections;
using System.IO;
using System.Diagnostics;
using Utils;
using SharpSvn;
using Ankh.Scc;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Ankh.StatusCache
{
    /// <summary>
    /// Maintains path->SvnItem mappings.
    /// </summary>
    public sealed class FileStatusCache : Ankh.Scc.IFileStatusCache
    {
        readonly object _lock = new object();
        readonly IAnkhServiceProvider _context;
        readonly SvnClient _client;
        readonly Dictionary<string, DeletedSvnItemList> _deletions;
        readonly Dictionary<string, SvnItem> _map;
        
        public FileStatusCache(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            _client = new SvnClient();
            _map = new Dictionary<string, SvnItem>(StringComparer.OrdinalIgnoreCase);
            _deletions = new Dictionary<string, DeletedSvnItemList>(StringComparer.OrdinalIgnoreCase);
        }        

        void Ankh.Scc.IFileStatusCache.UpdateStatus(string directory, SvnDepth depth)
        {
            RefreshPath(SvnTools.GetNormalizedFullPath(directory), SvnNodeKind.Unknown, depth);
        }

        void Ankh.Scc.IFileStatusCache.RefreshItem(SvnItem item, SvnNodeKind nodeKind)
        {
            RefreshPath(item.FullPath, nodeKind, SvnDepth.Files);

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

                    updateItem.RefreshTo(item.Exists ? AnkhStatus.NotVersioned : AnkhStatus.NotExisting);
                }
            }

            Debug.Assert(updateItem.IsStatusClean(), "The item requesting to be updated is updated");
        }

        SvnItem CreateItem(string fullPath, AnkhStatus status)
        {
            return new SvnItem(_context, fullPath, status);
        }

        SvnItem CreateItem(string fullPath, AnkhStatus status, SvnNodeKind nodeKind)
        {
            return new SvnItem(_context, fullPath, status, nodeKind);
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
        void RefreshPath(string path, SvnNodeKind pathKind, SvnDepth depth)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            string walkPath = path;
            bool walkingDirectory = false;

            switch(pathKind)
            {
                case SvnNodeKind.Directory:
                    walkingDirectory = true;
                    break;
                default:
                    if(File.Exists(path))
                    {
                        pathKind = SvnNodeKind.File;
                        goto case SvnNodeKind.File;
                    }
                    break;
                case SvnNodeKind.File:
                    if(depth != SvnDepth.Empty)
                    {
                        walkPath = Path.GetDirectoryName(path);
                        walkingDirectory = true;
                    }
                    break;
            }

            SvnStatusArgs args = new SvnStatusArgs();
            args.Depth = depth;
            args.RetrieveAllEntries = true;
            args.NoIgnore = true;
            args.ThrowOnError = false;

            lock (_lock)
            {
                DeletedSvnItemList deletedItems = null;

                if (depth >= SvnDepth.Files && depth <= SvnDepth.Infinity) // Exclude possible new depths!
                {
                    // We get more information for free, lets use that to update other items

                    if (_deletions.TryGetValue(walkPath, out deletedItems))
                    {
                        // The path we walk contains deletions; mark them dirty to detect them after the status call
                        foreach (SvnItem item in deletedItems)
                        {
                            if ((depth > SvnDepth.Files) || item.IsFile)
                                ((ISvnItemUpdate)item).TickItem();
                        }
                    }
                }

                bool ok = _client.Status(walkPath, args, RefreshCallback);

                if (ok)
                {
                    // Status call succeeded
                    if (deletedItems != null)
                    {
                        for(int i = 0; i < deletedItems.Count; i++)
                        {
                            SvnItem item = deletedItems[i];

                            if ((depth > SvnDepth.Files) || item.IsFile)
                            {
                                ISvnItemUpdate updateItem = item;

                                if (updateItem.IsItemTicked()) // Should have been unticked if it is still deleted
                                {                                    
                                    deletedItems.RemoveAt(i--);
                                    
                                    // TODO: BH: Verify if this is always safe...
                                    // (RefreshItem() fixes up if we dispose the item we are looking at)

                                    SvnItem other;
                                    if(_map.TryGetValue(item.FullPath, out other))
                                    {
                                        if(other == item)
                                            _map.Remove(item.FullPath);
                                    }

                                    item.Dispose();
                                }
                            }
                        }
                    }
                }

                SvnItem pathItem;
                if (!_map.TryGetValue(path, out pathItem))
                {
                    // We promised data on path but we didn't receive from subversion
                    // Let's just provide what we know about the on-disk files

                    if (walkingDirectory)
                    {
                        DirectoryInfo dir = new DirectoryInfo(walkPath);

                        if (!_map.ContainsKey(walkPath))
                        {
                            // Mark it as existing if we are sure 
                            _map[walkPath] = CreateItem(walkPath, AnkhStatus.NotVersioned,
                                dir.Exists ? SvnNodeKind.Directory : SvnNodeKind.None);
                        }

                        if (walkPath != path && dir.Exists)
                        {
                            // BH: Ankh used do this for all files here; this prevents casing regenerations but does not help much
                            foreach (FileInfo file in dir.GetFiles())
                            {
                                if (!_map.ContainsKey(file.FullName))
                                    _map[file.FullName] = CreateItem(file.FullName, AnkhStatus.NotVersioned, SvnNodeKind.File);
                            }
                        }
                    }

                    if (!_map.ContainsKey(path))
                    {
                        // Ok: We have a non existing file/directory; just create a nonexisting one
                        _map[path] = CreateItem(path, AnkhStatus.NotExisting);
                    }
                }
                else
                {
                    // We promised to update the item specified by path; let's verify if we did.
                    ISvnItemUpdate updateItem = pathItem;

                    if (!updateItem.IsStatusClean())
                    {
                        // We did not; so the file does not exist!
                        updateItem.RefreshTo(pathItem.Exists ? AnkhStatus.NotVersioned : AnkhStatus.NotExisting);
                    }
                }
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
            // Note: There is a lock(_lock) around this in our caller; should we remove it there and apply it here?

            string path = e.FullPath; // SharpSvn normalized it for us

            Debug.Assert(path == SvnTools.GetNormalizedFullPath(e.FullPath), "Normalization rules apply");

            // is there already an item for this path?
            SvnItem item;
            AnkhStatus status = new AnkhStatus(e);

            if (!_map.TryGetValue(path, out item) || !item.NewFullPathOk(path, status))
            {
                // We only create an item if we don't have an existing
                // with a valid path. (No casing changes allowed!)

                if (item != null)
                {
                    ((ISvnItemUpdate)item).RefreshTo(status); // Give them a reference to the status (might fo
                    item.Dispose();
                }

                _map[path] = item = new SvnItem(_context, path, status);
            }
            else
                ((ISvnItemUpdate)item).RefreshTo(status);
            
            if (e.LocalContentStatus == SvnStatus.Deleted)
            {
                string parentDir = Path.GetDirectoryName(path);

                // store the deletions keyed on the parent directory
                DeletedSvnItemList deletedItems;

                if (!_deletions.TryGetValue(parentDir, out deletedItems))
                    _deletions[parentDir] = deletedItems = new DeletedSvnItemList();

                if(deletedItems.Contains(path) && deletedItems[path] != item)
                    deletedItems.Remove(path);

                deletedItems.Add(item);
            }

            // Note: There is a lock(_lock) around this in our caller
        }

        /// <summary>
        /// Marks the specified file dirty
        /// </summary>
        /// <param name="file"></param>
        void Ankh.Scc.IFileStatusCache.MarkDirty(string path)
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

        /// <summary>
        /// Marks the specified file dirty
        /// </summary>
        /// <param name="file"></param>
        void Ankh.Scc.IFileStatusCache.MarkDirty(IEnumerable<string> paths)
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

                lock (_lock)
                {
                    SvnItem item;

                    if (!_map.TryGetValue(path, out item))
                    {
                        string truePath = SvnTools.GetTruePath(path);

                        // Just create an item based on his name. Delay the svn calls as long as we can
                        _map[path] = item = new SvnItem(_context, truePath ?? path, 
                            (truePath != null) ? AnkhStatus.NotVersioned : AnkhStatus.NotExisting, SvnNodeKind.Unknown);

                        item.MarkDirty(); // Load status on first access
                    }

                    return item;
                }
            }
        }

        /// <summary>
        /// Clears the whole cache; called from solution closing (Scc)
        /// </summary>
        public void ClearCache()
        {
            lock (_lock)
            {
                this._deletions.Clear();
                this._map.Clear();
            }
        }

        /// <summary>
        /// Returns a list of deleted items in the specified directory.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public IList<SvnItem> GetDeletions(string dir)
        {
            if (dir == null)
                throw new ArgumentNullException("dir");

            dir = SvnTools.GetNormalizedFullPath(dir);

            lock (_lock)
            {
                DeletedSvnItemList deletions;
                if (_deletions.TryGetValue(dir, out deletions))
                {
                    deletions = RefreshDeletionsList(deletions);
                    _deletions[dir] = deletions;
                    return deletions;
                }
                else
                    return new SvnItem[] { };
            }
        }

        IEnumerable<SvnItem> Ankh.Scc.IFileStatusCache.GetDeletions(string dir)
        {
            return GetDeletions(dir);
        }

        static DeletedSvnItemList RefreshDeletionsList(DeletedSvnItemList deletions)
        {
            DeletedSvnItemList newList = new DeletedSvnItemList();

            // First mark all items dirty to get an optimized refresh
            foreach (SvnItem item in deletions)
            {
                item.MarkDirty();
            }

            // Now get the deleted items
            foreach(SvnItem item in deletions)
            {
                if (item.Status.LocalContentStatus == SvnStatus.Deleted)
                    newList.Add(item);
            }

            return newList;
        }

        #region IFileStatusCache Members


        /// <summary>
        /// Determines whether the path value specifies a valid path
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid path] [the specified name]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (path.LastIndexOf(':') > 1)
                return false;

            // TODO: Add more checks. This code is called from the OpenDocumentTracker

            return true;
        }

        #endregion
    }
}
