using System;
using System.Collections.Generic;
using System.Text;
using Outlook;
using MAPI;

namespace ErrorReportExtractor
{
    public class OutlookContainer : IReportContainer
    {
        public OutlookContainer()
        {
            this.callback = new NullProgressCallback();
            this.callback.Verbose("Creating Outlook application object");

            this.outlook = new Outlook.Application();

            this.callback.Verbose( "Getting mapi session: {0}", this.MapiSession );
        }

        

        public IEnumerable<IErrorReport> GetItems(string folderPath, DateTime? itemsAfter, int? startID)
        {
            MAPIFolder folder = this.GetFolder( folderPath );
            if ( itemsAfter != null)
            {
                this.callback.Verbose("Retrieving items after {0} from Outlook folder {1}", itemsAfter, folder.FullFolderPath);
            }
            else
            {
                this.callback.Verbose("Retrieving all items from Outlook folder {0}", folder.FullFolderPath);
            }

            this.callback.Verbose( "Outlook folder has {0} items", folder.Items.Count );
            int start = startID.HasValue ? startID.Value : 1;
            for (int i = start; i < folder.Items.Count; i++)
            {
               
                ErrorReport report = new ErrorReport();
                Outlook.MailItem outlookItem = null;
                try
                {
                    outlookItem = folder.Items.Item( i ) as Outlook.MailItem;
                }
                catch ( System.Exception ex )
                {
                    this.callback.Exception( ex );
                    continue;
                }

                if ( outlookItem == null )
                {
                    this.callback.Verbose( "Item {0} not a mail item.", i );
                    continue;
                }
                if ( itemsAfter != null && outlookItem.ReceivedTime < itemsAfter )
                {
                    if ( i % 50 == 0 )
                    {
                        this.callback.Verbose( "Skipping item {0} since {1} is before {2}", i, outlookItem.ReceivedTime, itemsAfter );
                    }
                    continue;
                }
                this.callback.Verbose( "Returning item #{0}", i );
                InitializeMailItemFromOutlookMailItem( folder, outlookItem, report );


                // It's not an error report if there's a reply to ID
                if ( report.ReplyToID != null )
                    continue;
               

                this.callback.Progress();
                report.Body = outlookItem.Body;
                report.ReceivedTime = outlookItem.ReceivedTime;
                yield return report;
                //break;             
            }
        }

        public IEnumerable<IMailItem> GetPotentialReplies(string folderPath, int? startIndex)
        {
            MAPIFolder folder = this.GetFolder( folderPath );
            int start = startIndex.HasValue ? startIndex.Value : 1;
            for ( int i = start; i <= folder.Items.Count; i++ )
            {
                Outlook.MailItem outlookItem = null;

                try
                {
                    outlookItem = folder.Items.Item( i ) as Outlook.MailItem;
                }
                catch ( System.Exception ex )
                {
                    this.callback.Exception( ex );
                    continue;
                }
                if ( outlookItem == null )
                {
                    this.callback.Warning( "Item {0} in folder {1} not a mail item.", i, folderPath );
                    continue;
                }
                MailItem mailItem = new MailItem();
                InitializeMailItemFromOutlookMailItem( folder, outlookItem, mailItem );

                // they have to be replies to be replies
                if ( mailItem.ReplyToID == null )
                {
                    continue;
                }

                this.callback.Verbose( "Returning item with index {0}, date {1} from Outlook.", i, mailItem.ReceivedTime );

                yield return mailItem;
            }
        }

        public void SetProgressCallback( IProgressCallback callback )
        {
            this.callback = callback;
        }

        private void InitializeMailItemFromOutlookMailItem( MAPIFolder folder, Outlook.MailItem outlookItem, MailItem mailItem )
        {
            Message mapiMessage = this.MapiSession.GetMessage( outlookItem.EntryID, folder.StoreID ) as Message;

            MapiFields fields = new MapiFields( mapiMessage.Fields as Fields );
            mailItem.InternetMailID = fields.AsString( CdoPropTags.CdoPR_INTERNET_MESSAGE_ID );
            mailItem.SenderEmail = fields.AsString( CdoPropTags.CdoPR_SENDER_EMAIL_ADDRESS );
            mailItem.SenderName = fields.AsString( CdoPropTags.CdoPR_SENDER_NAME );
            if ( outlookItem.Recipients.Count > 0 )
            {
                mailItem.ReceiverEmail = outlookItem.Recipients.Item( 1 ).Address;
                mailItem.ReceiverName = outlookItem.Recipients.Item( 1 ).Name;
            }
            mailItem.ReplyToID = fields.AsString( MapiFields.InReplyTo );
            mailItem.Subject = outlookItem.Subject;
            mailItem.Body = outlookItem.Body;
            mailItem.ReceivedTime = outlookItem.ReceivedTime;
        }

        private MAPIFolder GetFolder( string folderPath )
        {
            NameSpace ns = outlook.GetNamespace( "MAPI" );

            this.callback.Verbose( "Finding MAPI folder {0}", folderPath );

            string[] pathComponents = folderPath.Split( '\\' );
            MAPIFolder folder = ns.Folders.Item( 1 );
            foreach ( string component in pathComponents )
            {
                folder = folder.Folders.Item( component );
            }
            this.callback.Verbose( "MAPI folder {0} found", folderPath );

            return folder;
        }

        private MAPI.Session MapiSession
        {
            get 
            {
                if ( this.mapiSession == null )
                {
                    this.mapiSession = new SessionClass();
                    this.mapiSession.Logon( Type.Missing, Type.Missing, false, false, null, Type.Missing, Type.Missing );
                }
                return mapiSession; 
            }
        }


        private Application outlook;
        private IProgressCallback callback;
        private MAPI.Session mapiSession;

        
    }
}
