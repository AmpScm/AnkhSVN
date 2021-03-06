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
using Ankh.Scc.Engine;

namespace Ankh.Scc
{
    public interface ISvnDirectoryUpdate
    {
        void TickAll();

        void Store(SvnItem item);

        bool ScheduleForCleanup { get; }

        void SetNeedsUpgrade();
        void SetNeedsCleanup();
    }
    /// <summary>
    /// Collection of <see cref="SvnItem"/> instances of a specific directory
    /// </summary>
    /// <remarks>
    /// <para>A SvnDirectory contains the directory itself and all files and directories contained directly within</para>
    /// 
    /// <para>Note: This tells us that all subdirectories are contained in the parent and in their own <see cref="SvnDirectory"/></para>
    /// </remarks>
    public sealed class SvnDirectory : SccDirectory<SvnItem>, ISvnDirectoryUpdate
    {
        readonly IAnkhServiceProvider _context;
        bool _needsUpgrade;
        bool _needsCleanup;

        public SvnDirectory(IAnkhServiceProvider context, string fullPath)
            : base(fullPath)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
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
                if (Contains(FullPath))
                    return this[FullPath]; // 99.9% case
                else
                {
                    // Get the item from the status cache
                    ISvnStatusCache cache = _context.GetService<ISvnStatusCache>();

                    if (cache == null)
                        return null;

                    SvnItem item = cache[FullPath];

                    if (item != null)
                    {
                        if(!Contains(FullPath))
                            Add(item); // In most cases the file is added by the cache
                    }

                    return item;
                }
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether this directories working copy needs an explicit upgrade
        /// </summary>
        public bool NeedsWorkingCopyUpgrade
        {
            get { return _needsUpgrade; }
        }

        /// <summary>
        /// Tick all items
        /// </summary>
        void ISvnDirectoryUpdate.TickAll()
        {
            _needsUpgrade = false;
            _needsCleanup = false;
            foreach (ISvnItemUpdate item in this)
            {
                item.TickItem();
            }
        }

        void ISvnDirectoryUpdate.Store(SvnItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Remove(item.FullPath);
            Add(item);
        }

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

        void ISvnDirectoryUpdate.SetNeedsUpgrade()
        {
            _needsUpgrade = true;
        }

        void ISvnDirectoryUpdate.SetNeedsCleanup()
        {
            _needsUpgrade = true;
        }

        public bool NeedsCleanup
        {
            get { return _needsCleanup; }
        }
    }
}
