using System;
using NSvn.Core;

namespace NSvn
{
	/// <summary>
	/// Contains the arguments passed to a notification event.
	/// </summary>
	public class NotificationEventArgs : EventArgs
	{
		public NotificationEventArgs( Notification notification )
		{
			this.notification = notification;
		}

        /// <summary>
        /// The Notification object associated with this event.
        /// </summary>
        public Notification Notification
        {
            get{ return this.notification; }
        }


        private Notification notification;
	}

    public delegate void NotificationEventHandler( object sender, NotificationEventArgs e );

}
