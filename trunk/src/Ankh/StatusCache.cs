// $Id$
using System;
using NSvn.Core;
using System.Collections;
using System.IO;
using System.Diagnostics;
using Utils;

namespace Ankh
{
    /// <summary>
    /// Maintains path->SvnItem mappings.
    /// </summary>
    public class StatusCache
    {
        public StatusCache( Client client )
        {
            this.client = client;
            this.table = new Hashtable();
            this.deletions = new Hashtable();
        }

        public void Status( string dir )
        {
            lock(this)
            {
                if ( !SvnUtils.IsWorkingCopyPath(dir) )
                    return;

                Debug.WriteLine( "Generating status cache for " + dir, "Ankh" );
                this.currentPath = dir;
                int youngest;
                this.client.Status( out youngest, dir, Revision.Unspecified, 
                    new StatusCallback( this.Callback ), true, true, false, true );
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
                    Status status = this.client.SingleStatus( normPath );
                    this.table[normPath] = item = new SvnItem( path, status );
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
                // we need to make sure at this point that they are still valid
                IList list = (IList)this.deletions[normdir];
                IList newList = new ArrayList();
                foreach( SvnItem item in list )
                {
                    // this item still valid?
                    item.Refresh(this.client);
                    if ( item.Status.TextStatus == StatusKind.Deleted )
                        newList.Add( item );
                }
                if ( newList.Count > 0 )
                {
                    this.deletions[normdir] = newList;
                    return (IList)this.deletions[normdir];
                }
                else
                {
                    this.deletions.Remove(normdir);             
                    return new SvnItem[]{};
                }
            }
            else
                return new SvnItem[]{};
        }

        /// <summary>
        /// Status callback.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="status"></param>
        private void Callback( string path, Status status )
        {
            Debug.WriteLine( "Received status for " + path + ": " + status.TextStatus, 
                "Ankh" );
            // we need all paths to be on ONE form
            string normPath = PathUtils.NormalizePath( path, this.currentPath );

            
            if ( status.TextStatus != StatusKind.Deleted )
            {
                // is there already an item for this path?
                SvnItem existingItem = (SvnItem)this.table[normPath];
                if ( existingItem != null )
                    existingItem.Refresh( status );
                else
                    this.table[normPath] = new SvnItem( path, status );
            }
            else
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

                // store the deletions keyed on the parent directory
                IList list = (IList)this.deletions[ containingDir ];
                if ( list == null )
                    list = new ArrayList();
                list.Add( new SvnItem( path, status ) );

                this.deletions[ containingDir ] = list;
            }
        }

        private int cacheHits = 0;
        private int cacheMisses = 0;
        

        private Hashtable deletions;
        private Hashtable table;
        private Client client;
        private string currentPath;
    }
}
