using System;
using System.Collections.Generic;
using System.Text;
using Outlook;
using MAPI;

namespace ErrorReportExtractor
{
    public class MailContainer
    {
        public MailContainer(string folderPath, IProgressCallback callback)
        {
            this.callback = callback;
            this.callback.Verbose("Creating Outlook application object");

            this.outlook = new Outlook.Application();

            this.mapiSession = new SessionClass();
            this.mapiSession.Logon(Type.Missing, Type.Missing, false, false, null, Type.Missing, Type.Missing);

            NameSpace ns = outlook.GetNamespace("MAPI");

            this.callback.Verbose("Finding MAPI folder {0}", folderPath);

            string[] pathComponents = folderPath.Split('\\');
            this.folder = ns.Folders.Item(1);
            foreach (string component in pathComponents)
            {
                this.folder = this.folder.Folders.Item(component);
            }
            this.callback.Verbose("MAPI folder {0} found", folderPath);
        }

        public IEnumerable<IErrorReport> GetAllItems(int? limit)
        {
            if ( limit != null)
            {
                this.callback.Verbose("Retrieving {0} items from Outlook folder {1}", limit, folder.FullFolderPath);
            }
            else
            {
                this.callback.Verbose("Retrieving all items from Outlook folder {0}", folder.FullFolderPath);
            }

            for (int i = 1; i < this.folder.Items.Count; i++)
            {
                if (limit != null && i >= limit)
                {
                    this.callback.Verbose("Reached limit of {0} items", limit);
                    break;
                }
                MailItem mailItem = this.folder.Items.Item(i) as MailItem;
                Message mapiMessage = this.mapiSession.GetMessage(mailItem.EntryID, this.folder.StoreID) as Message;

                MapiFields fields = new MapiFields (mapiMessage.Fields as Fields);
                string messageID = fields.AsString(CdoPropTags.CdoPR_INTERNET_MESSAGE_ID);

                // It's not an error report if there's a reply to ID
                string replyToID = fields.AsString(MapiFields.InReplyTo);
                if (replyToID != null)
                    continue;

                this.callback.Progress();
                yield return new ErrorReport(messageID, mailItem.Subject, mailItem.Body, mailItem.SenderName,
                    mailItem.ReceivedTime);
            }
        }

        private Application outlook;
        private MAPIFolder folder;
        private IProgressCallback callback;
        private MAPI.Session mapiSession;
    }
}
