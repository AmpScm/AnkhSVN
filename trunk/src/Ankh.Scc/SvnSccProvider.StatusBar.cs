using System;
using System.Collections.Generic;
using Ankh.VS;
using System.Drawing;
using Ankh.Commands;

namespace Ankh.Scc
{
    partial class SvnSccProvider
    {
        static string StripComponents(string info, int len)
        {
            bool broken = false;

            while(info.Length > len)
            {
                int n = info.IndexOf('/');

                if (n <= 0)
                    break;

                info = info.Substring(n + 1);
                broken = true;
            }

            return broken ? ".." + info : info;
        }
        #region Branch

        protected override void OnBranchUIClicked(Point clickedElement)
        {
            GetService<IAnkhCommandService>().PostExecCommand(AnkhCommand.SolutionSwitchDialog);
        }

        protected override string BranchName
        {
            get
            {
                SvnItem item = GetService<IAnkhSolutionSettings>().ProjectRootSvnItem;

                if (item == null || item.Uri == null)
                    return "";

                return StripComponents("^/" + item.WorkingCopy.RepositoryRoot.MakeRelativeUri(item.Uri).ToString().TrimEnd('/'), 20);
            }
        }
        protected override string BranchDetail
        {
            get
            {
                SvnItem item = GetService<IAnkhSolutionSettings>().ProjectRootSvnItem;

                if (item == null || item.Uri == null)
                    return "";

                return "^/" + item.WorkingCopy.RepositoryRoot.MakeRelativeUri(item.Uri).ToString();
            }
        }

        protected override ImageReference BranchIcon
        {
            get
            {
                return base.BranchIcon;
            }
        }
        #endregion

        #region PendingChange

        protected override void OnPendingChangesClicked(Point clickedElement)
        {
            GetService<IAnkhCommandService>().PostExecCommand(AnkhCommand.ShowPendingChanges);
        }

        bool _trackPendingChanges;
        int _lastPcCount;

        void TrackPendingChanges()
        {
            IPendingChangesManager pcm = GetService<IPendingChangesManager>();
            if (pcm == null)
                return;

            _trackPendingChanges = true;
            pcm.BatchUpdateStarted += SvnSccProvider_BatchUpdateStarted;
            pcm.PendingChanges.CollectionChanged += PendingChanges_CollectionChanged;

        }

        private void PendingChanges_CollectionChanged(object sender, Collections.CollectionChangedEventArgs<PendingChange> e)
        {
            RaisePropertyChanged("PendingChangeCount");
        }

        private void SvnSccProvider_BatchUpdateStarted(object sender, BatchStartedEventArgs e)
        {
            e.Disposers += delegate
            {
                int n = GetService<IPendingChangesManager>().PendingChanges.Count;

                if (n != _lastPcCount)
                {
                    _lastPcCount = n;
                    RaisePropertyChanged("PendingChangeCount");
                }
            };
        }

        protected override int PendingChangeCount
        {
            get
            {
                if (!_trackPendingChanges)
                    TrackPendingChanges();
                return GetService<IPendingChangesManager>().PendingChanges.Count;
            }
        }

        protected override string PendingChangeLabel
        {
            get
            {
                return "Pending Changes";
            }
        }

        protected override string PendingChangeDetail
        {
            get
            {
                return "Open Pending Changes Window";
            }
        }
        #endregion

        #region Repository

        protected override void OnRepositoryUIClicked(Point clickedElement)
        {
            GetService<IAnkhCommandService>().PostExecCommand(AnkhCommand.SolutionSwitchDialog);
        }

        protected override string RepositoryName
        {
            get
            {
                SvnItem item = GetService<IAnkhSolutionSettings>().ProjectRootSvnItem;

                if (item == null || item.Uri == null)
                    return "";

                return item.WorkingCopy.RepositoryRoot.PathAndQuery.TrimEnd('/').TrimStart('/');
            }
        }

        protected override string RepositoryDetail
        {
            get
            {
                SvnItem item = GetService<IAnkhSolutionSettings>().ProjectRootSvnItem;

                if (item == null || item.Uri == null)
                    return "";

                return item.WorkingCopy.RepositoryRoot.ToString();
            }
        }

        protected override ImageReference RepositoryIcon
        {
            get
            {
                return base.RepositoryIcon;
            }
        }
#endregion
    }
}
