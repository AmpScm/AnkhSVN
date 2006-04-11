using System;
namespace ErrorReportExtractor
{
    public interface IMailer : IService
    {
        void SendReply( IErrorReport report, string replyText );
    }
}
