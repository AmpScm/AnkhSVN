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
        /// Factory method.
        /// </summary>
        /// <param name="path">Path to the resource.</param>
        /// <returns>An WorkingCopyresource object representing the resource.</returns>
        public static WorkingCopyResource FromPath( string path )
        {
            if ( System.IO.File.Exists( path ) )
                return new WorkingCopyFile( path );
            else if ( System.IO.Directory.Exists( path ) )
                return new WorkingCopyDirectory( path );
            else
                throw new ArgumentException( "Path must be a file or a directory", "path" );  
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
                int youngest;
                StatusDictionary dict = Client.Status( out youngest, this.Path, 
                    false, true, false, true, this.ClientContext );
                return dict.Get( this.Path );
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
        /// Is this a directory
        /// </summary>
        public abstract bool IsDirectory
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


        private string path;
        private ILogMessageProvider logMessageProvider;
	}
}
