using System;
using System.Collections.Generic;
using System.Text;
using ErrorReportExtractor;
using System.ComponentModel;
using System.Windows.Forms;

namespace ErrorReport.GUI
{
    public class ImportDialogUCP
    {
        public event EventHandler ImportFinished;

        public ImportDialogUCP( IProgressCallback progressCallback, IStorage storage, ISynchronizeInvoke invoker )
        {
            this.callback = progressCallback;
            this.storage = storage;
            this.invoker = invoker;
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
            set { folderPath = value; }
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



        public void RunImport()
        {
            OutlookContainer container = new OutlookContainer();
            container.SetProgressCallback( this.callback );

            MethodInvoker proc = delegate
            {
                DateTime? itemsAfter = this.ItemsAfterEnabled ? (DateTime?)this.ItemsAfter : null;
                if ( this.ImportReports )
                {
                    this.callback.Info( "Starting import of error reports from Outlook folder {0}.", this.FolderPath );
                    this.storage.Store( container.GetItems( this.FolderPath, itemsAfter ) );
                }

                if ( this.ImportReplies )
                {
                    this.callback.Info( "Starting import of error replies from Outlook folder {0}.", this.FolderPath );
                    this.storage.StorePotentialReplies( container.GetPotentialReplies( this.FolderPath ) );
                }
            };

            proc.BeginInvoke( delegate( IAsyncResult result )
            {
                try
                {

                    proc.EndInvoke( result );
                    this.OnImportFinished();
                }
                catch ( Exception ex )
                {
                    this.callback.Exception( ex );
                }
            }, null );
            
        }

        private void OnImportFinished()
        {

            MethodInvoker invoker = delegate
            {
                if ( this.ImportFinished != null )
                {
                    this.ImportFinished( this, EventArgs.Empty );
                }
            };
            this.invoker.Invoke( invoker, null );
        }

        private void PerformImport()
        {
            
            
        }

        private bool itemsAfterEnabled;

        private DateTime itemsAfter = DateTime.Now;

        
        private IStorage storage;
        private string folderPath;
        private IProgressCallback callback;
        private bool importReports;
        private bool importReplies;
        private ISynchronizeInvoke invoker;
    }
}
