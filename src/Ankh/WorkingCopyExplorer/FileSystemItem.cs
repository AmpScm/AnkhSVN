using System;
using System.Text;
using Ankh.UI;
using System.IO;

using Utils;
using System.Collections;
using SharpSvn;
using Ankh.Scc;

namespace Ankh.WorkingCopyExplorer
{
    internal abstract class FileSystemItem : TreeNode, Ankh.UI.IFileSystemItem
    {
        readonly IAnkhServiceProvider _context;
        readonly SvnItem _item;
        public event EventHandler<ItemChangedEventArgs> ItemChanged;

        public FileSystemItem(IAnkhServiceProvider context, FileSystemItem parent, WorkingCopyExplorer explorer, SvnItem svnItem)
            : base(parent)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (svnItem == null)
                throw new ArgumentNullException("svnItem");

            _context = context;
            _item = svnItem;

            this.explorer = explorer;
            this.CurrentStatus = MergeStatuses(_item);
        }

        protected IAnkhServiceProvider Context
        {
            get { return _context; }
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

        public WorkingCopyExplorer Explorer
        {
            get { return this.explorer; }
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

        public void Open()
        {
            this.explorer.OpenItem(this.SvnItem.FullPath);
        }

        [TextProperty("TextStatus", Order = 0, TextWidth = 18)]
        public SvnStatus TextStatus
        {
            get { return _item.Status.LocalContentStatus; }
        }

        [TextProperty("PropertyStatus", Order = 1, TextWidth = 18)]
        public SvnStatus PropertyStatus
        {
            get { return _item.Status.LocalPropertyStatus; }
        }

        [TextProperty("Locked", Order = 2, TextWidth = 10)]
        public string Locked
        {
            get
            {
                return _item.IsLocked ? "Yes" : "";
            }
        }

        [TextProperty("Read-only", Order = 3, TextWidth = 13)]
        public string ReadOnly
        {
            get
            {
                return _item.IsReadOnly ? "Yes" : "";
            }
        }

        [StateImageProperty]
        public int StateImage
        {
            get
            {
                return (int)_context.GetService<IStatusImageMapper>().GetStatusImageForSvnItem(SvnItem);
            }
        }


        public override bool Equals(object obj)
        {
            return Equals(obj as FileSystemItem);
        }

        public bool Equals(FileSystemItem other)
        {
            if (other == null)
                return false;

            return PathUtils.AreEqual(this.SvnItem.FullPath, other.SvnItem.FullPath);
        }

        public override int GetHashCode()
        {
            return _item.FullPath.GetHashCode();
        }

        internal new void Refresh()
        {
            _item.MarkDirty();
        }

        public static FileSystemItem Create(IAnkhServiceProvider context, WorkingCopyExplorer explorer, SvnItem item)
        {
            if (item.IsDirectory)
            {
                return new FileSystemDirectoryItem(context, explorer, item);
            }
            else
            {
                return new FileSystemFileItem(context, explorer, item);
            }
        }

        protected override void DoDispose()
        {
            //this.svnItem.Changed -= new EventHandler( this.ChildOrResourceChanged );
        }

        protected override bool RemoveTreeNodeIfResourcesDeleted()
        {
            // TODO: Implement
            return false;
        }

        protected override NodeStatus ThisNodeStatus()
        {
            return MergeStatuses(_item);
        }

        protected virtual void OnItemChanged(ItemChangedType changeType)
        {
            if (this.ItemChanged != null)
            {
                this.ItemChanged(this, new ItemChangedEventArgs(changeType));
            }
        }

        protected override void OnChanged()
        {
            base.OnChanged();

            this.OnItemChanged(ItemChangedType.StatusChanged);
        }

        private WorkingCopyExplorer explorer;
    }
}
