using System;
using System.Windows.Forms;
using Ankh.ExtensionPoints.RepositoryProvider;
using Ankh.UI.WizardFramework;

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

        internal void FillUsernamePassword(ScmUserNamePasswordEventArgs e)
        {
            if (this.repoProviderControl != null)
            {
                this.repoProviderControl.UserNamePasswordCallback(e);
                if (!e.Cancel && (string.IsNullOrEmpty(e.UserName) || string.IsNullOrEmpty(e.Password)))
                {
                    e.Cancel = true;
                }
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
                        Control c = GetControlFor(this.repoProviderControl);
                        if (c != null)
                        {
                            this.repoProviderControl.ScmRepositorySelectionControlEvent += new System.EventHandler<ScmRepositorySelectionControlEventArgs>(repoProviderControl_ScmRepositorySelectionControlEvent);
                            this.repoProviderControlPanel.Controls.Clear();
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
            bool isComplete = Uri != null;
            IsPageComplete = isComplete;
            WizardMessage msg = null;
            if (!isComplete)
            {
                string wMsgStr = e.Message;
                if (string.IsNullOrEmpty(wMsgStr) && e.Exception != null)
                {
                    wMsgStr = e.Exception.Message;
                }
                if (!string.IsNullOrEmpty(wMsgStr))
                {
                    msg = new WizardMessage(wMsgStr, WizardMessage.MessageType.Error);
                }
            }
            if (Message != msg) // for null
            {
                Message = msg;
                Wizard.UpdateMessage();
            }
        }

        private static Control GetControlFor(ScmRepositorySelectionControl providerControl)
        {
            Control result = null;
            if (providerControl != null)
            {
                IWin32Window window = providerControl.Window;
                if (window != null)
                {
                    IntPtr handle = window.Handle;
                    if (handle != null && handle != IntPtr.Zero)
                    {
                        result = Control.FromHandle(window.Handle);
                    }
                }
            }
            return result;
        }
    }
}
