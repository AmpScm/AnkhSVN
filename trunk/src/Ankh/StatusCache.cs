// $Id$
using System;
using NSvn.Core;
using System.Collections;
using System.IO;

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
        }

        public void Status( string dir )
        {
            lock(this)
            {
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
                        "Ankh", "Cached item not found for " + normPath );
                    Status status = this.client.SingleStatus( normPath );
                    this.table[normPath] = item = new SvnItem( path, status );
                }
                else
                    System.Diagnostics.Debug.WriteLine( 
                        "Ankh", "Cached item found for " + path );

                return item;
            }                
        }

        /// <summary>
        /// Status callback.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="status"></param>
        private void Callback( string path, Status status )
        {
            // we need all paths to be on ONE form
            string normPath = this.NormalizePath( path );

            // is there already an item for this path?
            SvnItem existingItem = (SvnItem)this.table[normPath];
            if ( existingItem != null )
                existingItem.Refresh( status );
            else
                this.table[normPath] = new SvnItem( path, status );
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

            return normPath.ToLower();
        }

        private Hashtable table;
        private Client client;
        private string currentPath;
    }
}
