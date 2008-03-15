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
        public StatusCache(IAnkhServiceProvider context)
        {
            this.context = context;
            this.client = new SvnClient();
            _map = new Dictionary<string, SvnItem>(StringComparer.OrdinalIgnoreCase);
            this._deletions = new Dictionary<string, List<SvnItem>>(StringComparer.OrdinalIgnoreCase);
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

            lock (this)
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
                            _map[normPath] = new SvnItem(context, file, AnkhStatus.Unversioned);
                    }
                    return;
                }

                this.currentPath = dir;
                SvnStatusArgs args = new SvnStatusArgs();
                args.Depth = depth;
                args.Revision = SvnRevision.None;
                args.RetrieveAllEntries = true;
                args.ContactRepository = false;
                args.NoIgnore = true;
                this.client.Status(dir, args, new EventHandler<SvnStatusEventArgs>(Callback));
            }
        }

        void Ankh.Scc.IFileStatusCache.UpdateStatus(string directory, SvnDepth depth)
        {
            Status(directory, depth);
        }

        public SvnItem this[string path]
        {
            get
            {
                string normPath = PathUtils.NormalizePath(path);
                SvnItem item;

                if (!_map.TryGetValue(normPath, out item))
                {
                    this.cacheMisses++;

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
                        if (this.client.GetStatus(normPath, args, out statuses) && statuses.Count > 0)
                        {
                            _map[normPath] = item = new SvnItem(context, path, new AnkhStatus(statuses[0]));
                        }
                        else
                            _map[normPath] = item = new SvnItem(context, path, AnkhStatus.None);
                    }
                }
                else
                {
                    this.cacheHits++;
                }

                return item;
            }
        }

        /// <summary>
        /// Clears the whole cache; called from solution closing (Scc)
        /// </summary>
        public void ClearCache()
        {
            this._deletions.Clear();
            this._map.Clear();
        }

        /// <summary>
        /// Current success rate of the cache.
        /// </summary>
        public float CacheHitSuccess
        {
            get
            {
                // we don't wanna divide by zero
                if (this.cacheHits + this.cacheMisses > 0)
                {
                    return (100 / ((float)this.cacheHits + this.cacheMisses)) * this.cacheHits;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Returns a list of deleted items in the specified directory.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public IList<SvnItem> GetDeletions(string dir)
        {
            string normdir = PathUtils.NormalizePath(dir, this.currentPath);

            List<SvnItem> deletions;
            if (_deletions.TryGetValue(normdir, out deletions))
            {
                deletions = RefreshDeletionsList(deletions);
                _deletions[normdir] = deletions;
                return deletions;
            }
            else
                return new SvnItem[] { };
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
            string normPath = PathUtils.NormalizePath(e.Path, this.currentPath);

            e.Detach();

            // is there already an item for this path?
            SvnItem existingItem;
            AnkhStatus status = new AnkhStatus(e);

            if (_map.TryGetValue(normPath, out existingItem) && !existingItem.IsUnversionable)
                existingItem.RefreshTo(status);
            else
                _map[normPath] = new SvnItem(context, e.Path, status);

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

        private List<SvnItem> RefreshDeletionsList(List<SvnItem> deletions)
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

        private int cacheHits = 0;
        private int cacheMisses = 0;

        readonly IAnkhServiceProvider context;
        private Dictionary<string, List<SvnItem>> _deletions;
        private SvnClient client;
        private string currentPath;
        readonly Dictionary<string, SvnItem> _map;
    }
}
