using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.Scc;
using Ankh.VS;
using Ankh.UI.VSSelectionControls;

namespace Ankh.UI.PendingChanges.Commits
{
    class PendingCommitItem : SmartListViewItem
    {
        readonly PendingChange _change;

        public PendingCommitItem(PendingCommitsView view, PendingChange change)
            : base(view)
        {
            if (change == null)
                throw new ArgumentNullException("change");

            _change = change;        

            Checked = true;

            RefreshText(view.Context);
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

            System.Drawing.Color clr = System.Drawing.Color.Black;

            if(item.IsConflicted)
                clr = System.Drawing.Color.Red;
            else if(item.IsDeleteScheduled)
                clr = System.Drawing.Color.DarkRed;
            else if(item.Status.IsCopied || item.Status.CombinedStatus == SharpSvn.SvnStatus.Added || !item.IsVersioned)
                clr = System.Drawing.Color.FromArgb(100,0,100);
            else if(item.IsModified)
                clr = System.Drawing.Color.DarkBlue;

            ForeColor = clr;
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
        public PendingChange PendingChange
        {
            get { return _change; }
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
