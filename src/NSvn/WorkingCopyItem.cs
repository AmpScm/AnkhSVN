using System;
using NSvn.Core;

namespace NSvn
{
	/// <summary>
	/// Represents an item(file or directory) in a working copy.
	/// </summary>
	public class WorkingCopyItem : ILocalItem
	{
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">Path to the item.</param>
		public WorkingCopyItem( string path )
		{
			this.path = path;
            this.clientContext = new ClientContext();
            this.clientContext.LogMessageCallback = new LogMessageCallback( this.LogMessageCallback );
		}


        /// <summary>
        /// Commit local changes to the repository.
        /// </summary>
        /// <param name="logMessage">The log message to accompany the commit.</param>
        /// <param name="recursive">Whether subitems should be recursively committed.</param>
        public void Commit( string logMessage, bool recursive )
        {
            this.Commit( new SimpleLogMessageProvider(logMessage), recursive );
        }

        /// <summary>
        /// Commit local changes to the repository.
        /// </summary>
        /// <param name="logMessageProvider">An object that can provide a log message.</param>
        /// <param name="recursive">Whether subitems should be recursively committed.</param>
        public void Commit( ILogMessageProvider logMessageProvider, bool recursive )
        {
            this.logMessageProvider = logMessageProvider;
            Client.Commit( new string[]{ this.Path }, recursive, this.ClientContext );
        }

        /// <summary>
        /// Updates the item recursively to the HEAD revision in the repository.
        /// </summary>
        public void Update()
        {
            this.Update( Revision.Head, true );
        }

        /// <summary>
        /// Updates the item to the given revision
        /// </summary>
        /// <param name="revision">The revision to update to</param>
        /// <param name="recursive">Whether subitems of the item should
        /// also be updated</param>
        public void Update( Revision revision, bool recursive )
        {
            Client.Update( this.Path, revision, recursive, this.ClientContext );
        }


        /// <summary>
        /// The status of the item.
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
        /// The local path of the item.
        /// </summary>
        public string Path
        {
            get{ return this.path; }
        }

        /// <summary>
        /// The ClientContext object used by this item.
        /// </summary>
        protected ClientContext ClientContext
        {
            get{ return this.clientContext; }
        }

        
        private string LogMessageCallback( CommitItem[] targets )
        {
            return this.logMessageProvider.GetLogMessage( targets );
        }


        private string path;
        private ClientContext clientContext;
        private ILogMessageProvider logMessageProvider;
	}
}
