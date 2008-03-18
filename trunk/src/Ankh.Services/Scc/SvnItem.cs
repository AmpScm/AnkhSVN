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

namespace Ankh
{    
    [Obsolete]
    public enum EventBehavior
    {
        Raise,
        DontRaise,
    }

    /// <summary>
    /// Represents a version controlled path on disk, caching it's status.
    /// </summary>
    public class SvnItem : LocalSvnItem
    {
        readonly IAnkhServiceProvider _context;
        readonly string _fullPath;
        bool _dirty;        
        AnkhStatus _status;        
        bool _refreshing;

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
        public string Path
        {
            get { return this._fullPath; }
        }

        public string Name
        {
            get { return System.IO.Path.GetFileName(Path); }
        }

        public void Refresh()
        {
            ISvnClientPool clientPool = _context.GetService<ISvnClientPool>();
            IFileStatusCache statusCache = _context.GetService<IFileStatusCache>();

            string path = this._fullPath;

            // if it's a single file, refresh the directory with Files unrecursively
            if (GetIsFile())
                path = System.IO.Path.GetDirectoryName(path);

            SvnStatusArgs args = new SvnStatusArgs();
            args.Depth = SvnDepth.Files;
            args.RetrieveAllEntries = true;
            args.ThrowOnError = false;

            using (SvnClient client = clientPool.GetNoUIClient())
            {
                if (client != null)
                {
                    client.Status(path, args, delegate(object sender, SvnStatusEventArgs e)
                    {
                        SvnItem i = statusCache[e.Path];
                        if (i != null)
                        {
                            i.RefreshTo(new AnkhStatus(e));
                        }
                    });
                }
            }
        }



        /// <summary>
        /// Set the status of this item to the passed in status.
        /// </summary>
        /// <param name="status"></param>
        public virtual void RefreshTo(AnkhStatus status)
        {
            if (status == null)
                throw new ArgumentNullException("status");

            _status = status;
            _dirty = false;
        }

        /// <summary>
        /// Is this item versioned?
        /// </summary>
        public virtual bool IsVersioned
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
        public virtual bool IsModified
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
        public virtual bool IsDirectory
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
        public virtual bool IsFile
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
        public virtual bool IsVersionable
        {
            get
            {
                using (EnsureClean())
                {
                    return SvnTools.IsBelowManagedPath(_fullPath);
                }
            }
        }

        /// <summary>
        /// Whether the item is read only on disk.
        /// </summary>
        public virtual bool IsReadOnly
        {
            get
            {
                try
                {
                    return this.IsFile && File.Exists(this.Path) &&
                        (File.GetAttributes(this.Path) & FileAttributes.ReadOnly) != 0;
                }
                catch (IOException)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Whether the item is locked
        /// </summary>
        /// <returns></returns>
        public virtual bool IsLocked
        {
            get
            {
                using (EnsureClean())
                {
                    return _status.LocalLocked;
                }
            }
        }


        public virtual bool IsDeleted
        {
            get
            {
                using (EnsureClean())
                {
                    return _status.LocalContentStatus == SvnStatus.Deleted;
                }
            }
        }

        public virtual bool IsDeletedFromDisk
        {
            get
            {
                using (EnsureClean())
                {
                    return _status.LocalContentStatus == SvnStatus.None && !File.Exists(this.Path) && !Directory.Exists(this.Path);
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
                    paths[i++] = item.Path;
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

        private sealed class UnversionableItem : SvnItem
        {
            public UnversionableItem(IAnkhServiceProvider context, string path)
                : base(context, path, AnkhStatus.Unversioned)
            { }  

            public override void RefreshTo(AnkhStatus status)
            {
                // empty
            }

            public override bool IsDirectory
            {
                get
                {
                    return false;
                }
            }

            public override bool IsFile
            {
                get
                {
                    return false;
                }
            }

            public override bool IsModified
            {
                get
                {
                    return false;
                }
            }

            public override bool IsVersioned
            {
                get
                {
                    return false;
                }
            }

            public override bool IsVersionable
            {
                get
                {
                    return false;
                }
            }

            public override bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public override bool IsDeleted
            {
                get
                {
                    return false;
                }
            }

            public override bool IsDeletedFromDisk
            {
                get
                {
                    return false;
                }
            }

            public override bool IsUnversionable
            {
                get { return true; }
            }
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
        }

        public bool IsDirty
        {
            get { return _dirty; }
        }

        public virtual bool IsUnversionable
        {
            get { return (object)_status == (object)AnkhStatus.Unversioned; }
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
    }
}
