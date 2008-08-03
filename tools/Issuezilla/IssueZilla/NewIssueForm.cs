using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Fines.IssueZillaLib;

namespace IssueZilla
{
    public partial class NewIssueForm : Form
    {
        public event EventHandler<CancelEventArgs> Posting;

        public NewIssueForm()
        {
            InitializeComponent();
        }

        public issue Issue
        {
            get { return this.issueBindingSource.DataSource as issue; }
            set { this.issueBindingSource.DataSource = value; }
        }

        public string Comment
        {
            get { return this.commentsRichTextBox.Text; }
        }

        public IMetadataSource MetaDataSource
        {
            get { return this.metadataSourceBindingSource.DataSource as IMetadataSource; }
            set { this.metadataSourceBindingSource.DataSource = value; }
        }

        private void postButton_Click( object sender, EventArgs e )
        {
            CancelEventArgs args = new CancelEventArgs( true );
            if ( this.Posting != null )
            {
                this.Posting( this, args );
            }

            if ( !args.Cancel )
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}