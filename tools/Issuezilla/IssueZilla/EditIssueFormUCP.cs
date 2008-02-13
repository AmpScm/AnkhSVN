using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Fines.IssueZillaLib;

using IServiceProvider = Fines.IssueZillaLib.IServiceProvider;
using System.Windows.Forms;

namespace IssueZilla
{
    class EditIssueFormUCP
    {
        public EditIssueFormUCP(issue issue, EditIssueForm form, IServiceProvider serviceProvider)
        {
            this.form = form;
            this.form.Posting += new EventHandler<CancelEventArgs>( form_Posting );

            this.metaDataSource = serviceProvider.GetService<IMetadataSource>();
            this.form.MetaDataSource = this.metaDataSource;
    
            this.issuePoster = serviceProvider.GetService<IIssuePoster>();

            this.form.Issue = issue;

            this.serviceProvider = serviceProvider;
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
                    "Updating issue " + this.form.Issue.issue_id,
                    worker );

                e.Cancel = false;
            }
        }

        private IMetadataSource metaDataSource;
        private IIssuePoster issuePoster;
        private EditIssueForm form;
        private IServiceProvider serviceProvider;
    }
}
