using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using ErrorReportExtractor.Properties;

namespace ErrorReportExtractor
{
    class Mailer : ErrorReportExtractor.IMailer
    {
        public Mailer(IProgressCallback cb)
        {
            this.progressCallback = cb;
        }

        public void SendReply( IErrorReport report, string replyText )
        {
            MailMessage message = new MailMessage();
            message.Subject = Settings.Default.Subject;


            message.From = new MailAddress(Settings.Default.SenderEmail, Settings.Default.SenderName);            
            message.To.Add(new MailAddress(report.SenderEmail, report.SenderName));
            message.CC.Add( new MailAddress( Settings.Default.CC ) );

            message.Body = replyText;

            message.Headers[ "In-reply-to" ] = report.ID;
            message.ReplyTo = new MailAddress(Settings.Default.ReplyToEmail);

            SmtpClient client = new SmtpClient(Settings.Default.MailServer);
            this.progressCallback.Info( "Sending reply to report with message ID {0} to {1}", report.ID, report.SenderName );
            client.Send( message );
        }

        private IProgressCallback progressCallback;
    }
}
