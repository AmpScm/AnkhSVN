using System;
using System.Windows.Forms;
using Ankh.ExtensionPoints.RepositoryProvider;

namespace Ankh.UI.RepositoryExplorer.RepositoryWizard
{
    public partial class RepositorySelectionPage : WizardFramework.WizardPage
    {
        private ScmRepositoryProvider repoProvider;
        private ScmRepositorySelectionControl repoProviderControl;

        public RepositorySelectionPage()
        {
            InitializeComponent();
            IsPageComplete = false;
            Text = RepositoryWizardResources.RepoSelectionPageHeaderTitle;
            Description = RepositoryWizardResources.RepoSelectionPageHeaderMessage;
        }

        public RepositorySelectionPage(ScmRepositoryProvider repoProvider)
            : this()
        {
            this.repoProvider = repoProvider;
            if (this.repoProvider != null)
            {
                Name = string.Format("ProviderPage_{0}", this.repoProvider.Id);
            }
        }

        /// <summary>
        /// The URL entered in the text box.
        /// </summary>
        public Uri Uri
        {
            get
            {
                Uri uri = null;
                if (this.repoProviderControl != null)
                {
                    string uriString = this.repoProviderControl.SelectedUri;
                    if (string.IsNullOrEmpty(uriString) || !Uri.TryCreate(uriString, UriKind.Absolute, out uri))
                    {
                        return null;
                    }
                }
                return uri;
            }
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                if (this.repoProvider != null)
                {
                    this.repoProviderControl = repoProvider.RepositorySelectionControl;
                    if (this.repoProviderControl != null)
                    {
                        IWin32Window window = this.repoProviderControl.Window;
                        if (window is Control)
                        {
                            this.repoProviderControl.ScmRepositorySelectionControlEvent += new System.EventHandler<ScmRepositorySelectionControlEventArgs>(repoProviderControl_ScmRepositorySelectionControlEvent);
                            this.repoProviderControlPanel.Controls.Clear();
                            Control c = (Control)window;
                            c.Dock = DockStyle.Fill;
                            this.repoProviderControlPanel.Controls.Add(c);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                this.errorLabel.Text = exc.Message;
            }
        }

        private void repoProviderControl_ScmRepositorySelectionControlEvent(object sender, ScmRepositorySelectionControlEventArgs e)
        {
            EvaluagePage();
        }

        private void EvaluagePage()
        {
            IsPageComplete = true
                && this.repoProviderControl != null
                && Uri != null;
        }
    }
}
