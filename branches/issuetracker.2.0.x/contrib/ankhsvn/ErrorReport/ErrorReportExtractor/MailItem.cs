using System;
using System.Collections.Generic;
using System.Text;

namespace ErrorReportExtractor
{
    public class MailItem : IMailItem
    {
        public MailItem(int errorReportID, int mailItemID, string subject, string body, string senderEmail, string senderName, DateTime receivedTime)
        {
            this.errorReportID = errorReportID;
            this.mailItemID = mailItemID;
            this.subject = subject;
            this.body = body;
            this.senderEmail = senderEmail;
            this.senderName = senderName;
            this.receivedTime = receivedTime;
        }

        public MailItem()
        {

        }

        public int ErrorReportID
        {
            get { return this.errorReportID; }
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

        public int MailItemID
        {
            get { return mailItemID; }
            set { mailItemID = value; }
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


        public string InternetMailID
        {
            get { return internetMailID; }
            set { internetMailID = value; }
        }

        public bool Read
        {
            get { return read; }
            set { read = value; }
        }

        public bool RepliedTo
        {
            get { return repliedTo; }
            set { repliedTo = value; }
        }





        private string internetMailID;

        private bool read;

        private bool repliedTo;
        private int errorReportID;
        private string replyToID;
        private string receiverEmail;
        private string receiverName;
        private int mailItemID;
        private DateTime receivedTime;
        private string senderName;
        private string senderEmail;
        private string body;
        private string subject;
        private List<IMailItem> replies = new List<IMailItem>();

       
    }
}
