// $Id$
using System;
using NSvn.Core;
using NSvn.Common;

namespace NSvn
{
    /// <summary>
    /// Represents a general context object for NSvn operations. To deal with the various
    /// callbacks from SVN, you need to derive from this class and override the callback
    /// methods.
    /// </summary>
    public class NSvnContext
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public NSvnContext()
        {
            this.clientContext = new ClientContext( 
                new NotifyCallback( this.NotifyCallback ),
                new AuthenticationBaton() );
            this.clientContext.LogMessageCallback = new LogMessageCallback(
                this.LogMessageCallback );
        }

        /// <summary>
        /// Add an authentication provider,
        /// </summary>
        /// <param name="provider">The provider to add.</param>
        public void AddAuthenticationProvider( IAuthenticationProvider provider )
        {
            this.clientContext.AuthBaton.Providers.Add( provider );
        }

        /// <summary>
        /// Remove an authentication provider.
        /// </summary>
        /// <param name="provider">The provider to remove.</param>
        public void RemoveAuthenticationProvider( IAuthenticationProvider provider )
        {
            this.clientContext.AuthBaton.Providers.Remove( provider );
        }

        

        /// <summary>
        /// Notification callback. Override in derived classes to process the notification.
        /// </summary>
        /// <param name="notification">A Notification object containing information 
        /// about the notification.</param>
        protected virtual void NotifyCallback( Notification notification )
        {
            // empty
        }

        /// <summary>
        /// Log message callback. Override in derived classes to provide a log message.
        /// </summary>
        /// <param name="commitItems">These objects contain information about 
        /// the items that are to be committed.</param>
        /// <returns>The log message, or null if the </returns>
        protected virtual string LogMessageCallback( CommitItem[] commitItems )
        {
            return "";
        }

        /// <summary>
        /// The ClientContext object used by this NSvnContext.
        /// </summary>
        protected internal ClientContext ClientContext
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.clientContext; }
        }

        private ClientContext clientContext;
    }
}
