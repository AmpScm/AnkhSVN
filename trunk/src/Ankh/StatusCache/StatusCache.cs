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
                this._client.Status(dir, args, new EventHandler<SvnStatusEventArgs>(Callback));
            }
        }

        void Ankh.Scc.IFileStatusCache.UpdateStatus(string directory, SvnDepth depth)
        {
            Status(directory, depth);
        }

        /// <summary>
        /// Marks the specified file dirty
        /// </summary>
        /// <param name="file"></param>
        void Ankh.Scc.IFileStatusCache.MarkDirty(string file)
        {
            lock (_lock)
            {
                string normPath = PathUtils.NormalizePath(file);
                SvnItem item;

                if (_map.TryGetValue(normPath, out item))
                {
                    item.MarkDirty();
                }
            }
        }

        public SvnItem this[string path]
        {
            get
            {
                lock (_lock)
                {
                    string normPath = PathUtils.NormalizePath(path);
                    SvnItem item;

                    if (!_map.TryGetValue(normPath, out item))
                    {
                        // fill the status cache from this directory
                        string directory = normPath;
                        if (File.Exists(directory))
                            directory = Path.GetDirectoryName(directory);

                        if (Directory.Exists(directory))
                            this.Status(directory, SvnDepth.Children);
                        if (!_map.TryGetValue(normPath, out item))
                        {
                            Collection<SvnStatusEventArgs> statuses;
                            SvnStatusArgs args = new SvnStatusArgs();
                            args.Depth = SvnDepth.Empty;
                            args.ThrowOnError = false;
                            args.RetrieveAllEntries = true;
                            if (this._client.GetStatus(normPath, args, out statuses) && statuses.Count > 0)
                            {
                                _map[normPath] = item = new SvnItem(_context, path, new AnkhStatus(statuses[0]));
                            }
                            else
                                _map[normPath] = item = new SvnItem(_context, path, AnkhStatus.None);
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
        private void Callback(object sender, SvnStatusEventArgs e)
        {
            // we need all paths to be on ONE form
            string normPath = PathUtils.NormalizePath(e.FullPath);

            e.Detach();

            // is there already an item for this path?
            SvnItem existingItem;
            AnkhStatus status = new AnkhStatus(e);

            if (_map.TryGetValue(normPath, out existingItem) && !existingItem.IsUnversionable)
                existingItem.RefreshTo(status);
            else
                _map[normPath] = new SvnItem(_context, e.Path, status);

            if (e.LocalContentStatus == SvnStatus.Deleted)
            {
                string containingDir;
                containingDir = Path.GetDirectoryName(e.Path);

                // store the deletions keyed on the parent directory
                List<SvnItem> list;

                if (!_deletions.TryGetValue(containingDir, out list))
                    list = new List<SvnItem>();

                SvnItem deletedItem;

                if (_map.TryGetValue(normPath, out deletedItem) && !list.Contains(deletedItem))
                {
                    list.Add(deletedItem);
                }

                _deletions[containingDir] = list;
            }
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
    }
}
