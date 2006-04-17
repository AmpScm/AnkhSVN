using System;
using System.Collections.Generic;
using System.Text;

namespace ErrorReportExtractor
{
    public class MailItem : IMailItem
    {
        public MailItem(string id, string subject, string body, string senderEmail, string senderName, DateTime receivedTime)
        {
            this.id = id;
            this.subject = subject;
            this.body = body;
            this.senderEmail = senderEmail;
            this.senderName = senderName;
            this.receivedTime = receivedTime;
        }

        public MailItem()
        {

        }

        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        public string Body
        {
            get { return body; }
            set 
            { 
                body = value;
                OnBodyChanged();
            }
        }

        protected virtual void OnBodyChanged()
        {
            
        }

        public string SenderName
        {
            get { return senderName != null ? senderName : senderEmail; }
            set { senderName = value; }
        }

        public string SenderEmail
        {
            get { return senderEmail; }
            set { senderEmail = value; }
        }

        public DateTime ReceivedTime
        {
            get { return receivedTime; }
            set { receivedTime = value; }
        }

        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        public string ReplyToID
        {
            get { return replyToID; }
            set { replyToID = value; }
        }

        public string ReceiverEmail
        {
            get { return receiverEmail; }
            set { receiverEmail = value; }
        }
        public string ReceiverName
        {
            get { return receiverName; }
            set { receiverName = value; }
        }

        public IList<IMailItem> Replies
        {
            get { return replies; }
        }

        private string replyToID;
        private string receiverEmail;
        private string receiverName;
        private string id;
        private DateTime receivedTime;
        private string senderName;
        private string senderEmail;
        private string body;
        private string subject;
        private List<IMailItem> replies = new List<IMailItem>();

       
    }
}
