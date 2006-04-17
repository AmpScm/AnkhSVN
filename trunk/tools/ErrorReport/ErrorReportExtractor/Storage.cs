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

namespace ErrorReportExtractor
{
    public class Storage : ErrorReportExtractor.IStorage
    {
        public Storage()
        {
            this.callback = new NullProgressCallback();
        }
        //public void Store(MailItem mailItem)
        //{
        //}

        public void Store(IEnumerable<IErrorReport> items)
        {
            QueriesTableAdapter itemsAdapter = new QueriesTableAdapter();
            StackTraceLinesTableAdapter linesAdapter = new StackTraceLinesTableAdapter();

            foreach (IErrorReport item in items)
            {
                //SqlTransaction trans = null;
                try
                {
                    if (callback.VerboseMode)
                    {
                        this.callback.Verbose("Inserting item with ID {0}, timestamp {1}", item.ID, item.ReceivedTime);
                    }
                    else
                    {
                        this.callback.Progress();
                    }
                    
                    //trans = itemsAdapter.BeginTransaction(conn);
                    //linesAdapter.JoinTransaction(trans);

                    int inserted = (int)
                    itemsAdapter.ImportErrorItem(item.ID, item.ReceivedTime, item.SenderEmail, item.SenderName, item.Body, item.Subject,
                        item.ExceptionType, "", "", item.MajorVersion, item.MinorVersion, item.PatchVersion, 
                        item.Revision, item.RepliedTo);

                    if (inserted == 0)
                    {
                        callback.Warning("Item with id {0} already exists in base", item.ID);
                        continue;
                    }

                    foreach (IStackTraceItem stItem in item.StackTrace)
                    {
                        linesAdapter.Insert(item.ID, stItem.MethodName, stItem.Parameters, stItem.Filename, stItem.LineNumber, stItem.SequenceNumber);
                    }

                    //trans.Commit();
                }
                catch (Exception )
                {
                    //if (trans != null)
                    //{
                    //    trans.Rollback();
                    //}   
                    throw;
                }
            }
        }

        public DateTime GetLastDate()
        {
            return DateTime.Now;
        }

        public IEnumerable<IErrorReport> GetAllReports()
        {
            //ErrorReportsDataSet ds = new ErrorReportsDataSet();
            //SqlDataAdapter adapter = new SqlDataAdapter( 
            //    "SELECT * FROM ErrorReportItems WHERE RepliedTo = 0 ORDER BY ReceivedTime ASC",
            //    Settings.Default.ErrorReportsConnectionString );
            //adapter.Fill( ds.ErrorReportItems );
            ErrorReportItemsTableAdapter adapter = new ErrorReportItemsTableAdapter();
            ErrorReportsDataSet.ErrorReportItemsDataTable table = adapter.GetAll();
            //ErrorReportItemsTableAdapter adapter = new ErrorReportItemsTableAdapter();
            this.callback.Info( "Got {0} error reports from the database.", table.Count );
            foreach ( ErrorReportsDataSet.ErrorReportItemsRow row in table)
            {
                ErrorReport report = new ErrorReport( row.ID, row.Subject, row.Body, row.SubmitterEmail, row.SubmitterName,
                    row.ReceivedTime );
                report.RepliedTo = row.RepliedTo;
                yield return report;
            }
            
        }



        public void AnswerReport( IErrorReport report, string replyText )
        {
            QueriesTableAdapter adapter = new QueriesTableAdapter();
            int count = (int)adapter.ReplyToReport( report.ID, replyText, Settings.Default.SenderEmail, report.SenderEmail,
                Settings.Default.SenderName, report.SenderName, null, null );
            if ( count != 1 )
            {
                callback.Error( "Attempting to mark message {0} as replied to, but {1} records in the database matched that ID",
                    report.ID, count );
            }
            else
            {
                report.RepliedTo = true;
            }
        }

        public void StorePotentialReplies( IEnumerable<IMailItem> items )
        {
            QueriesTableAdapter adapter = new QueriesTableAdapter();
            foreach ( IMailItem item in items )
            {
                int code = (int)adapter.InsertPotentialErrorReply( item.ID, item.ReceivedTime, item.SenderEmail, item.SenderName,
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
        }

        public void GetReplies( IErrorReport report )
        {
            ErrorRepliesTableAdapter adapter = new ErrorRepliesTableAdapter();
            this.callback.Verbose( "Retrieving replies to message with ID {0}", report.ID );
            ErrorReportsDataSet.ErrorRepliesDataTable table = adapter.GetRepliesToReport( report.ID );
            this.FillReplies(report, table.Select("ParentReply IS NULL"));
        }

        private void FillReplies( IMailItem item, DataRow[] rows )
        {
            foreach ( ErrorReportsDataSet.ErrorRepliesRow row in rows )
            {
                MailItem reply = new MailItem( "", "", row.ReplyText, row.SenderEmail, row.SenderName, row.ReplyTime );
                item.ReceiverEmail = row.RecipientEmail;
                item.ReceiverName = row.RecipientName;

                item.Replies.Add( reply );

                FillReplies( reply, row.Table.Select( "ParentReply = " + row.ReplyID ));
            }
        }


        public void SetProgressCallback( IProgressCallback callback )
        {
            this.callback = callback;
        }
        private IProgressCallback callback;

    }
}
