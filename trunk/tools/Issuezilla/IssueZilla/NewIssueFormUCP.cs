using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Fines.IssueZillaLib;

using IServiceProvider = Fines.IssueZillaLib.IServiceProvider;

namespace IssueZilla
{
    class NewIssueFormUCP
    {
        public NewIssueFormUCP(NewIssueForm form, IServiceProvider serviceProvider)
        {
            this.form = form;
            this.form.Posting += new EventHandler<CancelEventArgs>( form_Posting );

            this.metaDataSource = serviceProvider.GetService<IMetadataSource>();
            this.form.MetaDataSource = this.metaDataSource;
    
            this.issuePoster = serviceProvider.GetService<IIssuePoster>();
            this.serviceProvider = serviceProvider;

            this.form.Issue = InitializeIssue();
        }

        public issue Issue
        {
            get { return this.form.Issue; }
        }


        

        void form_Posting( object sender, System.ComponentModel.CancelEventArgs e )
        {
            if ( this.issuePoster != null )
            {
                string comment = this.form.Comment;
                DoWorkEventHandler worker = delegate
                {
                    this.form.Issue.reporter = "arild_fines";
                    this.issuePoster.UpdateIssue( this.form.Issue, comment );
                };

                this.serviceProvider.GetService<IBackgroundWorkerService>().DoWork(
                    "Posting issue", worker );

                e.Cancel = false;
            }
            e.Cancel = false;
        }

        private issue InitializeIssue()
        {
            issue issue = new issue();
            issue.component = this.GetDefault( this.metaDataSource.Components );
            issue.issue_type = this.GetDefault( this.metaDataSource.IssueTypes );
            issue.op_sys = this.GetDefault( this.metaDataSource.OperatingSystems );
            issue.rep_platform = this.GetDefault( this.metaDataSource.Platforms );
            issue.subcomponent = this.GetDefault( this.metaDataSource.SubComponents );
            issue.version = this.GetDefault( this.metaDataSource.Versions );
            issue.priority = this.GetDefault( this.metaDataSource.Priorities );
            issue.issue_file_loc = "http://";

            return issue;
        }

        private string GetDefault( IList<MetadataItem<string, string>> iList )
        {
            if ( iList.Count > 0 )
            {
                return iList[ 0 ].Key;
            }
            else
            {
                return String.Empty;
            }
        }

        private IMetadataSource metaDataSource;
        private IServiceProvider serviceProvider;
        private IIssuePoster issuePoster;
        private NewIssueForm form;
    }
}
