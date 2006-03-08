using System;
using System.Collections.Generic;
using System.Text;
using ErrorReportExtractor.ErrorReportsDataSetTableAdapters;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using System.Transactions;

namespace ErrorReportExtractor
{
    class Storage
    {
        public Storage(IProgressCallback callback)
        {
            this.callback = callback;
        }
        //public void Store(MailItem mailItem)
        //{
        //}

        public void Store(IEnumerable<IErrorReport> items)
        {
            ImportErrorItemTableAdapter itemsAdapter = new ImportErrorItemTableAdapter();
            StackTraceLinesTableAdapter linesAdapter = new StackTraceLinesTableAdapter();
            ErrorReportExtractor.ErrorReportsDataSet.ImportErrorItemDataTable dummyTable = 
                new ErrorReportsDataSet.ImportErrorItemDataTable(); ;
            SqlConnection conn = itemsAdapter.Connection;
            linesAdapter.Connection = conn;

            conn.Open();

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

                    int inserted = 
                    itemsAdapter.Fill(dummyTable, item.ID, item.ReceivedTime, item.SenderName, item.Body, item.Subject,
                        item.ExceptionType, "", "", item.MajorVersion, item.MinorVersion, item.PatchVersion, 
                        item.Revision, item.RepliedTo);

                    if (dummyTable[0].Column1 == 0)
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

        private IProgressCallback callback;
    }
}
