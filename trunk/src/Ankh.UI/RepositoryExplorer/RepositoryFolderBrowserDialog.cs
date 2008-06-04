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
        public bool EnableNewFolder
        {
            get { return newFolderButton.Enabled; }
            set { newFolderButton.Visible = newFolderButton.Enabled = value; }
        }
    }
}
