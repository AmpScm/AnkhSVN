// $Id$
using System;
using NSvn.Core;

namespace NSvn
{
    /// <summary>
    /// An object dispatching notification events from SVN operations.
    /// </summary>
    public class Notifications 
    {
        #region event declarations
        /// <summary>Got an add.</summary>         
        public event NotificationEventHandler AddReceived;

        /// <summary>Got a copy.</summary>          
        public event NotificationEventHandler CopyReceived;

        /// <summary>Got a delete.</summary>         
        public event NotificationEventHandler DeleteReceived;

        /// <summary>Got a restore.</summary>          
        public event NotificationEventHandler RestoreReceived;

        /// <summary>Got a revert.</summary>               
        public event NotificationEventHandler RevertReceived;

        /// <summary>Got a revert failed.</summary>          
        public event NotificationEventHandler FailedRevertReceived;

        /// <summary>Got a resolve.</summary>          
        public event NotificationEventHandler ResolveReceived;

        /// <summary>Got a status.</summary>              
        public event NotificationEventHandler StatusReceived;
        /// <summary>Got a skip.</summary>          
        public event NotificationEventHandler SkipReceived;
        
        /// <summary>Got a delete.</summary>          
        public event NotificationEventHandler UpdateDeleteReceived;

        /// <summary>Got an add in an update.</summary>          
        public event NotificationEventHandler UpdateAddReceived;

        /// <summary>Got any other 
        ///          action in an update.</summary>          
        public event NotificationEventHandler UpdateUpdateReceived;

        /// <summary>Got an update. </summary>          
        public event NotificationEventHandler UpdateCompletedReceived;

        /// <summary>About to update an external module, use for
        ///                                checkouts and switches too,
        ///                                end with @c svn_wc_update_completed.</summary>          
        public event NotificationEventHandler UpdateExternalReceived;

        /// <summary>Got a commit modified.</summary>          
        public event NotificationEventHandler CommitModifiedReceived;

        /// <summary>Got a commit added.</summary>              
        public event NotificationEventHandler CommitAddedReceived;

        /// <summary>Got a commit deleted.</summary>               
        public event NotificationEventHandler CommitDeletedReceived;

        /// <summary>Got a commit replaced.</summary>              
        public event NotificationEventHandler CommitReplacedReceived;

        ///<summary>Delta was sent to the repository.</summary>              
        public event NotificationEventHandler CommitPostfixTxDeltaReceived;

        /// <summary>
        /// Invoked for <em>every</em> notification. Use the Notification
        /// object to figure out which notification was fired. This 
        /// even will be fired <em>after</em> the specific notification events.       
        /// </summary>
        public event NotificationEventHandler NotificationReceived;

        /// <summary>
        /// Invoked when a log message is required.
        /// </summary>
        public event LogMessageEventHandler QueryLogMessage;
        #endregion

        #region Notify(very boring stuff)
        /// <summary>
        /// Callback for notifications. Dispatch into class events.
        /// </summary>
        /// <param name="notification">A Notificaton object containing 
        /// information about the notification</param>
        internal void Notify( SvnResource resource, Notification notification )
        {
            NotificationEventArgs args = new NotificationEventArgs( notification );

            switch( notification.Action )
            {
                case NotifyAction.Add:
                    if ( this.AddReceived != null )
                        this.AddReceived( this, args );
                    break;
                case NotifyAction.CommitAdded:
                    if ( this.CommitAddedReceived != null )
                        this.CommitAddedReceived( this, args );
                    break;
                case NotifyAction.CommitDeleted:
                    if ( this.CommitDeletedReceived != null )
                        this.CommitDeletedReceived( this, args );
                    break;
                case NotifyAction.CommitModified:
                    if ( this.CommitModifiedReceived != null )
                        this.CommitModifiedReceived( this, args );
                    break;
                case NotifyAction.CommitPostfixTxDelta:
                    if ( this.CommitPostfixTxDeltaReceived != null )
                        this.CommitPostfixTxDeltaReceived( this, args );
                    break;
                case NotifyAction.CommitReplaced:
                    if ( this.CommitReplacedReceived != null )
                        this.CommitReplacedReceived( this, args );
                    break;
                case NotifyAction.Copy:
                    if ( this.CopyReceived != null )
                        this.CopyReceived( this, args );
                    break;
                case NotifyAction.Delete:
                    if ( this.DeleteReceived != null )
                        this.DeleteReceived( this, args );
                    break;
                case NotifyAction.FailedRevert:
                    if ( this.FailedRevertReceived!= null )
                        this.FailedRevertReceived( this, args );
                    break;
                case NotifyAction.Resolve:
                    if ( this.ResolveReceived!= null )
                        this.ResolveReceived( this, args );
                    break;
                case NotifyAction.Restore:
                    if ( this.RestoreReceived!= null )
                        this.RestoreReceived( this, args );
                    break;
                case NotifyAction.Revert:
                    if ( this.RevertReceived!= null )
                        this.RevertReceived( this, args );
                    break;
                case NotifyAction.Skip:
                    if ( this.SkipReceived!= null )
                        this.SkipReceived( this, args );
                    break;
                case NotifyAction.Status:
                    if ( this.StatusReceived!= null )
                        this.StatusReceived( this, args );
                    break;
                case NotifyAction.UpdateAdd:
                    if ( this.UpdateAddReceived!= null )
                        this.UpdateAddReceived( this, args );
                    break;
                case NotifyAction.UpdateCompleted:
                    if ( this.UpdateCompletedReceived != null )
                        this.UpdateCompletedReceived( this, args );
                    break;
                case NotifyAction.UpdateDelete:
                    if ( this.UpdateDeleteReceived!= null )
                        this.UpdateDeleteReceived( this, args );
                    break;
                case NotifyAction.UpdateExternal:
                    if ( this.UpdateExternalReceived!= null )
                        this.UpdateExternalReceived( this, args );
                    break;
                case NotifyAction.UpdateUpdate:
                    if ( this.UpdateUpdateReceived != null )
                        this.UpdateUpdateReceived( this, args );
                    break;
                default:
                    throw new ArgumentException( "Unknown notification type. This " +
                        "should not happen", "notification" );
            }

            if ( this.NotificationReceived != null )
                this.NotificationReceived( this, args );
        }
        #endregion

        /// <summary>
        /// Callback method for log messages.
        /// </summary>
        /// <param name="resource">The resource this log message request is originating from
        /// </param>
        /// <param name="items">The targets to be committed.</param>
        /// <returns>A string containing the log message, or null if the commit is canceled
        /// </returns>
        internal string LogMessageCallback( SvnResource resource, CommitItem[] items )
        {
            if ( this.QueryLogMessage != null )
            {
                LogMessageEventArgs args = new LogMessageEventArgs( items );
                this.QueryLogMessage( resource, args );
                if ( !args.Canceled )
                    return args.LogMessage;
                else
                    return null;
            }
            else
                return "";
        }

    }
}
