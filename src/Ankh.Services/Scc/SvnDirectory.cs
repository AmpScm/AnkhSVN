// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Ankh.Scc
{
    public interface ISvnDirectoryUpdate
    {
        void TickAll();
        void TickFiles();

        void Store(SvnItem item);

        bool ScheduleForCleanup { get; }
    }
    /// <summary>
    /// Collection of <see cref="SvnItem"/> instances of a specific directory
    /// </summary>
    /// <remarks>
    /// <para>A SvnDirectory contains the directory itself and all files and directories contained directly within</para>
    /// 
    /// <para>Note: This tells us that all subdirectories are contained in the parent and in their own <see cref="SvnDirectory"/></para>
    /// </remarks>
    public sealed class SvnDirectory : KeyedCollection<string, SvnItem>, ISvnDirectoryUpdate
    {
        readonly IAnkhServiceProvider _context;
        readonly string _fullPath;

        public SvnDirectory(IAnkhServiceProvider context, string fullPath)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentNullException("fullPath");

            _context = context;
            _fullPath = fullPath;
        }

        protected override string GetKeyForItem(SvnItem item)
        {
            return item.FullPath;
        }

        /// <summary>
        /// Gets the directory item
        /// </summary>
        /// <value>The directory.</value>
        public SvnItem Directory
        {
            [DebuggerStepThrough]
            get 
            {
                if (Contains(_fullPath))
                    return this[_fullPath]; // 99.9% case
                else
                {
                    // Get the item from the status cache
                    IFileStatusCache cache = _context.GetService<IFileStatusCache>();

                    if (cache == null)
                        return null;

                    SvnItem item = cache[_fullPath];

                    if (item != null)
                    {
                        if(!Contains(_fullPath))
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
        void ISvnDirectoryUpdate.TickAll()
        {
            foreach (ISvnItemUpdate item in this)
            {
                item.TickItem();
            }
        }

        /// <summary>
        /// Tick all files and the directory itself
        /// </summary>
        void ISvnDirectoryUpdate.TickFiles()
        {
            foreach (SvnItem item in this)
            {
                if(item.IsFile || item == Directory)
                    ((ISvnItemUpdate)item).TickItem();
            }

            ((ISvnItemUpdate)this.Directory).TickItem();
        }

        void ISvnDirectoryUpdate.Store(SvnItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Remove(item.FullPath);
            Add(item);
        }

        #region ISvnDirectoryUpdate Members

        bool ISvnDirectoryUpdate.ScheduleForCleanup
        {
            get
            {
                foreach (SvnItem item in this)
                {
                    if (((ISvnItemUpdate)item).IsItemTicked())
                        return true;
                }

                return false;
            }
        }

        #endregion
    }
}
