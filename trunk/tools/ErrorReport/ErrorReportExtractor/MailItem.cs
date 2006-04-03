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

        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        public string Body
        {
            get { return body; }
            set { body = value; }
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

        private string id;
        private DateTime receivedTime;
        private string senderName;
        private string senderEmail;
        private string body;
        private string subject;
    }
}
