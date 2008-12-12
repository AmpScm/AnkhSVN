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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using System.Collections.ObjectModel;

namespace Ankh.Scc
{
    /// <summary>
    /// A bunch of <see cref="Predicate{SvnItem}"/> methods
    /// </summary>
    public static class SvnItemFilters
    {
        public static bool Evaluate(SvnItem item, Predicate<SvnItem> filter)
        {
            if (item == null)
                return false;
            if (filter == null)
                return true;

            foreach (Predicate<SvnItem> i in filter.GetInvocationList())
            {
                if (!i(item))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Filter for conflicted items.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool ConflictedFilter(SvnItem item)
        {
            return item.Status.LocalContentStatus == SvnStatus.Conflicted;
        }

        /// <summary>
        /// A ResourceFilterCallback method that filters for modified items.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool ModifiedFilter(SvnItem item)
        {
            return item.IsModified;
        }

        /// <summary>
        /// A ResourceFilterCallback that filters for versioned directories.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool DirectoryFilter(SvnItem item)
        {
            return item.IsVersioned && item.IsDirectory;
        }

        public static bool VersionedFilter(SvnItem item)
        {
            return item.IsVersioned;
        }

        public static bool UnversionedFilter(SvnItem item)
        {
            return !item.IsVersioned;
        }

        public static bool UnmodifiedSingleFileFilter(SvnItem item)
        {
            return item.IsVersioned && !item.IsModified && item.IsFile;
        }

        public static bool UnmodifiedItemFilter(SvnItem item)
        {
            return item.IsVersioned && !item.IsModified;
        }

        public static bool VersionedSingleFileFilter(SvnItem item)
        {
            return item.IsVersioned && item.IsFile;
        }

        public static bool LockedFilter(SvnItem item)
        {
            return item.IsLocked;
        }

        public static bool NotLockedAndLockableFilter(SvnItem item)
        {
            return item.IsVersioned && !item.IsLocked && item.IsFile;
        }

        public static bool ExistingOnDiskFilter(SvnItem item)
        {
            return item.Exists;
        }

        public static bool NotDeletedFilter(SvnItem item)
        {
            return !item.IsDeleteScheduled && item.Exists;
        }

        public static bool NoFilter(SvnItem item)
        {
            return true;
        }

        /// <summary>
        /// Filters a list of SvnItem instances using the provided callback.
        /// </summary>
        /// <param name="items">An IList containing SvnItem instances.</param>
        /// <param name="callback">A callback to be used to determine whether 
        /// an item should be included in the returned list.</param>
        /// <returns>A new IList of SvnItem instances.</returns>
        public static Collection<SvnItem> Filter(IList items, Predicate<SvnItem> callback)
        {
            List<SvnItem> list = new List<SvnItem>(items.Count);
            foreach (SvnItem item in items)
            {
                if (callback(item))
                    list.Add(item);
            }

            return new Collection<SvnItem>(list);
        }
    }
}
