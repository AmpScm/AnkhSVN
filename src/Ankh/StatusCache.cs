// $Id$
using System;
using NSvn.Core;
using System.Collections;
using System.IO;
using System.Diagnostics;

namespace Ankh.Solution
{
    /// <summary>
    /// Maintains path->SvnItem mappings.
    /// </summary>
    internal class StatusCache
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
                string normPath = this.NormalizePath(path);
                SvnItem item = (SvnItem)this.table[normPath];

                if ( item == null )
                {
                    System.Diagnostics.Debug.WriteLine( 
                        "Cached item not found for " + normPath, "Ankh" );
                    Status status = this.client.SingleStatus( normPath );
                    this.table[normPath] = item = new SvnItem( path, status );
                }
                else
                    System.Diagnostics.Debug.WriteLine( 
                        "Cached item found for " + path,  
                        "Ankh" );

                return item;
            }                
        }

        /// <summary>
        /// Returns a list of deleted items in the specified directory.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public IList GetDeletions( string dir )
        {
            string normdir = this.NormalizePath(dir);
            if ( this.deletions.ContainsKey( normdir ) )
                return (IList)this.deletions[normdir];
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
            string normPath = this.NormalizePath( path );

            
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

                containingDir = this.NormalizePath( containingDir );

                // store the deletions keyed on the parent directory
                IList list = (IList)this.deletions[ containingDir ];
                if ( list == null )
                    list = new ArrayList();
                list.Add( new SvnItem( path, status ) );

                this.deletions[ containingDir ] = list;
            }
        }

        /// <summary>
        /// Make sure all paths are on the same form, lower case and rooted.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string NormalizePath( string path )
        {
            string normPath = path.Replace( "/", "\\" );
            if ( !Path.IsPathRooted( normPath ) )
                normPath = Path.Combine( this.currentPath, normPath );

            if ( normPath[normPath.Length-1] == '\\' )
                normPath = normPath.Substring(0, normPath.Length-1);

            return normPath.ToLower();
        }

        private Hashtable deletions;
        private Hashtable table;
        private Client client;
        private string currentPath;
    }
}
