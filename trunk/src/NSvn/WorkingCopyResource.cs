using System;
using System.IO;
using NSvn.Core;

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
            //TODO: pull this up
            this.ClientContext.LogMessageCallback = new LogMessageCallback( this.LogMessageCallback );
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
                throw new ArgumentException( "Path must be a file or a directory", "path" );  

            resource.status = status;
            return resource;
        }


        /// <summary>
        /// Commit local changes to the repository.
        /// </summary>
        /// <param name="logMessage">The log message to accompany the commit.</param>
        /// <param name="recursive">Whether subresources should be recursively committed.</param>
        public void Commit( string logMessage, bool recursive )
        {
            this.Commit( new SimpleLogMessageProvider(logMessage), recursive );
        }

        /// <summary>
        /// Commit local changes to the repository.
        /// </summary>
        /// <param name="logMessageProvider">An object that can provide a log message.</param>
        /// <param name="recursive">Whether subresources should be recursively committed.</param>
        public void Commit( ILogMessageProvider logMessageProvider, bool recursive )
        {
            this.logMessageProvider = logMessageProvider;
            Client.Commit( new string[]{ this.Path }, recursive, this.ClientContext );
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
        /// The status of the resource.
        /// </summary>
        public Status Status
        {
            get
            {
                this.CheckForModifications();
                if ( this.status == null )
                {
                    int youngest;
                    StatusDictionary dict = Client.Status( out youngest, this.Path, 
                        false, true, false, true, this.ClientContext );
                    this.status = dict.Get( this.Path );
                }
                return this.status;
            }
        }

        /// <summary>
        /// The local path of the resource.
        /// </summary>
        public string Path
        {
            get{ return this.path; }
        }

        /// <summary>
        /// Is this a versioned resource?
        /// </summary>
        public bool IsVersioned
        {
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

                this.DoInvalidate();
            }
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

        /// <summary>
        /// Callback function for log messages
        /// </summary>
        /// <param name="targets"></param>
        /// <returns></returns>
        private string LogMessageCallback( CommitItem[] targets )
        {
            return this.logMessageProvider.GetLogMessage( targets );
        }

        protected const string ADMIN_AREA = ".svn";
        protected const int MODIFICATION_DELTA = 5;
        private string path;
        private Status status;
        private ILogMessageProvider logMessageProvider;
        private DateTime lastModTime;
	}
}
