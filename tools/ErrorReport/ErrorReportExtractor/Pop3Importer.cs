using System;
using System.Collections.Generic;
using System.Text;
using Lesnikowski.Client;
using Lesnikowski.Mail;
using Fines.Utils.Collections;

namespace ErrorReportExtractor
{
    public class Pop3Importer : IPop3Importer
    {
        public Pop3Importer(string host, int port, string user, string password)
        {
            this.host = host;
            this.port = port;
            this.user = user;
            this.password = password;
        }
        #region IPop3Importer Members

        public void DeleteFromServer( int startIndex, int length )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        #endregion

        #region IReportContainer Members

        public IEnumerable<IErrorReport> GetItems()
        {
            Pop3 pop3 = LoginAndStat();
            for ( int i = 1; i <= pop3.MessageCount; i++ )
            {
                string headerString = pop3.GetMessageHeader( i );
                string inReplyTo = this.GetHeader( headerString, "In-reply-to" );
                if ( inReplyTo != null )
                {
                    // not a report
                    continue;
                }

                ErrorReport report = new ErrorReport();
                InitializeMailItemFromPop3Message(report, pop3, i, headerString);

                callback.Verbose( "Retrieved POP3 mail from {0} with subject '{1}'", report.ReceivedTime, report.Subject );                

                //this.callback.Info( "{0}: '{1}'", header.Date, messageID != null ? messageID : "<NULL>");
                //yield break;
                yield return report;

            }
            yield break;
        }

        public IEnumerable<IMailItem> GetPotentialReplies()
        {
            Pop3 pop3 = LoginAndStat();
            for ( int i = 1; i <= pop3.MessageCount; i++ )
            {
                string headerString = pop3.GetMessageHeader( i );
                string inReplyTo = this.GetHeader( headerString, "In-reply-to" );
                if ( inReplyTo == null )
                {
                    // not a reply
                    continue;
                }

                MailItem mailItem = new MailItem();
                InitializeMailItemFromPop3Message( mailItem, pop3, i, headerString );

                callback.Verbose( "Retrieved POP3 mail from {0} with subject '{1}' as potential reply.", mailItem.ReceivedTime, mailItem.Subject );

                yield return mailItem;
            }
        }

        private void InitializeMailItemFromPop3Message( IMailItem mailItem, Pop3 pop3, int i, string headerString )
        {
            SimpleMailMessage msg = SimpleMailMessage.Parse( pop3.GetMessage( i ) );
            mailItem.Body = msg.TextDataString;
            mailItem.InternetMailID = this.GetHeader( headerString, "Message-id" );
            mailItem.Read = false;
            mailItem.ReceivedTime = msg.Date;

            if ( msg.To.Count > 0 )
            {
                mailItem.ReceiverEmail = msg.To[ 0 ].Address;
                mailItem.ReceiverName = msg.To[ 0 ].Name;
            }

            mailItem.ReplyToID = this.GetHeader( headerString, "In-reply-to");
            if ( msg.From.Count > 0 )
            {
                mailItem.SenderEmail = msg.From[ 0 ].Address;
                mailItem.SenderName = msg.From[ 0 ].Name;
            }

            mailItem.Subject = msg.Subject;
        }

        private string GetHeader( string headerString, string header )
        {
            IList<string> lines = headerString.Split( new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries );
            int headerLineIndex = ListUtils.FindFirstIndex( lines, delegate( string s ) { return s.StartsWith( header, StringComparison.InvariantCultureIgnoreCase ); } );
            if ( headerLineIndex < 0 )
            {
                return null;
            }

            string headerLine = lines[headerLineIndex].Remove( 0, header.Length );
            if ( headerLine.StartsWith( ":" ) )
            {
                headerLine = headerLine.Substring( 1 );
            }

            headerLineIndex++;
            while ( headerLineIndex < lines.Count && ( lines[ headerLineIndex ].StartsWith( " " ) || lines[ headerLineIndex ].StartsWith( "\t" ) ) )
            {
                headerLine += lines[ headerLineIndex ];
                headerLineIndex++;
            }

            return headerLine.Trim();
        }

        
       

        #endregion

        #region IService Members

        public void SetProgressCallback( IProgressCallback callback )
        {
            this.callback = callback;
        }

        #endregion

        private Pop3 LoginAndStat()
        {
            Pop3 pop3 = new Pop3();
            pop3.User = this.user;
            pop3.Password = this.password;
            pop3.Connect( this.host, this.port );
            if ( pop3.HasTimeStamp )
            {
                pop3.APOPLogin();
            }
            else
            {
                pop3.Login();
            }

            pop3.GetAccountStat();
            return pop3;
        }


        private IProgressCallback callback;
        private string host;
        private int port;
        private string user;
        private string password;
    }
}
