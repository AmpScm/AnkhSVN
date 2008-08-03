using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Fines.IssueZillaLib;
using System.Diagnostics;
using Fines.Utils.Collections;

namespace IssueZilla
{
    public partial class IssueList : UserControl
    {
        public IssueList()
        {
            InitializeComponent();

            Binding binding = this.urlLinkLabel.DataBindings[ "Text" ];
            binding.Format += new ConvertEventHandler( urlBinding_Format );

            this.modifiedFont = new Font( this.issuesGrid.DefaultCellStyle.Font, FontStyle.Bold );
            this.newFont = new Font( this.issuesGrid.DefaultCellStyle.Font, FontStyle.Italic );

            this.issuesGrid.CellDoubleClick += new DataGridViewCellEventHandler( issuesGrid_CellDoubleClick );
        }

        public CommandBase RowClickCommand
        {
            get { return this.rowClickCommand; }
            set { this.rowClickCommand = value; }
        }

        void issuesGrid_CellDoubleClick( object sender, DataGridViewCellEventArgs e )
        {
            issue issue = issuesGrid.Rows[ e.RowIndex ].DataBoundItem as issue;
            if ( issue != null && this.rowClickCommand != null )
            {
                this.rowClickCommand.Execute();
            }
        }

        void urlBinding_Format( object sender, ConvertEventArgs e )
        {
            string issueId = e.Value as string;
            if ( issueId != null && this.UrlFormat != null )
            {
                e.Value = String.Format( this.UrlFormat, issueId );
            }
        }

        public IList<issue> Issues
        {
            get { return this.issueBindingSource.DataSource as IList<issue>; }
            set 
            {
                if ( value != null )
                {
                    this.issueBindingSource.DataSource = value;
                }
                else
                {
                    this.issueBindingSource.DataSource = typeof( issue );
                }
            }
        }

        public issue CurrentIssue
        {
            get { return this.issueBindingSource.Current as issue; }
            set { this.issueBindingSource.Position = this.issueBindingSource.IndexOf( value ); }
        }

        public string UrlFormat
        {
            get { return this.baseUrl; }
            set { this.baseUrl = value; }
        }

        private string baseUrl;

        private void urlLinkLabel_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            if ( e.Button == MouseButtons.Left )
            {
                Process.Start( this.urlLinkLabel.Text );
            }
        }

        private void copyUrlToolStripMenuItem_Click( object sender, EventArgs e )
        {
            Clipboard.SetText( this.urlLinkLabel.Text );
        }

        private void issuesGrid_CellFormatting( object sender, DataGridViewCellFormattingEventArgs e )
        {
            issue issue = this.issuesGrid.Rows[ e.RowIndex ].DataBoundItem as issue;
            if ( issue != null )
            {
                if ( issue.State == IssueState.New )
                {
                    e.CellStyle.Font = this.newFont;
                }
                else if( issue.State == IssueState.Modified )
                {
                    e.CellStyle.Font = this.modifiedFont;
                }
            }
           
        }

        private CommandBase rowClickCommand;
        private Font newFont;
        private Font modifiedFont;
    }
}
