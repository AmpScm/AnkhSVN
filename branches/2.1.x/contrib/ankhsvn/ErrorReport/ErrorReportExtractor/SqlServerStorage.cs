using System;
using System.Collections.Generic;
using System.Text;
using ErrorReportExtractor.ErrorReportsDataSetTableAdapters;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using System.Transactions;
using ErrorReportExtractor.Properties;
using System.Data;
using System.Diagnostics;

namespace ErrorReportExtractor
{
    public class SqlServerStorage : ErrorReportExtractor.IStorage
    {
        public SqlServerStorage()
        {
            this.callback = new NullProgressCallback();
        }
        //public void Store(MailItem mailItem)
        //{
        //}

        public void Store(IEnumerable<IErrorReport> items)
        {
            using(ConnectionScope cscope = new ConnectionScope())
            //using ( TransactionScope tran = new TransactionScope( TransactionScopeOption.Required, TimeSpan.Zero ) )
            {
                QueriesTableAdapter itemsAdapter = QueriesTableAdapter.Create();
                StackTraceLinesTableAdapter linesAdapter = StackTraceLinesTableAdapter.Create();

                foreach ( IErrorReport item in items )
                {
                    if ( callback.VerboseMode )
                    {
                        this.callback.Verbose( "Inserting item with ID {0}, timestamp {1}", item.InternetMailID, item.ReceivedTime );
                    }
                    else
                    {
                        this.callback.Progress();
                    }

                    //trans = itemsAdapter.BeginTransaction(conn);
                    //linesAdapter.JoinTransaction(trans);

                    int insertedID = (int)
                    itemsAdapter.ImportErrorItem( item.InternetMailID, item.ReceivedTime, item.SenderEmail, item.SenderName, item.Body, item.Subject,
                        item.ExceptionType, "", "", item.MajorVersion, item.MinorVersion, item.PatchVersion,
                        item.Revision, item.RepliedTo );

                    if ( insertedID == 0 )
                    {
                        callback.Warning( "Item with id {0} already exists in base", item.InternetMailID );
                        continue;
                    }
                    else if ( insertedID == -1 )
                    {
                        callback.Error( "Item with id {0}, received {1} from {2} could not be inserted",
                            item.InternetMailID, item.ReceivedTime, item.SenderEmail );
                        continue;
                    }
                    else
                    {
                        this.callback.Verbose( "Item with ID {0} successfully inserted.", insertedID );
                    }

                    foreach ( IStackTraceItem stItem in item.StackTrace )
                    {
                        // TODO: Get rid of first arg here
                        linesAdapter.Insert( "", stItem.MethodName, stItem.Parameters, stItem.Filename, stItem.LineNumber,
                            stItem.SequenceNumber, insertedID );
                    }
                }
                //tran.Complete();
            }
        }

        public void UpdateErrorReports( IEnumerable<IErrorReport> reports )
        {
           
            using(ConnectionScope scope = new ConnectionScope())
            using ( TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.Zero) )
            {
                ErrorReportsTableAdapter adapter = ErrorReportsTableAdapter.Create();

                foreach(IErrorReport report in reports)
                {
                    adapter.UpdateErrorReport( report.ExceptionType, report.ExceptionMessage, report.StackTrace.Text, 
                        report.MajorVersion, report.MinorVersion,
                        report.PatchVersion, report.Revision, report.DteVersion, report.MailItemID, report.MailItemID );
                }

                tran.Complete();
            }
        }

        public DateTime GetLastDate()
        {
            return DateTime.Now;
        }

        public IEnumerable<IErrorReport> GetAllReports()
        {
            using ( ConnectionScope scope = new ConnectionScope() )
            {
                ErrorReportsDataSet dataset = new ErrorReportsDataSet();

                ErrorReportItemsTableAdapter adapter = ErrorReportItemsTableAdapter.Create();
                ErrorReportsDataSet.ErrorReportItemsDataTable table = adapter.GetData();
                this.callback.Info( "Got {0} error reports from the database.", table.Count );
                foreach ( ErrorReportsDataSet.ErrorReportItemsRow row in table )
                {
                    ErrorReport report = new ErrorReport( row.ID, row.MailItemID, "", row.Body, row.SenderEmail, row.SenderName,
                        row.Time );
                    //report.RepliedTo = row.RepliedTo;
                    report.HasReplies = row.ReplyCount > 0;
                    report.Read = row.Read;
                    report.InternetMailID = row.MailID;
                    yield return report;
                }
            }
            
        }

        public IEnumerable<IMailItem> GetAllItems()
        {
            using ( ConnectionScope scope = new ConnectionScope() )
            {
                MailItemsTableAdapter adapter = MailItemsTableAdapter.Create();
                this.callback.Verbose( "Retrieving all mail items from database" );
                ErrorReportsDataSet.MailItemsDataTable table = adapter.GetMailItemsWithThreadCount();

                this.callback.Verbose( "Mail items table has {0} items.", table.Count );

                foreach ( ErrorReportsDataSet.MailItemsRow row in table )
                {
                    yield return CreateMailItem( row );
                }
            }
        }



        public void AnswerReport( IMailItem mailItem, string replyText )
        {
            using ( ConnectionScope scope = new ConnectionScope() )
            {
                QueriesTableAdapter adapter = QueriesTableAdapter.Create();
                int count = (int)adapter.ReplyToReport( mailItem.ErrorReportID, replyText, Settings.Default.SenderEmail, mailItem.SenderEmail,
                    Settings.Default.SenderName, mailItem.SenderName, null, null );
                if ( count != 1 )
                {
                    callback.Error( "Attempting to mark message {0} as replied to, but {1} records in the database matched that ID",
                        mailItem.MailItemID, count );
                }
                else
                {
                    mailItem.RepliedTo = true;
                }
            }
        }

        public void StorePotentialReplies( IEnumerable<IMailItem> items )
        {
            using(ConnectionScope scope = new ConnectionScope())
            using ( TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, TimeSpan.Zero) )
            {
                QueriesTableAdapter adapter = QueriesTableAdapter.Create();
                foreach ( IMailItem item in items )
                {
                    int code = (int)adapter.InsertPotentialErrorReply( item.InternetMailID, item.ReceivedTime, item.SenderEmail, item.SenderName,
                        item.ReceiverEmail, item.ReceiverName, item.Body, item.Subject, item.ReplyToID );
                    if ( code == 0 )
                    {
                        this.callback.Verbose( "Mail from {0} with subject {1} not a reply to any existing report.",
                            item.ReceivedTime, item.Subject );
                    }
                    else if ( code == 2 )
                    {
                        this.callback.Verbose( "Mail from {0} with subject {1} already in the database.",
                            item.ReceivedTime, item.Subject );
                    }
                    else
                    {
                        this.callback.Info( "Inserted mail from {0} with subject {1} as a reply to an existing report.",
                            item.ReceivedTime, item.Subject );
                    }
                }
                tran.Complete();
            }
        }

        public void GetReplies( IErrorReport report )
        {
            using ( ConnectionScope scope = new ConnectionScope() )
            {
                MailItemsTableAdapter adapter = MailItemsTableAdapter.Create();
                this.callback.Verbose( "Retrieving replies to report with ID {0}", report.ErrorReportID );
                ErrorReportsDataSet.MailItemsDataTable table = adapter.GetRepliesToReport( report.ErrorReportID );
                if ( table.Count > 0 )
                {
                    // Table should be ordered by date, so the first reply should be first
                    this.FillReplies( report, table.Select("ParentReply IS NULL OR ParentReply = " + report.MailItemID));
                }
            }
        }

        public void UpdateMailItem( IMailItem item )
        {
            using ( ConnectionScope scope = new ConnectionScope() )
            {
                MailItemsTableAdapter adapter = MailItemsTableAdapter.Create();
                this.callback.Verbose( "Updating message '{0}' as read.", item.MailItemID );

                int updated = adapter.UpdateMailItem( item.Body, item.SenderName, item.SenderEmail, item.ReceiverName, item.ReceiverEmail,
                    item.Read, item.MailItemID );
                if ( updated == 0 )
                {
                    this.callback.Warning( "Message with id {0} was not updated.", item.MailItemID );
                }
            }
        }
        


        private void FillReplies( IMailItem item, DataRow[] rows )
        {
            foreach ( ErrorReportsDataSet.MailItemsRow row in rows )
            {
                MailItem reply = CreateMailItem( row );

                item.Replies.Add( reply );

                FillReplies( reply, row.Table.Select( "ParentReply = " + row.ID));
            }
        }

        private static MailItem CreateMailItem( ErrorReportsDataSet.MailItemsRow row )
        {
            MailItem reply = new MailItem( row.ErrorReportID, row.ID, "", row.Body, row.SenderEmail, row.SenderName, row.Time );
            reply.ReceiverEmail = row.IsRecipientEmailNull() ? string.Empty : row.RecipientEmail;
            reply.ReceiverName = row.IsRecipientNameNull() ? String.Empty : row.RecipientName;
            reply.Read = row.Read;
            reply.RepliedTo = row.IsNumItemsInThreadNull() ? true : row.NumItemsInThread > 1;

            
            if ( !row.IsMailIDNull() )
            {
                reply.InternetMailID = row.MailID;
            }
            return reply;
        }


        public void SetProgressCallback( IProgressCallback callback )
        {
            this.callback = callback;
        }
        private IProgressCallback callback;

    }
}
