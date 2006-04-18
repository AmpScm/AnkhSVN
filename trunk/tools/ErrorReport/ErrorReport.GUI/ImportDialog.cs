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

            this.enableItemsAfterCheckBox.DataBindings.Add( "Checked", this.ucp, "ItemsAfterEnabled", false, DataSourceUpdateMode.OnPropertyChanged );
            this.itemsAfterPicker.DataBindings.Add( "Value", this.ucp, "ItemsAfter", false, DataSourceUpdateMode.OnPropertyChanged );
            this.itemsAfterPicker.DataBindings.Add( "Enabled", this.enableItemsAfterCheckBox, "Checked", false, DataSourceUpdateMode.Never,
                DateTime.MinValue );
        }

        private void importButton_Click( object sender, EventArgs e )
        {
            this.ucp.ImportFinished += delegate { this.DialogResult = DialogResult.OK; this.Close(); };
            this.progressBar.Enabled = this.progressBar.Visible = true;
            this.ucp.RunImport();

        }

        private ImportDialogUCP ucp;
    }
}