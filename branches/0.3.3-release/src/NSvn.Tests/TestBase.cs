// $Id$
using System;
using NUnit.Framework;
using NSvn.Core;
using NSvn.Common;

namespace NSvn.Tests
{
    /// <summary>
    /// Summary description for TestBase.
    /// </summary>
    public class TestBase : NSvn.Core.Tests.TestBase
    {
        #region SetupEventHandlers
        protected void SetupEventHandlers( Notifications n )
        {
            n.AddReceived += new NotificationEventHandler( this.AddReceived );
            n.CopyReceived += new NotificationEventHandler( this.CopyReceived );
            n.DeleteReceived += new NotificationEventHandler( this.DeleteReceived );
            n.RestoreReceived += new NotificationEventHandler( this.RestoreReceived );
            n.ResolveReceived += new NotificationEventHandler( this.ResolveReceived );
            n.RevertReceived += new NotificationEventHandler( this.RevertReceived );
            n.SkipReceived += new NotificationEventHandler( this.SkipReceived );
            n.CommitAddedReceived += new NotificationEventHandler( this.CommitAddedReceived );
            n.CommitDeletedReceived += new NotificationEventHandler( this.CommitDeletedReceived );
            n.CommitModifiedReceived += new NotificationEventHandler( this.CommitModifiedReceived );
            n.CommitPostfixTxDeltaReceived += new NotificationEventHandler( this.CommitPostfixTxDeltaReceived );
            n.CommitReplacedReceived += new NotificationEventHandler( this.CommitReplacedReceived );
            n.FailedRevertReceived += new NotificationEventHandler( this.FailedRevertReceived );
            n.UpdateAddReceived += new NotificationEventHandler( this.UpdateAddReceived );
            n.UpdateCompletedReceived += new NotificationEventHandler( this.UpdateCompletedReceived );
            n.UpdateDeleteReceived += new NotificationEventHandler( this.UpdateDeleteReceived );
            n.UpdateExternalReceived += new NotificationEventHandler( this.UpdateExternalReceived );
            n.UpdateUpdateReceived += new NotificationEventHandler( this.UpdateUpdateReceived );

            n.NotificationReceived += new NotificationEventHandler( this.NotificationReceived );
        }
        #endregion

        /// <summary>
        /// The number of notifications made in this test
        /// </summary>
        protected int NotificationCount
        {
            get{ return this.noNotifications; }
        }

        /// <summary>
        /// Resets the count of notifications
        /// </summary>
        protected void ResetNotificationCount()
        {
            this.noNotifications = 0;
        }

        #region notification event handlers(boring)
        private void AddReceived( object sender, NotificationEventArgs args )
        {
            Assertion.AssertEquals( "Wrong type of notification", 
                NotifyAction.Add, args.Notification.Action );
        }

        private void CopyReceived( object sender, NotificationEventArgs args )
        {
            Assertion.AssertEquals( "Wrong type of notification", 
                NotifyAction.Copy, args.Notification.Action );
        }

        private void DeleteReceived( object sender, NotificationEventArgs args )
        {
            Assertion.AssertEquals( "Wrong type of notification", 
                NotifyAction.Delete, args.Notification.Action );
        }

        private void RestoreReceived( object sender, NotificationEventArgs args )
        {
            Assertion.AssertEquals( "Wrong type of notification", 
                NotifyAction.Restore, args.Notification.Action );
        }

        private void ResolveReceived( object sender, NotificationEventArgs args )
        {
            Assertion.AssertEquals( "Wrong type of notification", 
                NotifyAction.Resolved, args.Notification.Action );
        }

        private void RevertReceived( object sender, NotificationEventArgs args )
        {
            Assertion.AssertEquals( "Wrong type of notification", 
                NotifyAction.Revert, args.Notification.Action );
        }

        private void FailedRevertReceived( object sender, NotificationEventArgs args )
        {
            Assertion.AssertEquals( "Wrong type of notification", 
                NotifyAction.FailedRevert, args.Notification.Action );
        }        

        private void SkipReceived( object sender, NotificationEventArgs args )
        {
            Assertion.AssertEquals( "Wrong type of notification", 
                NotifyAction.Skip, args.Notification.Action );
        }

        private void UpdateDeleteReceived( object sender, NotificationEventArgs args )
        {
            Assertion.AssertEquals( "Wrong type of notification", 
                NotifyAction.UpdateDelete, args.Notification.Action );
        }

        private void UpdateAddReceived( object sender, NotificationEventArgs args )
        {
            Assertion.AssertEquals( "Wrong type of notification", 
                NotifyAction.UpdateAdd, args.Notification.Action );
        }

        private void UpdateUpdateReceived( object sender, NotificationEventArgs args )
        {
            Assertion.AssertEquals( "Wrong type of notification", 
                NotifyAction.UpdateUpdate, args.Notification.Action );
        }

        private void UpdateCompletedReceived( object sender, NotificationEventArgs args )
        {
            Assertion.AssertEquals( "Wrong type of notification", 
                NotifyAction.UpdateCompleted, args.Notification.Action );
        }

        private void UpdateExternalReceived( object sender, NotificationEventArgs args )
        {
            Assertion.AssertEquals( "Wrong type of notification", 
                NotifyAction.UpdateExternal, args.Notification.Action );
        }

        private void CommitModifiedReceived( object sender, NotificationEventArgs args )
        {
            Assertion.AssertEquals( "Wrong type of notification", 
                NotifyAction.Add, args.Notification.Action );
        }

        private void CommitAddedReceived( object sender, NotificationEventArgs args )
        {
            Assertion.AssertEquals( "Wrong type of notification", 
                NotifyAction.Add, args.Notification.Action );
        }

        private void CommitDeletedReceived( object sender, NotificationEventArgs args )
        {
            Assertion.AssertEquals( "Wrong type of notification", 
                NotifyAction.Add, args.Notification.Action );
        }

        private void CommitReplacedReceived( object sender, NotificationEventArgs args )
        {
            Assertion.AssertEquals( "Wrong type of notification", 
                NotifyAction.CommitReplaced, args.Notification.Action );
        }

        private void CommitPostfixTxDeltaReceived( object sender, NotificationEventArgs args )
        {
            Assertion.AssertEquals( "Wrong type of notification", 
                NotifyAction.CommitPostfixTxDelta, args.Notification.Action );
        }

        private void NotificationReceived( object sender, NotificationEventArgs args )
        {
            this.noNotifications++;
        }
        #endregion

        private int noNotifications = 0;



    }
}
