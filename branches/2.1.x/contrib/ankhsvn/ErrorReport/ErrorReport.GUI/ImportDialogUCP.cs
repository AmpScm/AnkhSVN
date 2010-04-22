using System;
using System.Collections.Generic;
using System.Text;
using ErrorReportExtractor;
using System.ComponentModel;
using System.Windows.Forms;
using ErrorReport.GUI.Properties;
using System.Collections;
using System.Data;
using System.Threading;

namespace ErrorReport.GUI
{
    public class ImportDialogUCP
    {
        public event EventHandler ImportFinished;
        public event EventHandler ReportsStartIndexChanged;
        public event EventHandler RepliesStartIndexChanged;

        public ImportDialogUCP( IProgressCallback progressCallback, IStorage storage, ISynchronizeInvoke invoker )
        {
            this.callback = progressCallback;
            this.storage = storage;
            this.invoker = invoker;

            this.settings = new Settings();
            this.settings.Reload();

            this.folderPath = this.settings.OutlookFolder;
            if ( this.folderPath == null )
            {
                this.folderPath = "";
            }

            GetStartIndicesFromSettings();

            this.settings.Save();
        }

       

        public bool ImportReplies
        {
            get { return importReplies; }
            set { importReplies = value; }
        }

        public bool ImportReports
        {
            get { return importReports; }
            set { importReports = value; }
        }

        public string FolderPath
        {
            get { return folderPath; }
            set 
            { 
                folderPath = value;
                this.GetStartIndicesFromSettings();
                if ( this.ReportsStartIndexChanged != null )
                {
                    this.ReportsStartIndexChanged( this, EventArgs.Empty );
                }
                if ( this.RepliesStartIndexChanged != null )
                {
                    this.RepliesStartIndexChanged( this, EventArgs.Empty );
                }
            }
        }

        public bool ItemsAfterEnabled
        {
            get { return itemsAfterEnabled; }
            set { itemsAfterEnabled = value; }
        }

        public DateTime ItemsAfter
        {
            get { return itemsAfter; }
            set { itemsAfter = value; }
        }

        public int ReportsStartIndex
        {
            get 
            {
                return this.reportsStartIndex;
            }
            set { this.reportsStartIndex = value; }
        }

        public int RepliesStartIndex
        {
            get
            {
                return this.repliesStartIndex;
            }
            set { this.repliesStartIndex = value; }
        }




        public void RunImport()
        {
            ////OutlookContainer container = new OutlookContainer();
            //container.SetProgressCallback( this.callback );

            //ThreadWorker worker = new ThreadWorker(this.invoker);
            //worker.Work += delegate
            //{
            //    if ( this.ImportReplies )
            //    {
            //        this.callback.Info( "Starting import of error replies from Outlook folder {0}.", this.FolderPath );
            //        int lastIndex;
            //        IEnumerable<IMailItem> items = container.GetPotentialReplies( this.folderPath, this.RepliesStartIndex, out lastIndex );
            //        this.storage.StorePotentialReplies( items );
            //        this.StoreStartIndex( this.settings.LastOutlookReplyIndexChecked, lastIndex );
            //    }

            //    if ( this.ImportReports )
            //    {
            //        this.callback.Info( "Starting import of error reports from Outlook folder {0}.", this.FolderPath );
            //        int lastIndex;
            //        IEnumerable<IErrorReport> items = container.GetItems( this.FolderPath, this.ReportsStartIndex, out lastIndex );
            //        this.storage.Store( items );
            //        this.StoreStartIndex( this.settings.LastOutlookReportIndexChecked, lastIndex );
            //    }
            //};

            //worker.WorkFinished += delegate 
            //{ 
            //    this.callback.Info( "Import finished." );
            //    if ( this.ImportFinished != null )
            //    {
            //        this.ImportFinished( this, EventArgs.Empty );
            //    }
            //};

            //worker.Exception += delegate( object sender, ThreadExceptionEventArgs args )
            //{
            //    this.callback.Exception( args.Exception );
            //};

            //worker.Start();
            
        }

        

        public void StoreSettings()
        {
            this.settings.OutlookFolder = this.folderPath;
            this.settings.Save();
        }

        private void StoreStartIndex( DataTable dataTable, int lastIndex )
        {
            DataRow[] rows = dataTable.Select( String.Format( "FolderPath='{0}'", this.FolderPath ) );
            if ( rows.Length == 0 )
            {
                dataTable.Rows.Add( this.folderPath, lastIndex );
            }
            else
            {
                rows[ 0 ][ "StartIndex" ] = lastIndex;
            }
        }

        private void GetStartIndicesFromSettings()
        {

            if ( this.settings.LastOutlookReplyIndexChecked == null )
            {
                this.settings.LastOutlookReplyIndexChecked = new DataTable();
                this.SetupSchemaForLastIndex( this.settings.LastOutlookReplyIndexChecked );
                this.settings.LastOutlookReplyIndexChecked.Rows.Add( this.folderPath, 1 );
            }

            if ( this.settings.LastOutlookReportIndexChecked == null )
            {
                this.settings.LastOutlookReportIndexChecked = new DataTable();
                this.SetupSchemaForLastIndex( this.settings.LastOutlookReportIndexChecked );
                this.settings.LastOutlookReportIndexChecked.Rows.Add( this.folderPath, 1 );
            }

            this.repliesStartIndex = GetStartIndex( this.folderPath, this.settings.LastOutlookReplyIndexChecked );
            this.reportsStartIndex = GetStartIndex( this.folderPath, this.settings.LastOutlookReportIndexChecked );

            //object o = this.GetLastIndexChecked( "Report", this.folderPath );
            //if ( o != null )
            //{
            //    this.reportsStartIndex = (int)o;
            //}
            //else
            //{
            //    this.reportsStartIndex = 1;
            //    this.SetLastIndexChecked( "Report", this.folderPath, this.reportsStartIndex );
            //}

            //o = this.GetLastIndexChecked( "Reply", this.folderPath );
            //if ( o != null )
            //{
            //    this.repliesStartIndex = (int)o;
            //}
            //else
            //{
            //    this.repliesStartIndex = 1;
            //    this.SetLastIndexChecked( "Reply", this.folderPath, this.repliesStartIndex );
            //}
        }
       
        private int GetStartIndex( string folderPath, DataTable dataTable )
        {
            DataRow[] rows = dataTable.Select( String.Format( "FolderPath='{0}'", folderPath ) );
            DataRow row = null;
            if ( rows.Length == 0 )
            {
                row = dataTable.Rows.Add( folderPath, 1 );
            }
            else
            {
                row = rows[ 0 ];
            }
            return (int)row[ "StartIndex" ];
        }

        //private void SetLastIndexChecked( string keyName, string folderPath, int index )
        //{
        //    string fullKeyName = GetFullKeyName( keyName, folderPath );
        //    this.settings[ fullKeyName ] = index;
        //}

        //private object GetLastIndexChecked( string keyName, string folderPath )
        //{
        //    return this.settings.L
        //    return this.settings[ fullKeyName ];
        //}

        //private static string GetFullKeyName( string keyName, string folderPath )
        //{
        //    string fullKeyName = String.Format( "{0}IndexChecked[{1}]", keyName, folderPath );
        //    return fullKeyName;
        //}

        private void SetupSchemaForLastIndex( DataTable dataTable )
        {
            dataTable.Columns.Add( "FolderPath", typeof( string ) );
            dataTable.Columns.Add( "StartIndex", typeof( int ) );

            dataTable.TableName = "LastIndex";

            dataTable.Constraints.Add( "PK", dataTable.Columns[ 0 ], true );
        }

        private bool itemsAfterEnabled;

        private DateTime itemsAfter = DateTime.Now;

        
        private IStorage storage;
        private string folderPath;
        private IProgressCallback callback;
        private bool importReports;
        private bool importReplies;
        private ISynchronizeInvoke invoker;
        private int reportsStartIndex;
        private int repliesStartIndex;

        private Settings settings;
    }
}
