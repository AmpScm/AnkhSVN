using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;

namespace Ankh.Scc
{
    /// <summary>
    /// Used to decide whether this particular SvnItem should be included in a collection.
    /// </summary>
    [Obsolete("Use Predicate<SvnItem>")]
    public delegate bool ResourceFilterCallback(SvnItem item);


    /// <summary>
    /// A bunch of <see cref="Predicate{SvnItem}"/> methods
    /// </summary>
    public static class SvnItemFilters
    {
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
            return !item.IsDeleted && item.Exists;
        }

        public static bool NoFilter(SvnItem item)
        {
            return true;
        }

    }
}
