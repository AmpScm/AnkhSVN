using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SharpSvn;

namespace Ankh.UI.SccManagement
{
    public partial class AddToSubversion : VSContainerForm
    {
        public AddToSubversion()
        {
            InitializeComponent();
        }

        string _pathToAdd;
        public string PathToAdd
        {
            get { return _pathToAdd; }
            set { _pathToAdd = value; }
        }


        public string WorkingCopyDir
        {
            get { return (string)localFolder.SelectedItem; }
        }

        protected override void OnContextChanged(EventArgs e)
        {
            base.OnContextChanged(e);

            if (Context != null)
                treeView1.Context = Context;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string directory = File.Exists(PathToAdd) ? Path.GetDirectoryName(PathToAdd) : PathToAdd;
            string root = Path.GetPathRoot(directory);

            projectNameBox.Text = Path.GetFileNameWithoutExtension(PathToAdd);

            localFolder.Items.Add(directory);
            while (!root.Equals(directory, StringComparison.OrdinalIgnoreCase))
            {
                directory = Path.GetDirectoryName(directory);
                localFolder.Items.Add(directory);
            }

            GC.KeepAlive(PathToAdd);
        }

        SvnClient _client;
        SvnClient Client
        {
            get { return _client ?? (_client = Context.GetService<ISvnClientPool>().GetClient()); }
        }

        private void createFolderButton_Click(object sender, EventArgs e)
        {
            Uri u = treeView1.SelectedNode.RawUri;
            using (CreateDirectory dialog = new CreateDirectory())
            {
                if (dialog.ShowDialog(Context) == DialogResult.OK)
                { 
                    SvnCreateDirectoryArgs args = new SvnCreateDirectoryArgs();
                    args.MakeParents = true;
                    Uri newDir = new Uri(u, dialog.NewDirectoryName);
                    args.LogMessage = dialog.LogMessage;
                    Client.RemoteCreateDirectory(newDir, args);
                    treeView1.AddRoot(newDir);
                }
            }
        }

        string previousUrl;
        private void timer1_Tick(object sender, EventArgs e)
        {
            Uri u;
            if (previousUrl != repositoryUrl.Text)
            {
                previousUrl = repositoryUrl.Text;
            }
            else if (!string.IsNullOrEmpty(previousUrl) && Uri.TryCreate(previousUrl, UriKind.Absolute, out u))
            {
                timer1.Enabled = false;
                treeView1.AddRoot(u);
            }
        }

        private void repositoryUrl_TextUpdate(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        public Uri RepositoryAddUrl
        {
            get
            {
                if (treeView1.SelectedNode == null)
                    return null;

                Uri u = treeView1.SelectedNode.RawUri;
                if (!string.IsNullOrEmpty(projectNameBox.Text))
                    u = new Uri(u, projectNameBox.Text + "/");
                if (addTrunk.Checked)
                    u = new Uri(u, "trunk/");
                return u;
            }
        }

        void UpdateUrlPreview()
        {
            if (RepositoryAddUrl == null)
                textBox1.Text = "";
            else
                textBox1.Text = RepositoryAddUrl.ToString();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdateUrlPreview();
        }

        private void addTrunk_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUrlPreview();
        }

        private void projectNameBox_TextChanged(object sender, EventArgs e)
        {
            UpdateUrlPreview();
        }
    }
}
