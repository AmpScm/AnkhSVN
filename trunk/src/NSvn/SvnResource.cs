using System;
using NSvn.Common;
using NSvn.Core;
using System.Collections;
using System.IO;

namespace NSvn
{
	/// <summary>
	/// Base class for all entity classes
	/// </summary>
	public class SvnResource
	{       
        protected SvnResource()
        {
            this.clientContext = new ClientContext();
            this.clientContext.LogMessageCallback = 
                new LogMessageCallback( this.LogMessageCallback );
            this.clientContext.NotifyCallback = 
                new NotifyCallback( this.NotifyCallback );
            this.clientContext.AuthBaton = new AuthenticationBaton();
            this.dispatchMapping = new Hashtable();
        }

        /// <summary>
        /// Create a ILocalResource from a local path.
        /// </summary>
        /// <param name="path">The path to the file/directory.</param>
        /// <returns>An ILocalResource object.</returns>
        public static ILocalResource FromLocalPath( string path )
        {
            if ( !IsVersioned( path ) )
                return null;

            int youngest;
            StatusDictionary dict = Client.Status( out youngest, path, 
                false, true, false, false, new ClientContext() );
            Status status = dict.GetFirst();
            System.Diagnostics.Debug.Assert( status != null, 
                "Couldn't get status for " + path );

            if ( status.TextStatus != StatusKind.Unversioned )
                return WorkingCopyResource.FromPath( path, status );
            else
                return UnversionedResource.FromPath( path );
        }

        /// <summary>
        /// An object dispatching notification events.
        /// </summary>
        public Notifications Notifications
        {
            get
            {
                // lazy initialization - we might not need it
                if ( this.notifications == null )
                {
                    this.notifications = new Notifications();
                }
                return this.notifications;
            }

            set
            {
                this.notifications = value;
            }
        }



        /// <summary>
        /// Gets or sets the collection of authentication providers
        /// used with this resource.
        /// </summary>
        public AuthenticationProviderCollection AuthenticationProviders
        {
            get{ return this.clientContext.AuthBaton.Providers; }
            set{ this.clientContext.AuthBaton.Providers = value; }
        }

        

        /// <summary>
        /// The ClientContext to be used in version control operations
        /// </summary>
        protected ClientContext ClientContext
        {
            get{ return this.clientContext; }
        }

        /// <summary>
        /// Callback receiver for notifications.
        /// </summary>
        /// <param name="notification">The notification.</param>
        private void NotifyCallback( Notification notification )
        {
            if ( this.notifications != null )
                this.notifications.Notify( this, notification );
        }
     
        /// <summary>
        /// Callback receiver for log message requests.
        /// </summary>
        /// <param name="items">The items that will be committed.</param>
        /// <returns>A string containing the log message, or null if the 
        /// commit is canceled.</returns>
        private string LogMessageCallback( CommitItem[] items )
        {
            // no event listeners - no log message
            if ( this.notifications == null )
                return "";
            return this.notifications.LogMessageCallback( this, items );    
        }

        /// <summary>
        /// Checks whether a given path is versioned.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsVersioned( string path )
        {
            string baseDir = File.Exists( path ) ? Path.GetDirectoryName( path ) : 
                path;
            
            return Directory.Exists( Path.Combine( baseDir, WCAREA ) );
        }

        
        
            

        private ClientContext clientContext;
        private IDictionary dispatchMapping;
        private Notifications notifications;
        private const string WCAREA=".svn";

	}
}
