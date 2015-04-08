using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ankh.Collections
{
    public sealed class ItemChangedEventArgs<T> : EventArgs
        where T : class
    {
        readonly T _item;
        public ItemChangedEventArgs(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _item = item;
        }

        public T Item
        {
            get { return _item; }
        }
    }

    public interface INotifyItemChanged<T> : INotifyCollection<T>
        where T : class
    {
        event EventHandler<ItemChangedEventArgs<T>> ItemChanged;
    }
}
