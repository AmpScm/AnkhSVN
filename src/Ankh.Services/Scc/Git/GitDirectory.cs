using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Ankh.Scc
{
    public interface IGitDirectoryUpdate
    {
        void TickAll();

        void Store(GitItem item);

        bool ScheduleForCleanup { get; }
    }

    public sealed class GitDirectory : KeyedCollection<string, GitItem>, IGitDirectoryUpdate
    {
        readonly IAnkhServiceProvider _context;
        readonly string _fullPath;

        public GitDirectory(IAnkhServiceProvider context, string fullPath)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentNullException("fullPath");

            _context = context;
            _fullPath = fullPath;
        }

        protected override string GetKeyForItem(GitItem item)
        {
            return item.FullPath;
        }

        /// <summary>
        /// Gets the directory item
        /// </summary>
        /// <value>The directory.</value>
        public GitItem Directory
        {
            [DebuggerStepThrough]
            get
            {
                if (Contains(_fullPath))
                    return this[_fullPath]; // 99.9% case
                else
                {
                    // Get the item from the status cache
                    IGitStatusCache cache = _context.GetService<IGitStatusCache>();

                    if (cache == null)
                        return null;

                    GitItem item = cache[_fullPath];

                    if (item != null)
                    {
                        if (!Contains(_fullPath))
                            Add(item); // In most cases the file is added by the cache
                    }

                    return item;
                }
            }
        }

        /// <summary>
        /// Gets the full path of the directory
        /// </summary>
        /// <value>The full path.</value>
        public string FullPath
        {
            [DebuggerStepThrough]
            get
            {
                if (Contains(_fullPath))
                    return this[_fullPath].FullPath;
                else
                    return _fullPath;
            }
        }

        /// <summary>
        /// Tick all items
        /// </summary>
        void IGitDirectoryUpdate.TickAll()
        {
            foreach (IGitItemUpdate item in this)
            {
                item.TickItem();
            }
        }

        void IGitDirectoryUpdate.Store(GitItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Remove(item.FullPath);
            Add(item);
        }

        bool IGitDirectoryUpdate.ScheduleForCleanup
        {
            get
            {
                foreach (GitItem item in this)
                {
                    if (((IGitItemUpdate)item).IsItemTicked())
                        return true;
                }

                return false;
            }
        }
    }
}
