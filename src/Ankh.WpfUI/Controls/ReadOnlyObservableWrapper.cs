using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ankh.WpfUI.Controls
{
    sealed class ReadOnlyObservableWrapper<T> : ReadOnlyCollectionWithNotify<T>, INotifyCollectionChanged where T : class
    {
        public new event NotifyCollectionChangedEventHandler CollectionChanged;

        public ReadOnlyObservableWrapper(CollectionWithNotify<T> collection)
            : base(collection)
        {

        }

        public ReadOnlyObservableWrapper(ReadOnlyCollectionWithNotify<T> collection)
            : base(collection)
        {

        }

        protected override void OnCollectionChanged(CollectionChangedEventArgs<T> e)
        {
            base.OnCollectionChanged(e);

            switch(e.Action)
            {
                case CollectionChange.Reset:
                    OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    break;
                case CollectionChange.Add:
                    OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItems[0], e.NewStartingIndex));
                    break;
                case CollectionChange.Remove:
                    OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.OldItems[0], e.OldStartingIndex));
                    break;
                case CollectionChange.Replace:
                case CollectionChange.Move:
                    throw new NotImplementedException();
            }
        }

        protected void OnNotifyCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, e);
        }

        public void Dispose()
        {
            base.Dispose(true);
        }
    }
}
