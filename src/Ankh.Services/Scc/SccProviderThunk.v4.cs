using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Imaging.Interop;
using Task = System.Threading.Tasks.Task;
using Ankh.VS;

namespace Ankh.Scc
{
    partial class SccProviderThunk : IVsSccChanges, IVsSccChangesDisplayInformation, IVsSccCurrentBranch, IVsSccCurrentBranchDisplayInformation, IVsSccCurrentRepository, IVsSccCurrentRepositoryDisplayInformation/*, IVsSccUnpublishedCommits, IVsSccUnpublishedCommitsDisplayInformation*/, IVsSccPublish, IVsSccSolution
    {
        string IVsSccCurrentBranchDisplayInformation.BranchDetail
        {
            get
            {
                return BranchDetail;
            }
        }

        ImageMoniker IVsSccCurrentBranchDisplayInformation.BranchIcon
        {
            get
            {
                ImageReference r = BranchIcon;
                return new ImageMoniker()
                {
                    Guid = r.Guid,
                    Id = r.Id
                };
            }
        }

        string IVsSccCurrentBranchDisplayInformation.BranchName
        {
            get
            {
                return BranchName;
            }
        }

        int IVsSccChangesDisplayInformation.PendingChangeCount
        {
            get
            {
                return PendingChangeCount;
            }
        }

        string IVsSccChangesDisplayInformation.PendingChangeDetail
        {
            get
            {
                return PendingChangeDetail;
            }
        }

        string IVsSccChangesDisplayInformation.PendingChangeLabel
        {
            get
            {
                return PendingChangeLabel;
            }
        }

        string IVsSccCurrentRepositoryDisplayInformation.RepositoryDetail
        {
            get { return RepositoryDetail; }
        }

        ImageMoniker IVsSccCurrentRepositoryDisplayInformation.RepositoryIcon
        {
            get
            {
                ImageReference r = RepositoryIcon;
                return new ImageMoniker()
                {
                    Guid = r.Guid,
                    Id = r.Id
                };
            }
        }

        string IVsSccCurrentRepositoryDisplayInformation.RepositoryName
        {
            get
            {
                return RepositoryName;
            }
        }

        /*int IVsSccUnpublishedCommitsDisplayInformation.UnpublishedCommitCount
        {
            get
            {
                return UnpublishedCommitCount;
            }
        }

        string IVsSccUnpublishedCommitsDisplayInformation.UnpublishedCommitDetail
        {
            get
            {
                return UnpublishedCommitDetail;
            }
        }

        string IVsSccUnpublishedCommitsDisplayInformation.UnpublishedCommitLabel
        {
            get
            {
                return UnpublishedCommitLabel;
            }
        }

        event EventHandler IVsSccUnpublishedCommits.AdvertisePublish
        {
            add { AdvertisePublish += value; }
            remove { AdvertisePublish -= value; }
        }*/

        // IVsSccSolution requires this event, but Subversion has no separate
        // "solution added" notification to raise here. Keep the interface contract
        // without turning a required compatibility event into global warning noise.
#pragma warning disable CS0067
        public event EventHandler AddedToSourceControl;
#pragma warning restore CS0067

        protected async Task RunOnMainThreadAsync(
            SccAction action,
            CancellationToken cancellationToken)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            cancellationToken.ThrowIfCancellationRequested();
            await ThreadHelper.JoinableTaskFactory
                .SwitchToMainThreadAsync(cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();
            action();
        }

        System.Threading.Tasks.Task IVsSccPublish.BeginPublishWorkflowAsync(
            CancellationToken cancellationToken)
        {
            return RunOnMainThreadAsync(
                OnPublishWorkflow,
                cancellationToken);
        }

        System.Drawing.Point GetPoint(ISccUIClickedEventArgs args)
        {
            System.Windows.Point p = args.ClickedElementPosition.TopRight;
            return new System.Drawing.Point((int)p.X, (int)p.Y);
        }

        System.Threading.Tasks.Task IVsSccCurrentBranch.BranchUIClickedAsync(ISccUIClickedEventArgs args, CancellationToken cancellationToken)
        {
            return RunOnMainThreadAsync(
                delegate { OnBranchUIClicked(GetPoint(args)); },
                cancellationToken);
        }

        System.Threading.Tasks.Task IVsSccChanges.PendingChangesUIClickedAsync(ISccUIClickedEventArgs args, CancellationToken cancellationToken)
        {
            return RunOnMainThreadAsync(
                delegate { OnPendingChangesClicked(GetPoint(args)); },
                cancellationToken);
        }

        System.Threading.Tasks.Task IVsSccCurrentRepository.RepositoryUIClickedAsync(ISccUIClickedEventArgs args, CancellationToken cancellationToken)
        {
            return RunOnMainThreadAsync(
                delegate { OnRepositoryUIClicked(GetPoint(args)); },
                cancellationToken);
        }

        //System.Threading.Tasks.Task IVsSccUnpublishedCommits.UnpublishedCommitsUIClickedAsync(ISccUIClickedEventArgs args, CancellationToken cancellationToken)
        //{
        //    return RunAsyncOnMainThread(delegate { OnUnpublishedCommitsUIClickedAsync(GetPoint(args)); });
        //}
    }
}
