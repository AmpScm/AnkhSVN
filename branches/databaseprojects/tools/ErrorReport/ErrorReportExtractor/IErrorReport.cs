using System;
namespace ErrorReportExtractor
{
    public interface IErrorReport
    {
        string Body { get; set; }
        string ID { get; set; }
        DateTime ReceivedTime { get; set; }
        string SenderName { get; set; }
        string Subject { get; set; }
        int? MajorVersion { get; }
        int? MinorVersion { get; }
        int? PatchVersion { get; }
        int? Revision { get; }
        string ExceptionType { get; }
        string DteVersion { get; }
        IStackTrace StackTrace { get; }
        bool RepliedTo { get; set; }
    }
}
