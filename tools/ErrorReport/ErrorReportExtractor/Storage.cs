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
                        this.callback.Verbose("Inserting item with ID {0}, timestamp {1}", item.InternetMailID, item.ReceivedTime);
                    }
                    else
                    {
                        this.callback.Progress();
                    }
                    
                    //trans = itemsAdapter.BeginTransaction(conn);
                    //linesAdapter.JoinTransaction(trans);

                    int insertedID = (int)
                    itemsAdapter.ImportErrorItem(item.InternetMailID, item.ReceivedTime, item.SenderEmail, item.SenderName, item.Body, item.Subject,
                        item.ExceptionType, "", "", item.MajorVersion, item.MinorVersion, item.PatchVersion, 
                        item.Revision, item.RepliedTo);

                    if (insertedID == 0)
                    {
                        callback.Warning("Item with id {0} already exists in base", item.InternetMailID);
                        continue;
                    }
                    else if ( insertedID == -1 )
                    {
                        callback.Error( "Item with id {0}, received {1} from {2} could not be inserted", 
                            item.InternetMailID, item.ReceivedTime, item.SenderEmail );
                        continue;
                    }

                    foreach (IStackTraceItem stItem in item.StackTrace)
                    {
                        // TODO: Get rid of first arg here
                        linesAdapter.Insert("", stItem.MethodName, stItem.Parameters, stItem.Filename, stItem.LineNumber, 
                            stItem.SequenceNumber, insertedID);
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
            ErrorReportsDataSet dataset = new ErrorReportsDataSet();

            ErrorReportItemsTableAdapter adapter = new ErrorReportItemsTableAdapter();
            ErrorReportsDataSet.ErrorReportItemsDataTable table = adapter.GetData();
            //ErrorReportItemsTableAdapter adapter = new ErrorReportItemsTableAdapter();
            this.callback.Info( "Got {0} error reports from the database.", table.Count );
            foreach ( ErrorReportsDataSet.ErrorReportItemsRow row in table)
            {
                ErrorReport report = new ErrorReport( row.ID, "", row.Body, row.SenderEmail, row.SenderName,
                    row.Time );
                //report.RepliedTo = row.RepliedTo;
                report.HasReplies = row.ReplyCount > 0;
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
        }

        public void GetReplies( IErrorReport report )
        {
            MailItemsTableAdapter adapter = new MailItemsTableAdapter();
            this.callback.Verbose( "Retrieving replies to message with ID {0}", report.ID );
            ErrorReportsDataSet.MailItemsDataTable table = adapter.GetRepliesToReport( report.ID );
            this.FillReplies(report, table.Select("ParentReply IS NULL"));
        }

        private void FillReplies( IMailItem item, DataRow[] rows )
        {
            foreach ( ErrorReportsDataSet.MailItemsRow row in rows )
            {
                MailItem reply = new MailItem( row.ID, "", row.Body, row.SenderEmail, row.SenderName, row.Time );
                reply.ReceiverEmail = row.RecipientEmail;
                reply.ReceiverName = row.RecipientName;

                if ( !row.IsMailIDNull() )
                {
                    reply.InternetMailID = row.MailID;
                }

                item.Replies.Add( reply );

                FillReplies( reply, row.Table.Select( "ParentReply = " + row.ID));
            }
        }


        public void SetProgressCallback( IProgressCallback callback )
        {
            this.callback = callback;
        }
        private IProgressCallback callback;

    }
}
