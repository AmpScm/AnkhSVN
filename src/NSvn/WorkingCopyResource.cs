// $Id$
using System;
using System.IO;
using System.Collections;
using NSvn.Core;
using System.Diagnostics;


namespace NSvn
{
    /// <summary>
    /// Represents an resource(file or directory) in a working copy.
    /// </summary>
    public abstract class WorkingCopyResource : SvnResource, ILocalResource
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">Path to the resource.</param>
        protected WorkingCopyResource( string path )
        {
            this.path = System.IO.Path.GetFullPath(path);
        }

        /// <summary>
        /// Creates a WorkingCopyResource object from a given path..
        /// </summary>
        /// <param name="path">Path to the resource.</param>
        /// <returns>An WorkingCopyresource object representing the resource.</returns>
        public static WorkingCopyResource FromPath( string path )
        {
            return FromPath( path, null );
        }
        
        /// <summary>
        /// Accepts an ILocalResourceVisitor.
        /// </summary>
        /// <param name="visitor"></param>
        public abstract void Accept( ILocalResourceVisitor visitor );

        /// <summary>
        /// Commit local changes to the repository.
        /// </summary>
        /// <param name="recursive">Whether subresources should be recursively committed.</param>
        public CommitInfo Commit( bool recursive )
        {
            return Client.Commit( new string[]{ this.Path }, !recursive, this.ClientContext );
        }

        /// <summary>
        /// Commit local changes to the repository.
        /// </summary>
        /// <param name="resources">The resources to commit.</param>
        /// <param name="recursive">Whether subitems should be recursively committed.</param>
        public static CommitInfo Commit( WorkingCopyResource[] resources, bool recursive )
        {
            string[] targets = GetPaths(resources);

            return Client.Commit( targets, recursive, resources[0].ClientContext );
        }


        /// <summary>
        /// Reverts resources to the state before they where modified.
        /// </summary>
        /// <param name="recursive">Whether subitems should be recursively reverted</param>
        public void Revert( bool recursive )
        {
            Client.Revert( this.Path, recursive, this.ClientContext );          
        }

        /// <summary>
        /// Reverts resources to the state before they where modified.
        /// </summary>
        /// <param name="resources">The resource to revert</param>
        /// <param name="recursive">Whether subitems should be reverted recursively</param>
        public static void Revert( WorkingCopyResource resource, bool recursive )
        {
            Client.Revert( resource.Path, recursive, resource.ClientContext);     
        }    

        /// <summary>
        /// Deletes resources from the working copy.
        /// </summary>
        /// <param name="force">Whether items should be deleted with force.</param>
        public void Remove( bool force )
        {
            Client.Delete( new string[]{ this.Path }, force, this.ClientContext );
        }

        /// <summary>
        /// Deletes resources from the working copy.
        /// </summary>
        /// <param name="resource">Items to be deleted.</param>
        /// <param name="force">Whether items should be deleted with force.</param>
        public static void Remove( WorkingCopyResource[] resources, bool force)
        {
            if ( resources.Length == 0 )
                return;

            string[] paths = GetPaths( resources );
            Client.Delete( paths, force, resources[0].ClientContext );
        }
                
        /*
                /// <summary>
                /// Copy resource to repository.
                /// </summary>
                /// <param name="logMessage">The log message to accompany the commit.</param>
                /// <param name="revision">The revision to copy to</param>
                /// <param name="dstPath">Path to copy to</param>
                 public void CopyTo( RepositoryResource dstUrl, string logMessage, Revision revision )
                {
                    this.CopyTo( dstUrl, new SimpleLogMessageProvider(logMessage), revision );
                }

                /// <summary>
                /// Copy resource to repository.
                /// </summary>
                /// <param name="logMessageProvider">An object that can provide a log message.</param>
                /// <param name="revision">The revision to copy to</param>
                /// <param name="dstPath">Path to copy to</param>
                public RepositoryResource CopyTo( RepositoryResource dstUrl, ILogMessageProvider logMessage, Revision revision  )
                {
                    this.logMessageProvider = logMessageProvider;
                    Client.Copy( this.Path, revision, dstUrl.ToString(), this.ClientContext );
                    return RepositoryResource.ReferenceEquals (this.Path );   
                }*/

        /// <summary>
        /// Copy resource to working copy file.
        /// </summary>
        /// <param name="dstPath">Path to copy to</param>
        public WorkingCopyResource CopyTo( WorkingCopyResource dstPath, Revision revision )
        {
            Client.Copy( this.Path, revision, dstPath.Path, this.ClientContext );
            return WorkingCopyResource.FromPath( this.Path );   
        }

        /// <summary>
        /// Updates the resource recursively to the HEAD revision in the repository.
        /// </summary>
        public void Update()
        {
            this.Update( Revision.Head, true );
        }

        /// <summary>
        /// Updates the resource to the given revision
        /// </summary>
        /// <param name="revision">The revision to update to</param>
        /// <param name="recursive">Whether subresources of the resource should
        /// also be updated</param>
        public void Update( Revision revision, bool recursive )
        {
            Client.Update( this.Path, revision, recursive, this.ClientContext );
        }

        /// <summary>
        /// Performs a diff against the text base.
        /// </summary>
        /// <param name="outStream"></param>
        /// <param name="errStream"></param>
        public void Diff( Stream outStream, Stream errStream )
        {
            Client.Diff( new string[]{}, this.Path, Revision.Base, this.Path,
                Revision.Working, false, true, false, outStream, errStream, 
                this.ClientContext );
        }

        /// <summary>
        /// The status of the resource.
        /// </summary>
        public Status Status
        {
            get
            {
                try
                {
                    this.CheckForModifications();
                    if ( this.status == null )
                    {
                        Trace.WriteLine( "Refreshing status for " + this.path, "NSvn" );
                        this.status = Client.SingleStatus( this.Path );
                    }
                    return this.status;
                }
                catch( IOException ioex )
                {
                    throw new StatusException( "Could not retrieve status for " + this.Path, ioex );
                }
            }
        }

        /// <summary>
        /// The local path of the resource.
        /// </summary>
        public string Path
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.path; }
        }

        /// <summary>
        /// Is this a versioned resource?
        /// </summary>
        public bool IsVersioned
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return true; }
        }

        /// <summary>
        /// Is this a directory?
        /// </summary>
        public abstract bool IsDirectory
        {
            get;
        }



        /// <summary>
        /// Creates a WorkingCopyResource object from a given path..
        /// </summary>
        /// <param name="path">Path to the resource.</param>
        /// <param name="status">A status object with which to initialize the object.</param>
        /// <returns>An WorkingCopyresource object representing the resource.</returns>
        internal static WorkingCopyResource FromPath( string path, Status status )
        {
            WorkingCopyResource resource;
            if ( System.IO.File.Exists( path ) )
                resource = new WorkingCopyFile( path );
            else if ( System.IO.Directory.Exists( path ) )
                resource = new WorkingCopyDirectory( path );
            else
                throw new ArgumentException( "Path must be a file or a directory: " + path, "path" );  

            resource.status = status;
            return resource;
        }

        /// <summary>
        /// Checks if the resource has been modified since information was
        /// retrieved from it. Invalidates such information if so.
        /// </summary>
        protected void CheckForModifications()
        {
            DateTime fileTime = this.LastWriteTime;
            DateTime adminTime = this.AdminAreaWriteTime;
            if ( fileTime > this.lastModTime || adminTime > this.lastModTime )
            {
                //reset the lastModTime
                this.lastModTime = adminTime > fileTime ? adminTime : fileTime;

                // we'll have to get a new status...
                this.status = null;
                //                Trace.WriteLine( "Cached value for WorkingCopyResource object invalidated",
                //                    "NSvn" );
                this.DoInvalidate();
            }
        }

        /// <summary>
        /// Get the paths from a set of WorkingCopyResource objects.
        /// </summary>
        /// <param name="resources"></param>
        /// <returns>An array of strings containing the paths.</returns>
        protected static string[] GetPaths(WorkingCopyResource[] resources)
        {
            ArrayList targets = new ArrayList();
            foreach( WorkingCopyResource resource in resources )
                targets.Add( resource.Path );
            return (string[])targets.ToArray( typeof( string ) );
        }


        /// <summary>
        /// A method that can be overridden by derived classes in order to invalidate
        /// instance data and reset modification times.
        /// </summary>
        protected virtual void DoInvalidate()
        {
            // empty here
        }

        /// <summary>
        /// Whether the resource has been modified.
        /// </summary>
        protected abstract DateTime LastWriteTime
        {
            get;
        }

        /// <summary>
        /// Returns the timestamp on the administrative directory of this
        /// resource.
        /// </summary>
        protected abstract DateTime AdminAreaWriteTime
        {
            get;
        }

        

        protected const string ADMIN_AREA = ".svn";
        protected const int MODIFICATION_DELTA = 5;
        private string path;
        private Status status;
        private DateTime lastModTime;
    }
}
