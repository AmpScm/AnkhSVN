using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Imaging.Interop;
using System.Drawing;

namespace Ankh.Scc
{
    partial class SccProviderThunk : IVsSccChanges, IVsSccChangesDisplayInformation, IVsSccCurrentBranch, IVsSccCurrentBranchDisplayInformation, IVsSccCurrentRepository, IVsSccCurrentRepositoryDisplayInformation, IVsSccUnpublishedCommits, IVsSccUnpublishedCommitsDisplayInformation, IVsSccPublish
    {
        string IVsSccCurrentBranchDisplayInformation.BranchDetail
        {
            get
            {
                return "Detail";
            }
        }

        ImageMoniker IVsSccCurrentBranchDisplayInformation.BranchIcon
        {
            get
            {
                return new ImageMoniker()
                {
                    Guid = new Guid("{ae27a6b0-e345-4288-96df-5eaf394ee369}"),
                    Id = 3668
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
                return 2;
            }
        }

        string IVsSccChangesDisplayInformation.PendingChangeDetail
        {
            get
            {
                return "That's 'two'";
            }
        }

        string IVsSccChangesDisplayInformation.PendingChangeLabel
        {
            get
            {
                return "PC Label";
            }
        }

        string IVsSccCurrentRepositoryDisplayInformation.RepositoryDetail
        {
            get
            {
                return "REP detail";
            }
        }

        ImageMoniker IVsSccCurrentRepositoryDisplayInformation.RepositoryIcon
        {
            get
            {
                return new ImageMoniker()
                {
                    Guid = new Guid("{ae27a6b0-e345-4288-96df-5eaf394ee369}"),
                    Id = 3668
                };
            }
        }

        string IVsSccCurrentRepositoryDisplayInformation.RepositoryName
        {
            get
            {
                return "REP name";
            }
        }

        int IVsSccUnpublishedCommitsDisplayInformation.UnpublishedCommitCount
        {
            get
            {
                return 7;
            }
        }

        string IVsSccUnpublishedCommitsDisplayInformation.UnpublishedCommitDetail
        {
            get
            {
                return "Seven";
            }
        }

        string IVsSccUnpublishedCommitsDisplayInformation.UnpublishedCommitLabel
        {
            get
            {
                return "Unpublished L";
            }
        }

        event EventHandler IVsSccUnpublishedCommits.AdvertisePublish
        {
            add { AdvertisePublish += value; }
            remove { AdvertisePublish -= value; }
        }

        System.Threading.Tasks.Task IVsSccPublish.BeginPublishWorkflowAsync(CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
            return new System.Threading.Tasks.Task(OnPublishWorkflow, cancellationToken);
        }

        System.Drawing.Rectangle GetRect(ISccUIClickedEventArgs args)
        {
            System.Windows.Rect r = args.ClickedElementPosition;
            return new System.Drawing.Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
        }

        System.Threading.Tasks.Task IVsSccCurrentBranch.BranchUIClickedAsync(ISccUIClickedEventArgs args, CancellationToken cancellationToken)
        {
            return new System.Threading.Tasks.Task(() => OnBranchUIClicked(GetRect(args)), cancellationToken);
        }

        System.Threading.Tasks.Task IVsSccChanges.PendingChangesUIClickedAsync(ISccUIClickedEventArgs args, CancellationToken cancellationToken)
        {
            return new System.Threading.Tasks.Task(() => OnPendingChangesClicked(GetRect(args)), cancellationToken);
        }

        System.Threading.Tasks.Task IVsSccCurrentRepository.RepositoryUIClickedAsync(ISccUIClickedEventArgs args, CancellationToken cancellationToken)
        {
            return new System.Threading.Tasks.Task(() => OnRepositoryUIClicked(GetRect(args)), cancellationToken);
        }

        System.Threading.Tasks.Task IVsSccUnpublishedCommits.UnpublishedCommitsUIClickedAsync(ISccUIClickedEventArgs args, CancellationToken cancellationToken)
        {
            return new System.Threading.Tasks.Task(() => OnUnpublishedCommitsUIClickedAsync(GetRect(args)), cancellationToken);
        }
    }
}
