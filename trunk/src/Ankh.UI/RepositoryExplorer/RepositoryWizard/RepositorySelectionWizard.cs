using System;
using System.Collections.Generic;
using Ankh.ExtensionPoints.RepositoryProvider;
using Ankh.UI.WizardFramework;
using SharpSvn;
using SharpSvn.Remote;

namespace Ankh.UI.RepositoryExplorer.RepositoryWizard
{
    public partial class RepositorySelectionWizard : WizardFramework.Wizard
    {
        private RepositoryProviderPage providerSelectionPage;
        private IDictionary<string, RepositorySelectionPage> repositorySelectionPages;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="utils"></param>
        public RepositorySelectionWizard(IAnkhServiceProvider context)
            : base()
        {
            InitializeComponent();
            Context = context;
            DefaultPageImage = RepositoryWizardResources.RepositoryWizardHeaderImage;
            Text = RepositoryWizardResources.Title;

            this.providerSelectionPage = new RepositoryProviderPage();
        }

        private Uri resultUri = null;
        public Uri GetSelectedRepositoryUri()
        {
            return this.resultUri;
        }

        public override void AddPages()
        {
            Pages.Add(this.providerSelectionPage);
            // the pages for repository providers are added lazily in 'GetNextPage'
        }

        public override bool NextIsFinish
        {
            get
            {
                if (CurrentPage == this.providerSelectionPage)
                {
                    return this.providerSelectionPage.Uri != null;
                }
                return base.NextIsFinish;
            }
        }

        public override WizardPage GetNextPage(WizardPage page)
        {
            if (page == this.providerSelectionPage)
            {
                ScmRepositoryProvider repoProvider = this.providerSelectionPage.SelectedRepositoryProvider;
                if (repoProvider != null)
                {
                    if (this.repositorySelectionPages == null)
                    {
                        this.repositorySelectionPages = new Dictionary<string, RepositorySelectionPage>();
                    }
                    string providerId = repoProvider.Id;
                    RepositorySelectionPage result = null;
                    if (!this.repositorySelectionPages.TryGetValue(providerId, out result))
                    {
                        result = new RepositorySelectionPage(repoProvider);
                        Pages.Add(result);
                        this.repositorySelectionPages.Add(providerId, result);
                    }
                    return result;
                }
            }
            return null;
        }

        public override void OnFinish(System.ComponentModel.CancelEventArgs e)
        {
            WizardMessage newMsg = null;
            this.resultUri = null;
            EnablePageAndButtons(false);
            try
            {
                Uri uri = this.providerSelectionPage.Uri;
                if (uri == null && this.repositorySelectionPages != null)
                {
                    ScmRepositoryProvider repoProvider = this.providerSelectionPage.SelectedRepositoryProvider;
                    if (repoProvider != null)
                    {
                        RepositorySelectionPage repoSelectionPage;
                        if (this.repositorySelectionPages.TryGetValue(repoProvider.Id, out repoSelectionPage))
                        {
                            uri = repoSelectionPage.Uri;
                        }
                    }
                }
                bool cancel = false;
                if (uri == null)
                {
                    cancel = true;
                }
                else
                {
                    Exception exc = null;
                    // TODO (BA) Should we validate here?
                    /*
                    try
                    {
                        ProgressRunnerArgs runnerArgs = new ProgressRunnerArgs();
                        runnerArgs.CreateLog = false;
                        // Check the validity of the specified repository location using IProgressRunner
                        ProgressRunnerResult result = Context.GetService<IProgressRunner>().RunModal("Checking URI",
                            runnerArgs,
                            delegate(object sender, ProgressWorkerArgs ee)
                            { CheckResult(uri); });
                        cancel = !(result.Succeeded || result.Exception == null);
                        exc = result.Exception;
                    }
                    catch (Exception runnere)
                    {
                        cancel = true; 
                        exc = runnere.InnerException == null ? runnere : runnere.InnerException;
                    }
                    */
                    if (cancel && exc != null)
                    {
                        newMsg = new WizardMessage(exc.Message, WizardMessage.MessageType.Error);
                    }
                }
                this.resultUri = cancel ? null : uri;
                DialogResult = cancel ? System.Windows.Forms.DialogResult.None : System.Windows.Forms.DialogResult.OK;
                if (CurrentPage.Message != newMsg)
                {
                    CurrentPage.Message = newMsg;
                    UpdateMessage();
                }
                e.Cancel = cancel;
            }
            finally
            {
                EnablePageAndButtons(true);
            }
        }

        private void CheckResult(Uri combined)
        {
            using (SvnPoolRemoteSession session = GetSession(combined))
            {
                SvnRemoteNodeKindArgs nka = new SvnRemoteNodeKindArgs();
                nka.ThrowOnError = true;

                SvnNodeKind kind;

                string path = session.MakeRelativePath(combined);

                if (session.GetNodeKind(path, nka, out kind))
                {
                    switch (kind)
                    {
                        case SvnNodeKind.Directory:
                            {
                                Uri parentUri = new Uri(combined, combined.PathAndQuery.EndsWith("/", StringComparison.Ordinal) ? "../" : "./");
                                return;
                            }
                        case SvnNodeKind.File:
                            {
                                SvnRemoteCommonArgs ca = new SvnRemoteCommonArgs();
                                ca.ThrowOnError = true;

                                Uri parentUri = new Uri(combined, "./");
                                Uri reposRoot;
                                if (!session.GetRepositoryRoot(ca, out reposRoot))
                                    return;
                                return;
                            }
                    }
                }
            }
        }

        private SvnPoolRemoteSession GetSession(Uri uri)
        {
            ISvnClientPool pool = (Context != null) ? Context.GetService<ISvnClientPool>() : null;

            if (pool != null)
            {
                return pool.GetRemoteSession(uri, true);
            }

            throw new InvalidOperationException();
        }
    }
}
