using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Ankh.Scc.Engine;
using Ankh.Scc.Git;

namespace Ankh.Scc
{
    public interface IGitDirectoryUpdate
    {
        void TickAll();

        void Store(GitItem item);

        bool ScheduleForCleanup { get; }
    }

    public sealed class GitDirectory : SccDirectory<GitItem>, IGitDirectoryUpdate
    {
        readonly IAnkhServiceProvider _context;

        public GitDirectory(IAnkhServiceProvider context, string fullPath)
            : base(fullPath)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentNullException("fullPath");

            _context = context;
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
                if (Contains(FullPath))
                    return this[FullPath]; // 99.9% case
                else
                {
                    // Get the item from the status cache
                    IGitStatusCache cache = _context.GetService<IGitStatusCache>();

                    if (cache == null)
                        return null;

                    GitItem item = cache[FullPath];

                    if (item != null)
                    {
                        if (!Contains(FullPath))
                            Add(item); // In most cases the file is added by the cache
                    }

                    return item;
                }
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
