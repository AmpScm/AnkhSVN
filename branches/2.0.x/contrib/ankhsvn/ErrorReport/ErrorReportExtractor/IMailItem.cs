using System;
using System.Collections.Generic;
using System.Text;

namespace ErrorReportExtractor
{
    public interface IMailItem
    {
        int ErrorReportID { get; }
        string Body { get; set; }
        int MailItemID { get; set; }
        string InternetMailID { get; set; }
        DateTime ReceivedTime { get; set; }
        string SenderName { get; set; }
        string SenderEmail { get; set; }
        string ReceiverName { get; set; }
        string ReceiverEmail { get; set; }
        string Subject { get; set; }
        string ReplyToID { get; set; }
        bool Read { get; set; }
        bool RepliedTo { get; set; }

        IList<IMailItem> Replies { get; }
    }
}
