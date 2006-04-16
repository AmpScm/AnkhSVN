using System;
using System.Collections.Generic;
using System.Text;
using Outlook;
using MAPI;

namespace ErrorReportExtractor
{
    public class MailContainer : IReportContainer
    {
        public MailContainer()
        {
            this.callback = new NullProgressCallback();
            this.callback.Verbose("Creating Outlook application object");

            this.outlook = new Outlook.Application();

            this.callback.Verbose( "Getting mapi session: {0}", this.MapiSession );
        }

        

        public IEnumerable<IErrorReport> GetAllItems(string folderPath, int? limit)
        {
            MAPIFolder folder = this.GetFolder( folderPath );
            if ( limit != null)
            {
                this.callback.Verbose("Retrieving {0} items from Outlook folder {1}", limit, folder.FullFolderPath);
            }
            else
            {
                this.callback.Verbose("Retrieving all items from Outlook folder {0}", folder.FullFolderPath);
            }

            for (int i = 1; i < folder.Items.Count; i++)
            {
                if (limit != null && i >= limit)
                {
                    this.callback.Verbose("Reached limit of {0} items", limit);
                    break;
                }
                ErrorReport report = new ErrorReport();
                Outlook.MailItem outlookItem = folder.Items.Item(i) as Outlook.MailItem;
                InitializeMailItemFromOutlookMailItem(folder, outlookItem, report);
                

                // It's not an error report if there's a reply to ID
                if (report.ReplyToID != null)
                    continue;

                this.callback.Progress();
                report.Body = outlookItem.Body;
                report.ReceivedTime = outlookItem.ReceivedTime;
                yield return report;
            }
        }

        public IEnumerable<IMailItem> GetPotentialReplies(string folderPath)
        {
            MAPIFolder folder = this.GetFolder( folderPath );
            for ( int i = 1; i <= folder.Items.Count; i++ )
            {
                Outlook.MailItem outlookItem = folder.Items.Item( i ) as Outlook.MailItem;
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
            mailItem.ID = fields.AsString( CdoPropTags.CdoPR_INTERNET_MESSAGE_ID );
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
