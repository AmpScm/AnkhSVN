using System;
using NSvn.Common;
using NSvn.Core;
using System.Collections;

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
            this.dispatchMapping = new Hashtable();
        }

        /// <summary>
        /// Create a ILocalResource from a local path.
        /// </summary>
        /// <param name="path">The path to the file/directory.</param>
        /// <returns>An ILocalResource object.</returns>
        public static ILocalResource FromLocalPath( string path )
        {
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
                    this.clientContext.NotifyCallback = new NotifyCallback( 
                        ((INotificationReceiver)this.notifications).Notify);
                }
                return this.notifications;
            }
        }

        /// <summary>
        /// Add an authentication provider.
        /// </summary>
        /// <param name="provider">The provider to add.</param>
        public void AddAuthenticationProvider( IAuthenticationProvider provider )
        {
            if ( this.ClientContext.AuthBaton == null )
                this.clientContext.AuthBaton = new AuthenticationBaton();

            this.clientContext.AuthBaton.Providers.Add( provider );
        }

        /// <summary>
        /// Remove an authentication provider.
        /// </summary>
        /// <param name="provider"></param>
        public void RemoveAuthenticationProvider( IAuthenticationProvider provider )
        {
            if ( this.ClientContext.AuthBaton != null )
                this.clientContext.AuthBaton.Providers.Remove( provider );
        }

        /// <summary>
        /// The ClientContext to be used in version control operations
        /// </summary>
        protected ClientContext ClientContext
        {
            get{ return this.clientContext; }
        }
        
        
            

        private ClientContext clientContext;
        private IDictionary dispatchMapping;
        private Notifications notifications;

	}
}
