using System;
using System.Text;
using System.Collections;
using Ankh.UI;
using SharpSvn;
using Ankh.Scc;
using System.Collections.Generic;
using Ankh.UI.WorkingCopyExplorer;
using System.IO;

namespace Ankh.WorkingCopyExplorer
{
    internal class FileSystemDirectoryItem : FileSystemItem
    {
        public FileSystemDirectoryItem(WorkingCopyExplorerControl control, SvnItem item)
            : base(control, null, item)
        {
            this.FindChildren();
            this.Refresh(false);
        }

        public override bool IsContainer
        {
            get { return true; }
        }

        public override Ankh.UI.IFileSystemItem[] GetChildren()
        {
            try
            {
                IFileSystemItem[] children = new IFileSystemItem[this.Children.Count];
                this.Children.CopyTo(children, 0);
                return children;
            }
            catch
            {
                return new IFileSystemItem[] { };
            }
        }


        public override void Refresh(bool rescan)
        {
            if (rescan)
            {
                IFileStatusCache cache = Context.GetService<IFileStatusCache>();

                if (cache != null)
                    cache.UpdateStatus(SvnItem.FullPath, SvnDepth.Infinity);

                this.FindChildren();

                this.OnItemChanged(ItemChangedType.ChildrenInvalidated);
            }
            this.CurrentStatus = MergeStatuses(this.CheckChildStatuses(), this.ThisNodeStatus());
        }


        private void FindChildren()
        {
            this.Children.Clear();

            if (this.SvnItem.IsVersioned)
            {
                foreach (FileSystemItem item in GetFileSystemItemsForDirectory(this.SvnItem))
                {
                    this.Children.Add(item);
                }
            }
        }

        internal IFileSystemItem[] GetFileSystemItemsForDirectory(SvnItem directoryItem)
        {
            SortedList<string, IFileSystemItem> items = new SortedList<string, IFileSystemItem>(StringComparer.OrdinalIgnoreCase);

            SvnDirectory dir = Control.StatusCache.GetDirectory(directoryItem.FullPath);

            if (dir != null)
            {
                SvnItem dirItem = dir.Directory;

                foreach (SvnItem item in dir)
                {
                    if (items.ContainsKey(item.Name))
                        items.Add(item.Name, FileSystemFileItem.Create(Control, item));
                }
            }

            foreach (string path in Directory.GetFileSystemEntries(directoryItem.FullPath))
            {
                string name = Path.GetFileName(path);
                if (!string.Equals(name, SvnClient.AdministrativeDirectoryName, StringComparison.OrdinalIgnoreCase) &&
                    !items.ContainsKey(name))
                {
                    SvnItem item = Control.StatusCache[path];

                    items.Add(name, FileSystemItem.Create(Control, item));
                }
            }

            return new List<IFileSystemItem>(items.Values).ToArray();
        }
    }
}
