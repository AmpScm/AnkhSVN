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
    public partial class EditIssueForm : Form
    {
        public event EventHandler<CancelEventArgs> Posting;

        public EditIssueForm()
        {
            InitializeComponent();

            this.actionComboBox.SelectedIndexChanged += new EventHandler( actionComboBox_SelectedIndexChanged );
        }

        

        public issue Issue
        {
            get { return this.issueBindingSource.DataSource as issue; }
            set 
            { 
                this.issueBindingSource.DataSource = value;
                this.InitializeActions();
            }
        }

        private void InitializeActions()
        {
            this.actionComboBox.Items.Add( new NoChangeEditIssueAction( this.Issue ) );
            this.actionComboBox.Items.Add( new AcceptIssueEditAction( this.Issue ) );
            this.actionComboBox.Items.Add( new ResolveIssueEditAction( this.Issue, this.MetaDataSource ) );

            this.actionComboBox.SelectedIndex = 0;
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
                IEditIssueAction action = this.actionComboBox.SelectedItem as IEditIssueAction;

                if ( action != null )
                {
                    action.Commit();
                    this.Posting( this, args ); 
                }
            }

            if ( !args.Cancel )
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        void actionComboBox_SelectedIndexChanged( object sender, EventArgs e )
        {
            this.editIssueActionPanel.Controls.Clear();

            IEditIssueAction action = this.actionComboBox.SelectedItem as IEditIssueAction;

            if ( action != null )
            {
                UserControl control = action.EditingControl;
                if ( control != null )
                {
                    this.editIssueActionPanel.Controls.Add( control );
                }
            }
        }
    }
}