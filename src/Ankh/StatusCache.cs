// $Id$
using System;

using System.Collections;
using System.IO;
using System.Diagnostics;
using Utils;
using SharpSvn;
using Utils.Services;
using System.Collections.ObjectModel;

namespace Ankh
{
    /// <summary>
    /// Maintains path->SvnItem mappings.
    /// </summary>
    public class StatusCache
    {
        public StatusCache( SvnClient client )
        {
            this.client = client;
            this.table = new Hashtable();
            this.deletions = new Hashtable();
        }

        /// <summary>
        /// Fill the cache by running status recursively on this directory.
        /// </summary>
        /// <param name="dir"></param>
	    [Obsolete("Assumes SvnDepth.Children")]
	    public void Status( string dir )
        {
            this.Status( dir, SvnDepth.Children );
        }

        /// <summary>
        /// Fill the cache by running status on this directory.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="recurse">Whether to recurse to subdirectories.</param>
        public void Status( string dir, SvnDepth depth )
        {
            lock(this)
            {
                Debug.WriteLine( 
                    String.Format("Generating {0} status cache for {1}",
                    depth, dir), 
                    "Ankh" );
                if ( !AnkhServices.GetService<IWorkingCopyOperations>().IsWorkingCopyPath(dir) )
                {
                    foreach (string file in Directory.GetFiles( dir ) )
                    {
                        string normPath = PathUtils.NormalizePath(file);
                        SvnItem existingItem = (SvnItem)this.table[normPath];
                        if ( existingItem != null )
                            existingItem.Refresh( AnkhStatus.None );
                        else
                            this.table[normPath] = SvnItem.Unversionable;
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

        public SvnItem this[string path]
        {
            get
            {
                string normPath = PathUtils.NormalizePath(path, this.currentPath);
                SvnItem item = (SvnItem)this.table[normPath];

                if ( item == null )
                {
                    System.Diagnostics.Debug.WriteLine( 
                        "Cached item not found for " + normPath, "Ankh" );
                    this.cacheMisses++;

                    // fill the status cache from this directory
                    string directory = normPath;
                    if ( File.Exists( directory ) )
                        directory = Path.GetDirectoryName( directory );

                    if ( Directory.Exists( directory ) )
                        this.Status( directory, SvnDepth.Children );

                    item = (SvnItem)this.table[ normPath ];
                    if (item == null)
                    {
                        Collection<SvnStatusEventArgs> statuses;
                        SvnStatusArgs args = new SvnStatusArgs();
                        args.ThrowOnError = false;
                        args.RetrieveAllEntries = true;
                        if (this.client.GetStatus(normPath, args, out statuses) && statuses.Count > 0)
                        {
                            this.table[normPath] = item = new SvnItem(path, statuses[0]);
                        }
                        else
                            this.table[normPath] = item = new SvnItem(path, AnkhStatus.None);
                    }
                }
                else
                {
                    this.cacheHits++;
                    System.Diagnostics.Debug.WriteLine( 
                        "Cached item found for " + path,  
                        "Ankh" );
                }

                return item;
            }                
        }

        /// <summary>
        /// Current success rate of the cache.
        /// </summary>
        public float CacheHitSuccess
        {
            get
            { 
                // we don't wanna divide by zero
                if ( this.cacheHits + this.cacheMisses > 0 )
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
        public IList GetDeletions( string dir )
        {
            string normdir = PathUtils.NormalizePath(dir, this.currentPath);
            if ( this.deletions.ContainsKey( normdir ) )
            {
                IList deletions = (IList)this.deletions[normdir];
                deletions = RefreshDeletionsList(deletions);
                this.deletions[normdir] = deletions;
                return deletions;
            }
            else
                return new SvnItem[]{};
        }

      

        /// <summary>
        /// Status callback.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="status"></param>
        private void Callback(object sender, SvnStatusEventArgs e)
        {
            Debug.WriteLine( "Received status for " + e.Path + ": " + e.LocalContentStatus, 
                "Ankh" );
            // we need all paths to be on ONE form
            string normPath = PathUtils.NormalizePath( e.Path, this.currentPath );

            // is there already an item for this path?
            SvnItem existingItem = (SvnItem)this.table[ normPath ];
            if ( existingItem != null && existingItem != SvnItem.Unversionable )
                existingItem.Refresh( e );
            else
                this.table[ normPath ] = new SvnItem( e.Path, e );

            if ( e.LocalContentStatus == SvnStatus.Deleted )
            {
                string containingDir;
                containingDir = GetNormalizedParentDirectory(e.Path);

                // store the deletions keyed on the parent directory
                IList list = (IList)this.deletions[ containingDir ];
                if ( list == null )
                    list = new ArrayList();
                SvnItem deletedItem = (SvnItem)this.table[ normPath ];
                if ( !list.Contains( deletedItem ) )
                {
                    list.Add( deletedItem );
                }

                this.deletions[ containingDir ] = list;
            }
        }

        //private void DeletedItemChanged(object sender, EventArgs e)
        //{
        //    SvnItem deletedItem = (SvnItem)sender;
        //    string parentDir = this.GetNormalizedParentDirectory( deletedItem.Path );

        //    // are we tracking this item? we should
        //    IList dirDeletions = (IList)this.deletions[ parentDir ];
        //    if ( dirDeletions == null )
        //    {
        //        Debug.WriteLine( "Event raised from deleted item we're not tracking" );
        //        return;
        //    }

        //    // get rid of the item from the directory
        //    dirDeletions.Remove( deletedItem );

        //    // is the list for that directory now empty?
        //    if ( dirDeletions.Count == 0 )
        //    {
        //        dirDeletions.Remove( parentDir );
        //    }

        //    // if it's anything but deleted, put it in the regular table
        //    if ( deletedItem.Status.TextStatus != StatusKind.None )
        //    {
        //        this.table[ deletedItem.Path ] = deletedItem;                
        //    }
        //}

        private string GetNormalizedParentDirectory(string path)
        {
            string containingDir;
            // find the parent directory
            if ( path[path.Length-1 ] == '\\' )
            {
                containingDir = Path.GetDirectoryName(
                    path.Substring(0, path.Length-1) );
            }
            else
            {
                containingDir = Path.GetDirectoryName( path );
            }

            containingDir = PathUtils.NormalizePath( containingDir, this.currentPath );
            return containingDir;
        }

        private IList RefreshDeletionsList(IList deletions)
        {
            ArrayList newList = new ArrayList();

            foreach (SvnItem item in deletions)
            {
                item.Refresh(this.client);
                if (item.Status.LocalContentStatus == SvnStatus.Deleted)
                    newList.Add(item);
            }

            return newList;
        }

        private int cacheHits = 0;
        private int cacheMisses = 0;
        

        private Hashtable deletions;
        private Hashtable table;
        private SvnClient client;
        private string currentPath;


#if DEBUG
        internal void ScanForOrphanedTreeNodes( TextWriter writer )
        {
            int numOrphans = 0;
            foreach ( SvnItem item in this.table.Values )
            {
                numOrphans += item.ScanForOrphanedTreeNodes( writer );
            }

            writer.WriteLine( "Found {0} orphaned nodes.", numOrphans );
        }
#endif
    }
}
