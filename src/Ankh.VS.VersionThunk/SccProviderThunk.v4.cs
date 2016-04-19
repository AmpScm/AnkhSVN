using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Imaging.Interop;

namespace Ankh.Scc
{
    partial class SccProviderThunk : IVsSccChanges, IVsSccChangesDisplayInformation, IVsSccCurrentBranch, IVsSccCurrentBranchDisplayInformation, IVsSccCurrentRepository, IVsSccCurrentRepositoryDisplayInformation, IVsSccUnpublishedCommits, IVsSccUnpublishedCommitsDisplayInformation
    {
        string IVsSccCurrentBranchDisplayInformation.BranchDetail
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        ImageMoniker IVsSccCurrentBranchDisplayInformation.BranchIcon
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IVsSccCurrentBranchDisplayInformation.BranchName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        int IVsSccChangesDisplayInformation.PendingChangeCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IVsSccChangesDisplayInformation.PendingChangeDetail
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IVsSccChangesDisplayInformation.PendingChangeLabel
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IVsSccCurrentRepositoryDisplayInformation.RepositoryDetail
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        ImageMoniker IVsSccCurrentRepositoryDisplayInformation.RepositoryIcon
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IVsSccCurrentRepositoryDisplayInformation.RepositoryName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        int IVsSccUnpublishedCommitsDisplayInformation.UnpublishedCommitCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IVsSccUnpublishedCommitsDisplayInformation.UnpublishedCommitDetail
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IVsSccUnpublishedCommitsDisplayInformation.UnpublishedCommitLabel
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler IVsSccUnpublishedCommits.AdvertisePublish
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        System.Threading.Tasks.Task IVsSccCurrentBranch.BranchUIClickedAsync(ISccUIClickedEventArgs args, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        System.Threading.Tasks.Task IVsSccChanges.PendingChangesUIClickedAsync(ISccUIClickedEventArgs args, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        System.Threading.Tasks.Task IVsSccCurrentRepository.RepositoryUIClickedAsync(ISccUIClickedEventArgs args, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        System.Threading.Tasks.Task IVsSccUnpublishedCommits.UnpublishedCommitsUIClickedAsync(ISccUIClickedEventArgs args, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
