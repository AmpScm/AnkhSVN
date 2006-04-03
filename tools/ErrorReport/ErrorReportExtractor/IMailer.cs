using System;
namespace ErrorReportExtractor
{
    public interface IMailer
    {
        void SendReply( IErrorReport report, string replyText );
    }
}
