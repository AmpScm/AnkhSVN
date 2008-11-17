using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI.VSSelectionControls;
using SharpSvn;
using Ankh.VS;
using Ankh.Scc;

namespace Ankh.UI.PendingChanges.Synchronize
{
    class SynchronizeListItem : SmartListViewItem
    {
        SvnItem _item;
        SvnStatusEventArgs _status;
        PendingChangeKind _localChange;
        PendingChangeKind _remoteChange;
        PendingChangeStatus _localStatus;
        PendingChangeStatus _remoteStatus;

        public SynchronizeListItem(SynchronizeListView list, SvnItem item, SvnStatusEventArgs status)
            : base(list)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _item = item;
            _status = status;

            _localChange = PendingChange.CombineStatus(status.LocalContentStatus, status.LocalPropertyStatus, false, item);
            _remoteChange = PendingChange.CombineStatus(status.RemoteContentStatus, status.RemotePropertyStatus, false, null);

            _localStatus = new PendingChangeStatus(_localChange);
            _remoteStatus = new PendingChangeStatus(_remoteChange);

            UpdateText();
        }

        IAnkhServiceProvider Context
        {
            get { return ((SynchronizeListView)ListView).Context; }
        }

        private void UpdateText()
        {
            IFileIconMapper mapper = Context.GetService<IFileIconMapper>();

            ImageIndex = GetIcon(mapper);

            StateImageIndex = mapper.GetStateIcon(GetIcon(_status));

            SetValues(
                _item.Status.ChangeList,
                _item.Directory,
                _item.FullPath,
                _localStatus.PendingCommitText,
                (_status.RemoteLock != null) ? PCStrings.LockedValue : "", // Locked
                SafeDate(_item.Modified),
                _item.Name,
                GetRelativePath(_item),
                GetProject(_item),
                _remoteStatus.PendingCommitText,
                _item.Extension,
                SafeWorkingCopy(_item));
        }

        private int GetIcon(IFileIconMapper mapper)
        {
            if (SvnItem.Exists)
                return mapper.GetIcon(_item.FullPath);
            else if (_status.NodeKind == SvnNodeKind.Directory)
                return mapper.DirectoryIcon;
            else if (_status.NodeKind == SvnNodeKind.None && _status.RemoteUpdateNodeKind == SvnNodeKind.Directory)
                return mapper.DirectoryIcon;
            else
                return mapper.GetIconForExtension(_item.Extension);
        }

        private StateIcon GetIcon(SvnStatusEventArgs status)
        {
            // TODO: Handle more special cases
            SvnStatus st = status.LocalContentStatus;

            bool localModified = IsMod(status.LocalContentStatus) || IsMod(status.LocalPropertyStatus);
            bool remoteModified = IsMod(status.RemoteContentStatus) || IsMod(status.RemotePropertyStatus);

            if (localModified && remoteModified)
                return StateIcon.Collision;
            else if (localModified)
                return StateIcon.Outgoing;
            else if (remoteModified)
                return StateIcon.Incoming;
            else
                return StateIcon.Blank;
        }

        private bool IsMod(SvnStatus svnStatus)
        {
            switch (svnStatus)
            {
                case SvnStatus.None:
                case SvnStatus.Normal:
                    return false;
                default:
                    return true;
            }
        }

        static string SafeWorkingCopy(SvnItem item)
        {
            if (item != null && item.WorkingCopy != null)
                return item.WorkingCopy.FullPath;

            return "";
        }

        static string GetProject(SvnItem _item)
        {
            return "";
        }

        string GetRelativePath(SvnItem item)
        {
            IAnkhSolutionSettings ss = Context.GetService<IAnkhSolutionSettings>();

            string path = ss.ProjectRootWithSeparator;

            if (item.FullPath.StartsWith(path, StringComparison.OrdinalIgnoreCase))
                return item.FullPath.Substring(path.Length);

            return item.FullPath;
        }

        static string SafeDate(DateTime dateTime)
        {
            if (dateTime.Ticks == 0 || dateTime.Ticks == 1)
                return "";

            DateTime n = dateTime.ToLocalTime();

            if (n < DateTime.Now - new TimeSpan(24, 0, 0))
                return n.ToString("d");
            else
                return n.ToString("T");
        }

        static string CombineChange(SvnStatus svnStatus, SvnStatus svnStatus_2)
        {
            return svnStatus.ToString() + " " + svnStatus_2.ToString();
        }

        public SvnItem SvnItem
        {
            get { return _item; }
        }
    }
}
