using System;
namespace ErrorReportExtractor
{
    public interface IMailer : IService
    {
        void SendReply( IMailItem report, string replyText );
    }
}
