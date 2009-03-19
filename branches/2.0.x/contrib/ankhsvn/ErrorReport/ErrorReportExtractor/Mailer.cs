using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using ErrorReportExtractor.Properties;

namespace ErrorReportExtractor
{
    public class Mailer : ErrorReportExtractor.IMailer, IService
    {
        public Mailer()
        {
            this.progressCallback = new NullProgressCallback();
        }

        public void SendReply( IMailItem report, string replyText )
        {
            MailMessage message = new MailMessage();
            message.Subject = Settings.Default.Subject;


            message.From = new MailAddress(Settings.Default.SenderEmail, Settings.Default.SenderName);            
            message.To.Add(new MailAddress(report.SenderEmail, report.SenderName));
            message.CC.Add( new MailAddress( Settings.Default.CC ) );

            message.Body = replyText;

            message.Headers[ "In-reply-to" ] = report.InternetMailID;
            message.ReplyTo = new MailAddress(Settings.Default.ReplyToEmail);

            SmtpClient client = new SmtpClient(Settings.Default.MailServer);
            this.progressCallback.Info( "Sending reply to report with message ID {0} to {1}", report.MailItemID, report.SenderName );
            client.Send( message );
        }

        public void SetProgressCallback( IProgressCallback callback )
        {
            this.progressCallback = callback;
        }

        private IProgressCallback progressCallback;

    }
}
