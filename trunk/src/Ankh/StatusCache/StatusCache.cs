// $Id$
using System;

using System.Collections;
using System.IO;
using System.Diagnostics;
using Utils;
using SharpSvn;
using Utils.Services;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Ankh
{
    /// <summary>
    /// Maintains path->SvnItem mappings.
    /// </summary>
    public class StatusCache : Ankh.Scc.IFileStatusCache
    {
        readonly object _lock = new object();
        readonly IAnkhServiceProvider _context;
        readonly SvnClient _client;
        readonly Dictionary<string, List<SvnItem>> _deletions;
        readonly Dictionary<string, SvnItem> _map;
        
        public StatusCache(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            _client = new SvnClient();
            _map = new Dictionary<string, SvnItem>(StringComparer.OrdinalIgnoreCase);
            _deletions = new Dictionary<string, List<SvnItem>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Fill the cache by running status on this directory.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="recurse">Whether to recurse to subdirectories.</param>
        public void Status(string dir, SvnDepth depth)
        {
            if (string.IsNullOrEmpty(dir))
                throw new ArgumentNullException("dir");

            lock (_lock)
            {
                if (!SvnTools.IsManagedPath(dir))
                {
                    // TODO: Clean all descendants of this directory
                    foreach (string file in Directory.GetFiles(dir))
                    {
                        string normPath = PathUtils.NormalizePath(file);
                        SvnItem existingItem;
                        if (_map.TryGetValue(normPath, out existingItem))
                            existingItem.RefreshTo(AnkhStatus.None);
                        else
                            _map[normPath] = new SvnItem(_context, file, AnkhStatus.Unversioned);
                    }
                    return;
                }

                SvnStatusArgs args = new SvnStatusArgs();
                args.Depth = depth;
                args.Revision = SvnRevision.None;
                args.RetrieveAllEntries = true;
                args.NoIgnore = true;
                args.ThrowOnError = false;
                this._client.Status(dir, args, StatusCallback);
            }
        }

        void Ankh.Scc.IFileStatusCache.UpdateStatus(string directory, SvnDepth depth)
        {
            Status(directory, depth);
        }

        void Ankh.Scc.IFileStatusCache.RefreshMe(SvnItem item, SvnNodeKind nodeKind)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            string path = item.FullPath;

            if (nodeKind == SvnNodeKind.File)
                path = Path.GetDirectoryName(path);
            // if it's a single file, refresh the directory with Files unrecursively            

            SvnStatusArgs args = new SvnStatusArgs();
            args.Depth = SvnDepth.Files;
            args.RetrieveAllEntries = true;
            args.NoIgnore = true;
            args.ThrowOnError = false;

            lock (_lock)
            {
                _client.Status(path, args, delegate(object sender, SvnStatusEventArgs e)
                    {
                        SvnItem i = this[e.Path];
                        if (i != null)
                        {
                            i.RefreshTo(new AnkhStatus(e));
                        }
                    });
            }
        }

        /// <summary>
        /// Marks the specified file dirty
        /// </summary>
        /// <param name="file"></param>
        void Ankh.Scc.IFileStatusCache.MarkDirty(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            lock (_lock)
            {
                string normPath = PathUtils.NormalizePath(path);
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
                    string normPath = PathUtils.NormalizePath(path);
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

                path = PathUtils.NormalizePath(path);

                lock (_lock)
                {
                    SvnItem item;

                    if (!_map.TryGetValue(path, out item))
                    {
                        // fill the status cache from this directory
                        string directory = path;
                        if (File.Exists(directory))
                            directory = Path.GetDirectoryName(directory);

                        if (Directory.Exists(directory))
                            this.Status(directory, SvnDepth.Files);
                        if (!_map.TryGetValue(path, out item))
                        {
                            Collection<SvnStatusEventArgs> statuses;
                            SvnStatusArgs args = new SvnStatusArgs();
                            args.Depth = SvnDepth.Empty;
                            args.ThrowOnError = false;
                            args.RetrieveAllEntries = true;
                            args.NoIgnore = true;
                            if (this._client.GetStatus(path, args, out statuses) && statuses.Count > 0)
                            {
                                SvnStatusEventArgs s0 = statuses[0];
                                _map[s0.FullPath] = item = new SvnItem(_context, s0.FullPath, new AnkhStatus(s0));
                            }
                            else
                            {
                                // Make sure the casing matches the on-disk casing if it exists
                                string truePath = SvnTools.GetTruePath(path) ?? path;

                                _map[path] = item = new SvnItem(_context, truePath, AnkhStatus.None);
                            }
                        }
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

            dir = PathUtils.NormalizePath(dir);

            lock (_lock)
            {
                List<SvnItem> deletions;
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

        /// <summary>
        /// Status callback.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="status"></param>
        private void StatusCallback(object sender, SvnStatusEventArgs e)
        {
            // Note: There is a lock(_lock) around this in our caller; should we remove it there and apply it here?

            string path = e.FullPath; // SharpSvn normalized it for us

            Debug.Assert(path == PathUtils.NormalizePath(e.FullPath), "Normalization rules must match SharpSVNs");

            // is there already an item for this path?
            SvnItem item;
            AnkhStatus status = new AnkhStatus(e);

            if (!_map.TryGetValue(path, out item) || item.FullPath != path)
            {
                // We only create an item if we don't have an existing
                // with a valid path. (No casing changes allowed!)

                if (item != null)
                    item.Dispose();

                _map[path] = new SvnItem(_context, path, status); 
            }                
            
            if (e.LocalContentStatus == SvnStatus.Deleted)
            {
                string parentDir = Path.GetDirectoryName(path);

                // store the deletions keyed on the parent directory
                List<SvnItem> list;

                if (!_deletions.TryGetValue(parentDir, out list))
                    list = new List<SvnItem>();

                SvnItem deletedItem;

                if (_map.TryGetValue(path, out deletedItem) && !list.Contains(deletedItem))
                {
                    list.Add(deletedItem);
                }

                _deletions[parentDir] = list;
            }

            // Note: There is a lock(_lock) around this in our caller
        }

        static List<SvnItem> RefreshDeletionsList(List<SvnItem> deletions)
        {
            List<SvnItem> newList = new List<SvnItem>();

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
