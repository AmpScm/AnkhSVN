using System;
using System.Collections.Generic;
using System.Text;
using Fines.IssueZillaLib;
using System.ComponentModel;
using System.Threading;

namespace IssueZilla
{
    interface IProgressDialog : IDisposable
    {
        void ShowError( Exception exception );
    }
    public class MainFormUCP
    {
        public MainFormUCP( MainForm form )
        {
            this.form = form;

            this.form.FilterControl.FilterChanged += new EventHandler( FilterControl_FilterChanged );
            this.form.FilterControl.SearchTextChanged += new EventHandler( FilterControl_SearchTextChanged );

        }

        
        internal void LoadIssues(IIssueSource source)
        {
            this.source = source;
            new LoadIssuesWorker( this ).Run();
        }

        internal void SetIssues( issuezilla issueZilla )
        {
            this.issueZilla = issueZilla;
            this.allIssues = this.issueZilla.issue;
            this.filteredIssues = new List<issue>( this.allIssues );
            this.searchFilteredIssues = new List<issue>( this.allIssues );

            this.form.SetIssues( this.searchFilteredIssues );
            this.form.UrlFormat = String.Format( "{0}show_bug.cgi?id={{0}}", this.issueZilla.urlbase );

            FilterCriteria criteria = new FilterCriteria( this.allIssues );
            this.form.FilterControl.SetFilterCriteria( criteria );
        }

        internal void StoreIssues( FileIssueSource source )
        {
            this.issueZilla.issue = this.allIssues;
            source.StoreIssues( this.issueZilla );
        }

        void FilterControl_FilterChanged( object sender, EventArgs e )
        {
            this.filteredIssues = this.form.FilterControl.Filter.ApplyFilter( this.allIssues );
            this.searchFilteredIssues = new BindingList<issue>( this.filteredIssues );
            this.form.SetIssues( this.searchFilteredIssues );
        }

        void FilterControl_SearchTextChanged( object sender, EventArgs e )
        {
            if ( this.searchIssuesWorker != null )
            {
                this.searchIssuesWorker.CancelAsync();
            }
            this.searchIssuesWorker = new SearchIssuesWorker();
            this.searchIssuesWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler( searchIssuesWorker_RunWorkerCompleted );
            this.searchIssuesWorker.Run( this.filteredIssues, this.form.FilterControl.SearchText );
        }

        void searchIssuesWorker_RunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( !e.Cancelled )
            {
                SearchIssuesWorker worker = sender as SearchIssuesWorker;
                this.form.SetIssues( worker.SearchFilteredIssues );
            }
        }


        private IIssueSource source;
        private MainForm form;
        private issuezilla issueZilla;
        private issue[] allIssues;
        private IList<issue> filteredIssues;
        private IList<issue> searchFilteredIssues;
        private SearchIssuesWorker searchIssuesWorker;

        private class LoadIssuesWorker : BackgroundWorker
        {
            public LoadIssuesWorker( MainFormUCP ucp)
            {
                this.ucp = ucp;
            }
           
            protected override void OnDoWork( DoWorkEventArgs e )
            {
                base.OnDoWork( e );

                this.issueZilla = ucp.source.GetAllIssues();
            }

            protected override void OnRunWorkerCompleted( RunWorkerCompletedEventArgs e )
            {
                base.OnRunWorkerCompleted( e );

                if ( e.Error != null )
                {
                    this.dialog.ShowError( e.Error );
                }

                this.dialog.Dispose();

                if ( this.issueZilla != null )
                {
                    this.ucp.SetIssues( this.issueZilla ); 
                }
            }

            internal void Run()
            {
                this.dialog = ucp.form.StartOperation();
                this.RunWorkerAsync();
            }

            private MainFormUCP ucp;
            private IProgressDialog dialog;
            private issuezilla issueZilla;
        }

        private class SearchIssuesWorker : BackgroundWorker
        {
            public SearchIssuesWorker()
            {
                this.WorkerReportsProgress = true;
                this.WorkerSupportsCancellation = true;
            }

            public void Run( IList<issue> issues, string searchString )
            {
                this.issues = issues;
                this.searchFilteredIssues = new List<issue>();
                this.searchString = searchString;

                this.RunWorkerAsync();
            }

            public IList<issue> SearchFilteredIssues
            {
                get { return this.searchFilteredIssues; }
            }


            protected override void OnDoWork( DoWorkEventArgs e )
            {
                base.OnDoWork( e );
                foreach ( issue issue in issues )
                {
                    if ( this.CancellationPending )
                    {
                        break;
                    }

                    if ( issue.ContainsText(this.searchString) )
                    {
                        this.searchFilteredIssues.Add( issue );
                    }
                }
                this.manualResetEvent.Set();
            }

            protected override void OnRunWorkerCompleted( RunWorkerCompletedEventArgs e )
            {
                base.OnRunWorkerCompleted( e );
            }

           

            private ManualResetEvent manualResetEvent = new ManualResetEvent(true);
            private IList<issue> issues;
            private IList<issue> searchFilteredIssues;
            private string searchString;
        }

        
    }
}
