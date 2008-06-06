using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.RepositoryExplorer
{
    public partial class RepositoryFolderBrowserDialog : VSContainerForm
    {
        public RepositoryFolderBrowserDialog()
        {
            InitializeComponent();
        }

        protected override void OnContextChanged(EventArgs e)
        {
            base.OnContextChanged(e);
            reposBrowser.Context = Context;
        }

        public Uri SelectedUri
        {
            get
            {
                Uri uri;
                if (!string.IsNullOrEmpty(urlBox.Text) && Uri.TryCreate(urlBox.Text, UriKind.Absolute, out uri))
                    return uri;

                return null;
            }

            set
            {
                if (value != null && value.IsAbsoluteUri)
                    urlBox.Text = value.AbsoluteUri;
                else
                    urlBox.Text = "";
            }
        }

        bool _initialized;

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (DesignMode)
                return;
            if (!_initialized && IsHandleCreated && Visible)
            {
                if (SelectedUri != null)
                {
                    _initialized = false;
                    BrowseText();
                }
            }
        }

        Uri _rootUri;
        /// <summary>
        /// Gets or sets the top level uri to browse below; Currently only used when initializing the dialog
        /// </summary>
        [DefaultValue(null)]
        public Uri RootUri
        {
            get { return _rootUri; }
            set { _rootUri = value; }
        }

        [DefaultValue(false)]
        public bool EnableNewFolderButton
        {
            get { return newFolderButton.Enabled; }
            set { newFolderButton.Visible = newFolderButton.Enabled = value; }
        }

        private void urlBox_Leave(object sender, EventArgs e)
        {
            BrowseText();
        }

        private void newFolderButton_Click(object sender, EventArgs e)
        {

        }

        private void urlBox_TextChanged(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            BrowseText();
        }

        private void urlBox_TextUpdate(object sender, EventArgs e)
        {
            BrowseText();
        }

        void BrowseText()
        {
            timer.Stop();

            Uri uri = SelectedUri;

            if(uri != null)
                reposBrowser.BrowseItem(uri);
        }

        private void reposBrowser_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RepositoryTreeNode tn = reposBrowser.SelectedNode;

            if(tn != null && tn.RawUri != null)
            {
                urlBox.Text = tn.RawUri.AbsoluteUri;
                timer.Stop();
            }
        }
    }
}
