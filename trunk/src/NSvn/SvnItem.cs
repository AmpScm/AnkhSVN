using System;
using NSvn.Common;
using NSvn.Core;
using System.Collections;

namespace NSvn
{
	/// <summary>
	/// Base class for all entity classes
	/// </summary>
	public class SvnItem
	{
        
        
        protected SvnItem()
        {
            this.clientContext = new ClientContext();
            this.dispatchMapping = new Hashtable();
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
