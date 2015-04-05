using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankh.Collections;

namespace Ankh.WpfUI.Controls
{
    sealed class ReadOnlyObservableWrapper<T> : ReadOnlyNotifyCollection<T>, INotifyCollectionChanged where T : class
    {
        NotifyCollectionChangedEventHandler _collectionChanged;

        internal ReadOnlyObservableWrapper(INotifyCollection<T> collection)
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
                    OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, e.NewItems, e.OldItems, e.NewStartingIndex));
                    break;
                case CollectionChange.Move:
                    OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, e.NewItems/*==OldItems*/, e.NewStartingIndex, e.OldStartingIndex));
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        void OnNotifyCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_collectionChanged != null)
                _collectionChanged(this, e);
        }

        public new event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { _collectionChanged += value; HookChanges(); }
            remove { _collectionChanged -= value; UnhookChanges(); }
        }
    }

    sealed class ReadOnlyObservableWrapper<TKey, TItem> : ReadOnlyKeyedNotifyCollection<TKey, TItem>, INotifyCollectionChanged where TItem : class
    {
        NotifyCollectionChangedEventHandler _collectionChanged;

        internal ReadOnlyObservableWrapper(IKeyedNotifyCollection<TKey, TItem> collection)
            : base(collection)
        {

        }

        protected override void OnCollectionChanged(CollectionChangedEventArgs<TItem> e)
        {
            base.OnCollectionChanged(e);

            switch (e.Action)
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
                    OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, e.NewItems, e.OldItems, e.NewStartingIndex));
                    break;
                case CollectionChange.Move:
                    OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, e.NewItems/*==OldItems*/, e.NewStartingIndex, e.OldStartingIndex));
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        void OnNotifyCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_collectionChanged != null)
                _collectionChanged(this, e);
        }

        public new event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { _collectionChanged += value; HookChanges(); }
            remove { _collectionChanged -= value; UnhookChanges(); }
        }
    }
}
