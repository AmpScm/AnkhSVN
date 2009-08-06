//using System;
//using System.Collections.Generic;
//using System.Text;
//using Outlook;
//using MAPI;
//using ErrorReportExtractor.Properties;

//namespace ErrorReportExtractor
//{
//    public class OutlookContainer : IOutlookContainer
//    {
//        public OutlookContainer()
//        {
//            this.callback = new NullProgressCallback();
//            this.callback.Verbose( "Creating Outlook application object" );

//            this.outlook = new Outlook.Application();

//            this.callback.Verbose( "Getting mapi session: {0}", this.MapiSession );
//        }

//        public IEnumerable<IErrorReport> GetItems()
//        {
//            int dummy;
//            return this.GetItems( Settings.Default.DefaultOutlookFolder, null, out dummy );
//        }

//        public IEnumerable<IMailItem> GetPotentialReplies()
//        {
//            int dummy;
//            return this.GetPotentialReplies( Settings.Default.DefaultOutlookFolder, null, out dummy );
//        }


//        public IEnumerable<IErrorReport> GetItems( string folderPath, int? startID, out int lastIndexRetrieved )
//        {
//            MAPIFolder folder = this.GetFolder( folderPath );

//            this.callback.Verbose( "Outlook folder has {0} items", folder.Items.Count );
//            int start = startID.HasValue ? startID.Value : 1;

//            lastIndexRetrieved = folder.Items.Count - 1;

//            return this.DoGetItems( folder, start );
//        }

//        private IEnumerable<IErrorReport> DoGetItems( MAPIFolder folder, int start )
//        {
//            for ( int i = start; i < folder.Items.Count; i++ )
//            {

//                ErrorReport report = new ErrorReport();
//                Outlook.MailItem outlookItem = null;
//                try
//                {
//                    outlookItem = folder.Items.Item( i ) as Outlook.MailItem;
//                }
//                catch ( System.Exception ex )
//                {
//                    this.callback.Exception( ex );
//                    continue;
//                }

//                if ( outlookItem == null )
//                {
//                    this.callback.Verbose( "Item {0} not a mail item.", i );
//                    continue;
//                }

//                this.callback.Verbose( "Returning item #{0}", i );
//                InitializeMailItemFromOutlookMailItem( folder, outlookItem, report );

//                // It's not an error report if there's a reply to ID
//                if ( report.ReplyToID != null )
//                    continue;

//                this.callback.Progress();
//                report.Body = outlookItem.Body;
//                report.ReceivedTime = outlookItem.ReceivedTime;
//                yield return report;
//                //break;             
//            }
//        }

//        public IEnumerable<IMailItem> GetPotentialReplies( string folderPath, int? startIndex, out int lastIndexRetrieved )
//        {
//            MAPIFolder folder = this.GetFolder( folderPath );
//            int start = startIndex.HasValue ? startIndex.Value : 1;
//            lastIndexRetrieved = folder.Items.Count - 1;

//            return this.DoGetPotentialReplies( folder, start );
//        }

//        private IEnumerable<IMailItem> DoGetPotentialReplies( MAPIFolder folder, int start )
//        {
//            for ( int i = start; i <= folder.Items.Count; i++ )
//            {
//                Outlook.MailItem outlookItem = null;

//                try
//                {
//                    outlookItem = folder.Items.Item( i ) as Outlook.MailItem;
//                }
//                catch ( System.Exception ex )
//                {
//                    this.callback.Exception( ex );
//                    continue;
//                }
//                if ( outlookItem == null )
//                {
//                    this.callback.Warning( "Item {0} in folder {1} not a mail item.", i, folder.Name );
//                    continue;
//                }
//                MailItem mailItem = new MailItem();
//                InitializeMailItemFromOutlookMailItem( folder, outlookItem, mailItem );

//                // they have to be replies to be replies
//                if ( mailItem.ReplyToID == null )
//                {
//                    continue;
//                }

//                this.callback.Verbose( "Returning item with index {0}, date {1} from Outlook.", i, mailItem.ReceivedTime );

//                yield return mailItem;
//            }
//        }

//        public void SetProgressCallback( IProgressCallback callback )
//        {
//            this.callback = callback;
//        }

//        private void InitializeMailItemFromOutlookMailItem( MAPIFolder folder, Outlook.MailItem outlookItem, MailItem mailItem )
//        {
//            Message mapiMessage = this.MapiSession.GetMessage( outlookItem.EntryID, folder.StoreID ) as Message;

//            MapiFields fields = new MapiFields( mapiMessage.Fields as Fields );
//            mailItem.InternetMailID = fields.AsString( CdoPropTags.CdoPR_INTERNET_MESSAGE_ID );
//            mailItem.SenderEmail = fields.AsString( CdoPropTags.CdoPR_SENDER_EMAIL_ADDRESS );
//            mailItem.SenderName = fields.AsString( CdoPropTags.CdoPR_SENDER_NAME );
//            if ( outlookItem.Recipients.Count > 0 )
//            {
//                mailItem.ReceiverEmail = outlookItem.Recipients.Item( 1 ).Address;
//                mailItem.ReceiverName = outlookItem.Recipients.Item( 1 ).Name;
//            }
//            mailItem.ReplyToID = fields.AsString( MapiFields.InReplyTo );
//            mailItem.Subject = outlookItem.Subject;
//            mailItem.Body = outlookItem.Body;
//            mailItem.ReceivedTime = outlookItem.ReceivedTime;
//        }

//        private MAPIFolder GetFolder( string folderPath )
//        {
//            NameSpace ns = outlook.GetNamespace( "MAPI" );

//            this.callback.Verbose( "Finding MAPI folder {0}", folderPath );

//            string[] pathComponents = folderPath.Split( '\\' );
//            MAPIFolder folder = ns.Folders.Item( 1 );
//            foreach ( string component in pathComponents )
//            {
//                folder = folder.Folders.Item( component );
//            }
//            this.callback.Verbose( "MAPI folder {0} found", folderPath );

//            return folder;
//        }

//        private MAPI.Session MapiSession
//        {
//            get
//            {
//                if ( this.mapiSession == null )
//                {
//                    this.mapiSession = new SessionClass();
//                    this.mapiSession.Logon( Type.Missing, Type.Missing, false, false, null, Type.Missing, Type.Missing );
//                }
//                return mapiSession;
//            }
//        }


//        private Application outlook;
//        private IProgressCallback callback;
//        private MAPI.Session mapiSession;


//    }
//}
