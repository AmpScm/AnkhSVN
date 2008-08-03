using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.Scc;
using Ankh.VS;
using Ankh.UI.VSSelectionControls;

namespace Ankh.UI.PendingChanges
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

            RefreshText(view.Context);
        }

        public void RefreshText(IAnkhServiceProvider context)
        {
            IAnkhSolutionSettings solSet = context.GetService<IAnkhSolutionSettings>();
            IFileStatusCache cache = context.GetService<IFileStatusCache>();

            Text = PendingChange.RelativePath;
            string start = solSet.ProjectRootWithSeparator;

            ImageIndex = PendingChange.IconIndex;
            SvnItem item = cache[FullPath];

            if (item == null)
                throw new InvalidOperationException(); // Item no longer valued

            PendingChangeStatus pcs = PendingChange.Change;

            SetValues(
                pcs.PendingCommitText,
                PendingChange.ChangeList,
                PendingChange.Item.Directory,
                PendingChange.FullPath,
                "", // Locked
                "", // Modified
                PendingChange.Name,
                PendingChange.RelativePath,
                PendingChange.Project,
                System.IO.Path.GetExtension(PendingChange.FullPath),
                SafeWorkingCopy(PendingChange.Item));

            ImageIndex = PendingChange.IconIndex;

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
