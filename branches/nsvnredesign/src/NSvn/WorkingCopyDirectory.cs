// $Id$
using System;
using NSvn.Common;
using NSvn.Core;
using System.IO;

namespace NSvn
{
    /// <summary>
    /// Represents a versioned directory in an SVN working copy.
    /// </summary>
    [Obsolete("Will be removed in a future version")]
    public class WorkingCopyDirectory : WorkingCopyResource
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        public WorkingCopyDirectory( string path ) : base( path )
        {
        }

        

        /// <summary>
        /// Accepts an ILocalResourceVisitor.
        /// </summary>
        public override void Accept( ILocalResourceVisitor visitor )
        {
            visitor.VisitWorkingCopyDirectory( this );
        }

        /// <summary>
        /// Recursively cleans up the working copy, removing any locks and completes any
        /// unfinished transactions.
        /// </summary>
        public void Cleanup( )
        {
            Client.Cleanup( this.Path, this.ClientContext );
        }

        /// <summary>
        /// Is this a directory?
        /// </summary>
        public override bool IsDirectory
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return true; }
        }

        /// <summary>
        /// Returns all the child objects of this resource.
        /// </summary>
        public LocalResourceDictionary Children
        {
            get
            {
                this.CheckForModifications();
                if ( this.children == null )
                    this.children = this.GetChildren();
                return this.children;
            }
        }

        /// <summary>
        /// Invalidates instance data of this resource object.
        /// </summary>
        protected override void DoInvalidate()
        {
            this.children = null; 
        }

        /// <summary>
        /// Whether the resource has been modified.
        /// </summary>
        protected override DateTime LastWriteTime
        {
            get
            {
                return Directory.GetLastWriteTime( this.Path );
            }
        }

        /// <summary>
        /// Returns the timestamp on the administrative directory of this
        /// resource.
        /// </summary>
        protected override DateTime AdminAreaWriteTime
        {
            get
            {                
                return Directory.GetLastWriteTime( System.IO.Path.Combine(
                    this.Path, ADMIN_AREA ) );
            }
        }       


        /// <summary>
        /// Retrieve the children of this directory.
        /// </summary>
        /// <returns>A LocalResourceDictionary object containing all the children.</returns>
        private LocalResourceDictionary GetChildren()
        {
            LocalResourceDictionary resDict = new LocalResourceDictionary();
            int youngest;

            // get the status of all subitems - this will effectively give us a list of them
            // and and we get their statuses as an added bonus
            // TODO: should this be recursive?
            StatusBuilder builder = new StatusBuilder();
            Client.Status( out youngest, this.Path, Revision.Unspecified, 
                new StatusCallback( builder.StatusFunc ), false, true, false, 
                false, this.ClientContext );            

            builder.ResDict.Remove( System.IO.Path.GetFileName( this.Path ) );
            return builder.ResDict;
        }

        /// <summary>
        /// Used to build up a list of statuses.
        /// </summary>
        private class StatusBuilder
        {
            public LocalResourceDictionary ResDict = new LocalResourceDictionary();

            public void StatusFunc( string path, Status status )
            {
                string basePath = System.IO.Path.GetFileName( path );

                // is this a versioned or an unversioned resource?
                if ( status.TextStatus == StatusKind.Unversioned )
                    ResDict[ basePath ] = UnversionedResource.FromPath( path );
                else
                    ResDict[ basePath ] = WorkingCopyResource.FromPath( path, status );
            }
        }

        

        private LocalResourceDictionary children;    
    }
}
