using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Fines.IssueZillaLib;
using System.Diagnostics;

namespace IssueZilla
{
    public partial class IssueList : UserControl
    {
        public IssueList()
        {
            InitializeComponent();

            Binding binding = this.urlLinkLabel.DataBindings[ "Text" ];
            binding.Format += new ConvertEventHandler( urlBinding_Format );
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
    }
}
