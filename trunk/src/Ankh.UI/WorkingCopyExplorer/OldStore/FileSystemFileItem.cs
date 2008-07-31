using System;
using System.Text;
using Ankh.UI;
using Ankh.UI.WorkingCopyExplorer;

namespace Ankh.WorkingCopyExplorer
{
    internal class FileSystemFileItem : FileSystemItem
    {
        public FileSystemFileItem(WorkingCopyExplorerControl control, SvnItem item)
            : base(control, null, item)
        {

        }

        public override bool IsContainer
        {
            get { return false; }
        }

        public override IFileSystemItem[] GetChildren()
        {
            return new IFileSystemItem[] { };
        }

        public override void Refresh(bool rescan)
        {
            if (rescan)
            {
                this.SvnItem.MarkDirty();
            }
            this.CurrentStatus = MergeStatuses(this.CheckChildStatuses(), this.ThisNodeStatus());
        }
    }
}
