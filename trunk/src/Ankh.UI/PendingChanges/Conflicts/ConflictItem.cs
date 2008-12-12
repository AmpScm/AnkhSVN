using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI.VSSelectionControls;
using Ankh.Scc;

namespace Ankh.UI.PendingChanges.Conflicts
{
    class ConflictListItem : SmartListViewItem
    {
        readonly PendingChange _change;
        public ConflictListItem(ConflictListView view, PendingChange change)
            : base(view)
        {
            if (change == null)
                throw new ArgumentNullException("change");

            _change = change;
            _change = change;
            RefreshText(view.Context);
        }

        internal PendingChange PendingChange
        {
            get { return _change; }
        }

        public void RefreshText(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            IFileStatusCache cache = context.GetService<IFileStatusCache>();

            ImageIndex = PendingChange.IconIndex;
            SvnItem item = cache[FullPath];

            if (item == null)
                throw new InvalidOperationException(); // Item no longer valued
            PendingChangeStatus pcs = PendingChange.Change ?? new PendingChangeStatus(PendingChangeKind.None);

            SetValues(
                pcs.PendingCommitText,
                PendingChange.ChangeList,
                GetDirectory(item),
                PendingChange.FullPath,
                item.IsLocked ? PCStrings.LockedValue : "", // Locked
                SafeDate(item.Modified), // Modified
                PendingChange.Name,
                PendingChange.RelativePath,
                PendingChange.Project,
                item.Extension,
                SafeWorkingCopy(item));
        }

        private string SafeDate(DateTime dateTime)
        {
            if (dateTime.Ticks == 0 || dateTime.Ticks == 1)
                return "";

            DateTime n = dateTime.ToLocalTime();

            if (n < DateTime.Now - new TimeSpan(24, 0, 0))
                return n.ToString("d");
            else
                return n.ToString("T");
        }

        private string GetDirectory(SvnItem svnItem)
        {
            if (svnItem.IsDirectory)
                return svnItem.FullPath;
            else
                return svnItem.Directory;
        }

        static string SafeWorkingCopy(SvnItem svnItem)
        {
            SvnWorkingCopy wc = svnItem.WorkingCopy;
            if (wc == null)
                return "";

            return wc.FullPath;
        }

        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <value>The full path.</value>
        public string FullPath
        {
            get { return _change.FullPath; }
        }
    }
}
