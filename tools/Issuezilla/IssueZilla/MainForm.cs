using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Fines.IssueZillaLib;
using IssueZilla.Properties;

namespace IssueZilla
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public FilterControl FilterControl
        {
            get { return this.filterControl; }
        }

        public IssueList IssueList
        {
            get { return this.issueList; }
        }

        public issue CurrentIssue
        {
            get { return this.issueList.CurrentIssue; }
            set { this.issueList.CurrentIssue = value; }
        }

        private void MainForm_Load( object sender, EventArgs e )
        {
            this.ucp = new MainFormUCP( this );

            StartupCommand command = new StartupCommand( this.ucp );
            command.Execute();

            this.saveToolStripMenuItem.Tag = new SaveCommand( this.ucp );
            this.newToolStripMenuItem.Tag = new NewIssueCommand( this.ucp );
        }

        internal IProgressDialog StartOperation()
        {
            ProgressDialog dialog = new ProgressDialog( this );
            dialog.Show(this);
            return dialog;
        }

        private MainFormUCP ucp;

        internal void SetIssues( IList<issue> issues )
        {
            this.issueList.Issues = issues;
        }

        private void MainForm_FormClosed( object sender, FormClosedEventArgs e )
        {
            Settings.Default.Save();
        }

        private void ExecuteClick( object sender, EventArgs e )
        {
            CommandBase cmd = null;
            Control control = sender as Control;
            ToolStripItem item = sender as ToolStripItem;
            if ( control != null )
            {
                cmd = control.Tag as CommandBase;
            }
            else if ( item != null )
            {
                cmd = item.Tag as CommandBase;
            }
            if ( cmd != null )
            {
                cmd.Execute();
            }
        }

        private void MainForm_KeyDown( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.F3 )
            {
                this.filterControl.FocusSearchTextBox();
            }
        }

        public string UrlFormat
        {
            get { return this.issueList.UrlFormat; }
            set { this.issueList.UrlFormat = value; }
        }
    }
}