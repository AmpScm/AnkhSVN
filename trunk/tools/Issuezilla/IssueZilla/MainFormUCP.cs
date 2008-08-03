using System;
using System.Collections.Generic;
using System.Text;
using Fines.IssueZillaLib;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

using IServiceProvider = Fines.IssueZillaLib.IServiceProvider;
using Fines.Utils.Collections;

namespace IssueZilla
{
    interface IProgressDialog : IDisposable
    {
        void ShowError( Exception exception );
    }
    public class MainFormUCP : IServiceProvider
    {
        public MainFormUCP( MainForm form )
        {
            this.form = form;

            this.form.FilterControl.FilterChanged += new EventHandler( FilterControl_FilterChanged );
            this.form.FilterControl.SearchTextChanged += new EventHandler( FilterControl_SearchTextChanged );

            this.form.IssueList.RowClickCommand = new RowClickCommand( this );

            this.services = new Dictionary<Type, object>();
            this.services.Add( typeof( IServiceProvider ), this.WindowHandle );
            this.services.Add( typeof( IBackgroundWorkerService ), new BackgroundWorkerService( this.WindowHandle ) );

        }

        internal IWin32Window WindowHandle
        {
            get { return this.form; }
        }

        internal ISynchronizeInvoke Invoker
        {
            get { return this.form; }
        }

        internal IIssuePoster IssuePoster
        {
            get { return this.source as IIssuePoster; }
        }

        internal issue SelectedIssue
        {
            get { return this.form.CurrentIssue; }
            set { this.form.CurrentIssue = value; }
        }

        internal void LoadIssues(IIssueSource source)
        {
            this.source = source;
            this.services.Add( typeof( IMetadataSource ), this.source as IMetadataSource );
            this.services.Add( typeof( IIssuePoster ), this.source as IIssuePoster );

            this.GetService<IBackgroundWorkerService>().DoWork( "Loading metadata",
                delegate
                {
                    this.GetService<IMetadataSource>().LoadMetaData();
                } );

            this.GetService<IBackgroundWorkerService>().DoWork( "Loading issues",
               new LoadIssuesWorker(this)
            );


        }

        internal void SetIssues( issuezilla issueZilla )
        {
            this.issueZilla = issueZilla;
            ResetIssues( this.issueZilla.issue );
        }

        private void ResetIssues(IList<issue> issues)
        {
            this.allIssues = new List<issue>( issues );
            this.filteredIssues = new List<issue>( this.allIssues );
            this.searchFilteredIssues = new List<issue>( this.allIssues );

            this.form.SetIssues( this.searchFilteredIssues );
            this.form.UrlFormat = String.Format( "{0}show_bug.cgi?id={{0}}", this.issueZilla.urlbase );

            FilterCriteria criteria = new FilterCriteria( this.allIssues );
            this.form.FilterControl.SetFilterCriteria( criteria );
        }

        internal void StoreIssues( FileIssueSource source )
        {
            this.issueZilla.issue = ListUtils.ToArray( this.allIssues ); ;
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

        public T GetService<T>() where T : class
        {
            if ( services.ContainsKey(typeof(T)) )
            {
                return services[ typeof( T ) ] as T;
            }
            else
            {
                return null;
            }
        }

        public void AddNewIssue( issue issue )
        {
            this.allIssues.Add( issue );
            this.ResetIssues( allIssues );            
        }

        private Dictionary<Type, object> services;
        private IIssueSource source;
        private MainForm form;
        private issuezilla issueZilla;
        private IList<issue> allIssues;
        private IList<issue> filteredIssues;
        private IList<issue> searchFilteredIssues;
        private SearchIssuesWorker searchIssuesWorker;

        private class LoadIssuesWorker : IBackgroundOperation
        {
            public LoadIssuesWorker( MainFormUCP ucp)
            {
                this.ucp = ucp;
            }
           
            public void Work( )
            {

                this.issueZilla = ucp.source.GetAllIssues();
            }

            public void WorkCompleted( RunWorkerCompletedEventArgs e )
            {
                if ( this.issueZilla != null )
                {
                    this.ucp.SetIssues( this.issueZilla ); 
                }
            }

            private MainFormUCP ucp;
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



        #region IServiceProvider Members

        

        #endregion

        
    }
}
