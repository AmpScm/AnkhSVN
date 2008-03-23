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

namespace Ankh
{    
    /// <summary>
    /// Represents a version controlled path on disk, caching it's status.
    /// </summary>
    public sealed class SvnItem : LocalSvnItem
    {
        readonly IAnkhServiceProvider _context;
        readonly string _fullPath;
        bool _dirty;        
        AnkhStatus _status;        
        bool _refreshing;
        int _readOnly;
        int _exists;
        int _mustLock;

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
        }

        /// <summary>
        /// The status of this item.
        /// </summsary>
        public AnkhStatus Status
        {
            get
            {
                using (EnsureClean())
                {
                    return _status;
                }
            }
        }

        /// <summary>
        /// The path of this item.
        /// </summary>
        public string FullPath
        {
            get { return this._fullPath; }
        }

        public string Name
        {
            get { return Path.GetFileName(FullPath); }
        }

        public void Refresh()
        {
            IFileStatusCache statusCache = _context.GetService<IFileStatusCache>();

            bool isFile = GetIsFile();

            statusCache.RefreshMe(this, isFile ? SvnNodeKind.File : SvnNodeKind.Directory);
        }

        /// <summary>
        /// Set the status of this item to the passed in status.
        /// </summary>
        /// <param name="status"></param>
        public void RefreshTo(AnkhStatus status)
        {
            if (status == null)
                throw new ArgumentNullException("status");

            _status = status;
            _dirty = false;
        }

        /// <summary>
        /// Is this item versioned?
        /// </summary>
        public bool IsVersioned
        {
            get
            {
                using (EnsureClean())
                {
                    return GetIsVersioned();
                }
            }
        }

        bool GetIsVersioned()
        {
            SvnStatus s = _status.LocalContentStatus;
            return s == SvnStatus.Added ||
                   s == SvnStatus.Conflicted ||
                   s == SvnStatus.Merged ||
                   s == SvnStatus.Modified ||
                   s == SvnStatus.Normal ||
                   s == SvnStatus.Replaced ||
                   s == SvnStatus.Deleted ||
                //s == StatusKind.Missing ||
                   s == SvnStatus.Incomplete;
        }

        /// <summary>
        /// Is this resource modified(implies that it is versioned)?
        /// </summary>
        public bool IsModified
        {
            get
            {
                using (EnsureClean())
                {
                    SvnStatus t = _status.LocalContentStatus;
                    SvnStatus p = _status.LocalPropertyStatus;
                    return GetIsVersioned() &&
                        (t != SvnStatus.Normal ||
                          (p != SvnStatus.None && p != SvnStatus.Normal));
                }
            }
        }

        /// <summary>
        /// Is this item a directory?
        /// </summary>
        public bool IsDirectory
        {
            get
            {
                using (EnsureClean())
                {
                    if (_status.NodeKind == SvnNodeKind.Directory || _status.NodeKind == SvnNodeKind.File)
                        return (_status.NodeKind == SvnNodeKind.Directory);
                    else
                        return Directory.Exists(this._fullPath);
                }
            }
        }

        /// <summary>
        /// Is this item a file?
        /// </summary>
        public bool IsFile
        {
            get
            {
                using (EnsureClean())
                {
                    return GetIsFile();
                }
            }
        }

        bool GetIsFile()
        {
            if (_status.NodeKind == SvnNodeKind.Directory || _status.NodeKind == SvnNodeKind.File)
                return _status.NodeKind == SvnNodeKind.File;
            else
                return File.Exists(this._fullPath);
        }

        /// <summary>
        /// Whether the item is potentially versionable.
        /// </summary>
        public bool IsVersionable
        {
            get
            {
                return SvnTools.IsBelowManagedPath(_fullPath);
            }
        }

        /// <summary>
        /// Whether the item is read only on disk.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                if (_readOnly == 0)
                {
                    if(this.IsFile && this.Exists)
                        _readOnly = ((File.GetAttributes(_fullPath) & FileAttributes.ReadOnly) != 0) ? 2 : 1;
                }
                return (_readOnly == 2);
            }
        }

        public bool Exists
        {
            get
            {
                if (_exists == 0)
                {
                    if (IsDirectory)
                        _exists = Directory.Exists(_fullPath) ? 2 : 1;
                    else if (IsFile)
                        _exists = File.Exists(_fullPath) ? 2 : 1;
                    else
                        _exists = 3;
                }
                return (_exists == 2);
            }
        }

        public bool ReadOnlyMustLock
        {
            get
            {
                if (_mustLock == 0)
                {
                    _mustLock = 1;

                    // Subversions sets must-lock files to read only; checking it without
                    // checking the readonly flag first costs to much time for us.
                    if (IsFile && IsReadOnly)
                    {
                        _mustLock = 1;
                        using (SvnClient client = _context.GetService<ISvnClientPool>().GetNoUIClient())
                        {
                            string propVal;
                            if (client.TryGetProperty(new SvnPathTarget(_fullPath), SvnPropertyNames.SvnNeedsLock, out propVal))
                            {
                                _mustLock = 2;
                            }
                        }
                    }
                }

                return (_mustLock == 2);
            }
        }

        /// <summary>
        /// Whether the item is locked
        /// </summary>
        /// <returns></returns>
        public bool IsLocked
        {
            get
            {
                using (EnsureClean())
                {
                    return _status.IsLockedLocal;
                }
            }
        }

        public bool InConflict
        {
            get
            {
                using (EnsureClean())
                {
                    return (_status.LocalContentStatus == SvnStatus.Conflicted) ||
                        (_status.LocalPropertyStatus == SvnStatus.Conflicted);
                }
            }
        }

        public bool IsDeleted
        {
            get
            {
                using (EnsureClean())
                {
                    return _status.LocalContentStatus == SvnStatus.Deleted;
                }
            }
        }

        public bool IsIgnored
        {
            get
            {
                using (EnsureClean())
                {
                    return (_status.LocalContentStatus == SvnStatus.Ignored);
                }
            }
        }

        public override string ToString()
        {
            return this._fullPath;
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

        /// <summary>
        /// Filters a list of SvnItem instances using the provided callback.
        /// </summary>
        /// <param name="items">An IList containing SvnItem instances.</param>
        /// <param name="callback">A callback to be used to determine whether 
        /// an item should be included in the returned list.</param>
        /// <returns>A new IList of SvnItem instances.</returns>
        public static IList Filter(IList items, Predicate<SvnItem> callback)
        {
            ArrayList list = new ArrayList(items.Count);
            foreach (SvnItem item in items)
            {
                if (callback(item))
                    list.Add(item);
            }

            return list;
        }               

        Cleaner EnsureClean()
        {
            Trace.Assert(!_refreshing, "Recursive refresh call");

            if (_dirty)
                this.Refresh();

#if TRACE
            return new Cleaner(this);
#else
            return null; // Optimized away
#endif
        }

        public void MarkDirty()
        {
            _dirty = true;
            _readOnly = 0;
            _exists = 0;
            _mustLock = 0; 
        }

        public bool IsDirty
        {
            get { return _dirty; }
        }

        #region Cleaner
        sealed class Cleaner : IDisposable
        {
            SvnItem _instance;

            public Cleaner(SvnItem instance)
            {
                _instance = instance;
            }
            public void Dispose()
            {
                _instance._refreshing = false;
            }
        }
        #endregion

        public void Dispose()
        {
            // TODO: Mark item as no longer valid
        }
    }
}
