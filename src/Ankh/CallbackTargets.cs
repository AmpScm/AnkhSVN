using System;
using NSvn;
using Ankh.UI;
using System.Windows.Forms;

namespace Ankh
{
	/// <summary>
	/// Deals with the callbacks from SVN.
	/// </summary>
	internal class CallbackTargets
	{
        internal CallbackTargets()
        {
            this.notifications = new Notifications();
            this.notifications.QueryLogMessage += new LogMessageEventHandler( this.LogMessage );
        }
        /// <summary>
        /// The Notifications instance associated with this object.
        /// </summary>
        internal Notifications Notifications
        {
            get{ return this.notifications; }
        }

        private void LogMessage( object sender, LogMessageEventArgs args )
        {
            CommitDialog dialog = new CommitDialog();
            if ( dialog.ShowDialog() == DialogResult.OK )
                args.LogMessage = dialog.LogMessage;
            else
                args.Canceled = true;
        }

        private Notifications notifications;
	}
}
