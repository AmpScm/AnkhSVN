using System;
using System.Text;
using Ankh.UI;
using System.IO;

using System.Collections;
using SharpSvn;
using Ankh.Scc;
using Ankh.UI.WorkingCopyExplorer;

namespace Ankh.WorkingCopyExplorer
{
    internal abstract class FileSystemItem : WCTreeNode, Ankh.UI.IFileSystemItem
    {
        readonly SvnItem _item;
        readonly WorkingCopyExplorerControl _ctrl;

        public FileSystemItem(WorkingCopyExplorerControl control, FileSystemItem parent, SvnItem svnItem)
            : base(parent)
        {
            if (control == null)
                throw new ArgumentNullException("control");
            else if (svnItem == null)
                throw new ArgumentNullException("svnItem");

            _item = svnItem;
            _ctrl = control;
        }

        protected IAnkhServiceProvider Context
        {
            get { return _ctrl.Context; }
        }

        protected WorkingCopyExplorerControl Control
        {
            get { return _ctrl; }
        }

        public abstract bool IsContainer { get; }

        public virtual string Text
        {
            get
            {
                return _item.Name;
            }
        }

        public SvnItem SvnItem
        {
            get { return _item; }
        }

        public override void GetResources(IList list, bool getChildItems, Predicate<SvnItem> filter)
        {
            if (filter(_item))
            {
                list.Add(_item);
            }
            if (getChildItems)
            {
                this.GetChildResources(list, getChildItems, filter);
            }
        }




        public abstract IFileSystemItem[] GetChildren();

        public void Open(IAnkhServiceProvider context)
        {
            Control.OpenItem(context, SvnItem.FullPath);
        }


        public override bool Equals(object obj)
        {
            return Equals(obj as FileSystemItem);
        }

        public bool Equals(FileSystemItem other)
        {
            if (other == null)
                return false;

            return String.Equals(this.SvnItem.FullPath, other.SvnItem.FullPath, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return _item.FullPath.GetHashCode();
        }

        public static FileSystemItem Create(WorkingCopyExplorerControl control, SvnItem item)
        {
            if (item.IsDirectory)
            {
                return new FileSystemDirectoryItem(control, item);
            }
            else
            {
                return new FileSystemFileItem(control, item);
            }
        } 
    }
}
