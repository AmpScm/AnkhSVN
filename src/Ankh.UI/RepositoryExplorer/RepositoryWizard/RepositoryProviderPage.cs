using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Ankh.Configuration;
using Ankh.ExtensionPoints.RepositoryProvider;
using Ankh.UI.VSSelectionControls;

namespace Ankh.UI.RepositoryExplorer.RepositoryWizard
{
    public partial class RepositoryProviderPage : WizardFramework.WizardPage
    {
        public static readonly string EXTENSIONS_WIKI_URL = @"http://ankhsvn.open.collab.net/ankhsvn/extensions";

        public RepositoryProviderPage()
        {
            Text = RepositoryWizardResources.RepoProviderPageHeaderTitle;
            Description = RepositoryWizardResources.RepoProviderPageHeaderMessage;
            InitializeComponent();
            SmartColumn providerName = new SmartColumn(this.providerListView, "&Provider", 200, "RepositoryProviderName");
            this.providerListView.AllColumns.Add(providerName);
            this.providerListView.Columns.AddRange(
                new ColumnHeader[]
                {
                    providerName
                });
        }

        /// <summary>
        /// The URL entered in the text box.
        /// </summary>
        public Uri Uri
        {
            get
            {
                Uri uri = null;
                if (this.urlRadioButton.Checked)
                {
                    if (string.IsNullOrEmpty(urlComboBox.Text) || !Uri.TryCreate(urlComboBox.Text, UriKind.Absolute, out uri))
                        return null;
                }
                return uri;
            }
        }

        public ScmRepositoryProvider SelectedRepositoryProvider
        {
            get
            {
                if (this.providerRadioButton.Checked)
                {
                    if (this.providerListView.SelectedItems.Count == 1)
                    {
                        RepositoryProviderListViewItem lvi = this.providerListView.SelectedItems[0] as RepositoryProviderListViewItem;
                        if (lvi != null)
                        {
                            return lvi.RepositoryProvider;
                        }
                    }
                }
                return null;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Context != null)
            {
                foreach (string url in Context.GetService<IAnkhConfigurationService>().GetRecentReposUrls())
                {
                    this.urlComboBox.Items.Add(url);
                }
            }
            this.urlComboBox.Select();
            AnkhAction populateProviders = new AnkhAction(InitializeProviders);
            BeginInvoke(populateProviders);
        }

        private void InitializeProviders()
        {
            ICollection<ScmRepositoryProvider> providers = GetRepositoryProviders();
            if (InvokeRequired)
            {
                Action<ICollection<ScmRepositoryProvider>> initProviders = new Action<ICollection<ScmRepositoryProvider>>(InitializeProviders);
                Invoke(initProviders, providers);
            }
            else
            {
                InitializeProviders(providers);
            }
        }

        private void InitializeProviders(ICollection<ScmRepositoryProvider> providers)
        {
            Debug.Assert(!InvokeRequired);
            this.providerRadioButton.Enabled 
                = this.providerRadioButton.Visible 
                = this.providerListView.Enabled 
                = this.providerListView.Visible
                = providers != null && providers.Count > 0;
            // show link to the WIKI that lists the available third party repository providers
            // only if there are not providers installed
            this.wikiLinkLabel.Enabled = this.wikiLinkLabel.Visible = providers == null || providers.Count == 0;
            this.wikiLinkLabel1.Enabled = this.wikiLinkLabel1.Visible = providers != null && providers.Count > 0;
            if (providers != null && providers.Count > 0)
            {
                this.providerListView.BeginUpdate();
                try
                {
                    this.providerListView.Items.Clear();
                    List<RepositoryProviderListViewItem> items = new List<RepositoryProviderListViewItem>();

                    foreach (ScmRepositoryProvider i in providers)
                    {
                        RepositoryProviderListViewItem lvi = new RepositoryProviderListViewItem(this.providerListView, i);
                        items.Add(lvi);
                    }
                    this.providerListView.Items.AddRange(items.ToArray());
                }
                finally
                {
                    this.providerListView.EndUpdate();
                }
            }
            UpdateUI();
        }

        private ICollection<ScmRepositoryProvider> GetRepositoryProviders()
        {
            if (Wizard != null && Wizard.Context != null)
            {
                IAnkhRepositoryProviderService repoProviderSvc = Wizard.Context.GetService<IAnkhRepositoryProviderService>();
                if (repoProviderSvc != null)
                {
                    return repoProviderSvc.GetRepositoryProviders(RepositoryType.Subversion);
                }
            }
            return new ScmRepositoryProvider[] { };
        }

        private void EvaluatePage()
        {
            bool isPageComplete = false
                || Uri != null
                || SelectedRepositoryProvider != null;
            IsPageComplete = isPageComplete;
        }

        private void providerListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            EvaluatePage();
        }

        private void urlComboBox_TextChanged(object sender, EventArgs e)
        {
            EvaluatePage();
        }

        private void urlRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();
            if (this.urlRadioButton.Checked)
            {
                this.urlComboBox.Select();
            }
        }

        private void providerRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUI();
            if (this.providerRadioButton.Checked)
            {
                this.providerListView.Focus();
            }

        }

        private void UpdateUI()
        {
            this.urlComboBox.Enabled = this.urlRadioButton.Checked;
            this.providerListView.Enabled = this.providerRadioButton.Checked;
            EvaluatePage();
        }

        private void wikiLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Context != null)
            {
                Ankh.VS.IAnkhWebBrowser wb = Context.GetService<Ankh.VS.IAnkhWebBrowser>();
                if (wb != null)
                {
                    Ankh.VS.AnkhBrowserArgs args = new Ankh.VS.AnkhBrowserArgs();
                    args.External = true;
                    Uri cfSignUpUrl = new Uri(EXTENSIONS_WIKI_URL);
                    wb.Navigate(cfSignUpUrl, args);
                }
            }
        }
    }

    sealed class RepositoryProviderListViewItem : SmartListViewItem
    {
        public RepositoryProviderListViewItem(SmartListView view, ScmRepositoryProvider repoProvider)
            : base(view)
        {
            Tag = repoProvider;
            Refresh();
        }

        public ScmRepositoryProvider RepositoryProvider
        {
            get { return Tag as ScmRepositoryProvider; }
        }

        public string RepositoryProviderName
        {
            get { return RepositoryProvider == null ? string.Empty : RepositoryProvider.Name; }
        }

        void Refresh()
        {
            SetValues(RepositoryProviderName);
        }
    }
}
