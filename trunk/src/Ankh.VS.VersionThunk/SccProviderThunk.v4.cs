﻿using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Imaging.Interop;

namespace Ankh.Scc
{
    partial class SccProviderThunk : IVsSccChanges, IVsSccChangesDisplayInformation, IVsSccCurrentBranch, IVsSccCurrentBranchDisplayInformation, IVsSccCurrentRepository, IVsSccCurrentRepositoryDisplayInformation, IVsSccUnpublishedCommits, IVsSccUnpublishedCommitsDisplayInformation, IVsSccPublish, IVsSccSolution
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

        public event EventHandler AddedToSourceControl;

        System.Threading.Tasks.Task IVsSccPublish.BeginPublishWorkflowAsync(CancellationToken cancellationToken)
        {
            System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(OnPublishWorkflow, cancellationToken);

            t.Start();
            return t;
        }

        System.Drawing.Point GetPoint(ISccUIClickedEventArgs args)
        {
            System.Windows.Point p = args.ClickedElementPosition.TopRight;
            return new System.Drawing.Point((int)p.X, (int)p.Y);
        }

        System.Threading.Tasks.Task IVsSccCurrentBranch.BranchUIClickedAsync(ISccUIClickedEventArgs args, CancellationToken cancellationToken)
        {
            System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(delegate { OnBranchUIClicked(GetPoint(args)); }, cancellationToken);

            t.Start();
            return t;
        }

        System.Threading.Tasks.Task IVsSccChanges.PendingChangesUIClickedAsync(ISccUIClickedEventArgs args, CancellationToken cancellationToken)
        {
            System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(delegate { OnPendingChangesClicked(GetPoint(args)); }, cancellationToken);

            t.Start();
            return t;
        }

        System.Threading.Tasks.Task IVsSccCurrentRepository.RepositoryUIClickedAsync(ISccUIClickedEventArgs args, CancellationToken cancellationToken)
        {
            System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(delegate { OnRepositoryUIClicked(GetPoint(args)); }, cancellationToken);

            t.Start();
            return t;
        }

        System.Threading.Tasks.Task IVsSccUnpublishedCommits.UnpublishedCommitsUIClickedAsync(ISccUIClickedEventArgs args, CancellationToken cancellationToken)
        {
            System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(delegate { OnUnpublishedCommitsUIClickedAsync(GetPoint(args)); });

            t.Start();
            return t;
        }
    }
}
