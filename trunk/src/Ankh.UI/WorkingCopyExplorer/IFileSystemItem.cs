using System;
using System.Text;

namespace Ankh.UI
{
    public enum ItemChangedType
    {
        StatusChanged,
        ChildrenInvalidated
    }

    public class ItemChangedEventArgs : EventArgs
    {
        public ItemChangedEventArgs( ItemChangedType changeType )
        {
            this.changeType = changeType;
        }
        public ItemChangedType ItemChangedType
        {
            get { return this.changeType; }
            set { this.changeType = value; }
        }

        private ItemChangedType changeType;
    }

    public interface IFileSystemItem
    {
        bool IsContainer
        {
            get;
        }

        string Text
        {
            get;
        }

        void Open(IAnkhServiceProvider context);

        SvnItem SvnItem { get; }

        IFileSystemItem[] GetChildren();
    }
}
