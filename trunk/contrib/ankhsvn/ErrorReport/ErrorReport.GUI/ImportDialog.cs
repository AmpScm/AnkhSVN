using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ErrorReportExtractor;

namespace ErrorReport.GUI
{
    public partial class ImportDialog : Form
    {
        public ImportDialog(ImportDialogUCP ucp)
        {
            InitializeComponent();
            this.ucp = ucp;
            this.DataBind();
        }

        private void DataBind()
        {
            this.importRepliesCheckBox.DataBindings.Add( "Checked", this.ucp, "ImportReplies" 
                ).DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            this.importReportsCheckBox.DataBindings.Add( "Checked", this.ucp, "ImportReports" 
                ).DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
            this.folderTextBox.DataBindings.Add( "Text", this.ucp, "FolderPath" 
                ).DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

            reportsIndexPanel.DataBindings.Add( "Enabled", this.importReportsCheckBox, "Checked" );
            repliesIndexPanel.DataBindings.Add( "Enabled", this.importRepliesCheckBox, "Checked" );

            reportsStartIndexTextBox.DataBindings.Add( "Text", this.ucp, "ReportsStartIndex" );
            repliesStartIndexTextBox.DataBindings.Add( "Text", this.ucp, "RepliesStartIndex" );

        }

        private void importButton_Click( object sender, EventArgs e )
        {
            this.ucp.ImportFinished += delegate { this.DialogResult = DialogResult.OK; this.Close(); };
            this.progressBar.Enabled = this.progressBar.Visible = true;
            this.ucp.RunImport();


        }

        protected override void OnFormClosed( FormClosedEventArgs e )
        {
            this.ucp.StoreSettings();
        }

        private ImportDialogUCP ucp;
    }
}