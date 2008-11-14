using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI.VSSelectionControls;
using SharpSvn;
using Ankh.VS;

namespace Ankh.UI.PendingChanges.Synchronize
{
    class SynchronizeListItem : SmartListViewItem
    {
        SvnItem _item;
        SvnStatusEventArgs _status;

        public SynchronizeListItem(SynchronizeListView list, SvnItem item, SvnStatusEventArgs status)
            : base(list)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _item = item;
            _status = status;

            UpdateText();
        }

        IAnkhServiceProvider Context
        {
            get { return ((SynchronizeListView)ListView).Context; }
        }

        private void UpdateText()
        {
            IFileIconMapper mapper = Context.GetService<IFileIconMapper>();

            ImageIndex = mapper.GetIcon(_item.FullPath);
            SetValues(
                _item.Status.ChangeList,
                _item.Directory,
                _item.FullPath,
                CombineChange(_status.LocalContentStatus, _status.LocalPropertyStatus),
                (_status.RemoteLock != null) ? PCStrings.LockedValue : "", // Locked
                SafeDate(_item.Modified),
                _item.Name,
                GetRelativePath(_item),
                GetProject(_item),
                CombineChange(_status.RemoteContentStatus, _status.RemotePropertyStatus),
                _item.Extension,
                SafeWorkingCopy(_item));
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
