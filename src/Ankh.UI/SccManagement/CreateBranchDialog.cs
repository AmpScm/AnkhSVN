using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.RepositoryExplorer;
using SharpSvn;
using Ankh.Scc;

namespace Ankh.UI.SccManagement
{
    public partial class CreateBranchDialog : VSContainerForm
    {
        public CreateBranchDialog()
        {
            InitializeComponent();
        }

        public string SrcFolder
        {
            get { return fromFolderBox.Text; }
            set { fromFolderBox.Text = value; }
        }

        public Uri SrcUri
        {
            get 
            {
                Uri r;

                if (!string.IsNullOrEmpty(fromUrlBox.Text) &&
                    Uri.TryCreate(fromUrlBox.Text, UriKind.Absolute, out r))
                {
                    return r;
                }

                return null;
            }
            set { fromUrlBox.Text = value.AbsoluteUri; }
        }

        public bool CopyFromUri
        {
            get { return !this.wcVersionRadio.Checked; }
        }

        public SvnRevision SelectedRevision
        {
            get
            {
                if (headVersionRadio.Checked)
                    return SvnRevision.Head;
                else if (wcVersionRadio.Checked)
                    return SvnRevision.Working;
                else
                    return new SvnRevision(Revision);
            }
        }


        bool _noTypeChange;

        public long Revision
        {
            get { return (long)versionBox.Value; }
            set { _noTypeChange = true; versionBox.Value = value; _noTypeChange = false; }
        }

        public bool SwitchToBranch
        {
            get { return switchBox.Checked; }
            set { switchBox.Checked = value; }
        }

        public Uri NewDirectoryName
        {
            get 
            {
                Uri r;

                if (!string.IsNullOrEmpty(toUrlBox.Text) && Uri.TryCreate(toUrlBox.Text, UriKind.Absolute, out r))
                {
                    return r;
                }
                else
                    return null;
            }
            set
            {
                if (value == null)
                    toUrlBox.Text = "";
                else
                    toUrlBox.Text = value.AbsoluteUri;
            }
        }

        bool _editSource;
        public bool EditSource
        {
            get { return _editSource; }
            set { fromFolderBox.ReadOnly = fromUrlBox.ReadOnly = !(_editSource = value); }
        }

        public string LogMessage
        {
            get { return logMessage.Text; }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if(!_noTypeChange)
                specificVersionRadio.Checked = true;

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void toUrlBrowse_Click(object sender, EventArgs e)
        {
            using (RepositoryFolderBrowserDialog dlg = new RepositoryFolderBrowserDialog())
            {
                dlg.EnableNewFolderButton = true;
                Uri r;

                if (Uri.TryCreate(toUrlBox.Text, UriKind.Absolute, out r))
                    dlg.SelectedUri = r;

                if (dlg.ShowDialog(Context) == DialogResult.OK)
                {
                    if (dlg.SelectedUri != null)
                        toUrlBox.Text = dlg.SelectedUri.AbsoluteUri;
                }
            }
        }

        private void toUrlBox_TextAlignChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = (NewDirectoryName != null);
        }

        private void versionBrowse_Click(object sender, EventArgs e)
        {
            using (LogViewerDialog lvd = new LogViewerDialog(new SvnOrigin(Context, SrcUri, null), Context))
            {
                if (lvd.ShowDialog(Context) != DialogResult.OK)
                    return;

                ISvnLogItem li = EnumTools.GetSingle(lvd.SelectedItems);

                if (li != null)
                {
                    Revision = li.Revision;
                    specificVersionRadio.Checked = true;
                }
            }
        }
    }
}
