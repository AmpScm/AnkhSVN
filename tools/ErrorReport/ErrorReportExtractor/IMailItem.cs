using System;
using System.Collections.Generic;
using System.Text;

namespace ErrorReportExtractor
{
    public interface IMailItem
    {
        string Body { get; set; }
        string ID { get; set; }
        DateTime ReceivedTime { get; set; }
        string SenderName { get; set; }
        string SenderEmail { get; set; }
        string Subject { get; set; }
    }
}
