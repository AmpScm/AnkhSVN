using System;
using System.Collections.Generic;
using System.ComponentModel;
using Ankh.Selection;

namespace Ankh
{
    public abstract class KeyedWrapCollectionWithNotify<TInner, TKey, TWrapped> : ReadOnlyKeyedCollectionWithNotify<TKey,TWrapped>, ISupportsKeyedCollectionChanged<TKey, TWrapped>, IDisposable
        where TInner : class
        where TWrapped : class
    {
        readonly IAnkhServiceProvider _context;
        WrapInnerCollection _inner;

        public KeyedWrapCollectionWithNotify(IAnkhServiceProvider context, KeyedCollectionWithNotify<TKey, TInner> collection)
            : base(new WrapInnerCollection(collection))
        {
            _context = context;
            _inner = (WrapInnerCollection)base.Items;
            _inner.Converter = this;

            ((WrapInnerCollection)this.Items).ResetCollection();
        }

        public KeyedWrapCollectionWithNotify(IAnkhServiceProvider context, ReadOnlyKeyedCollectionWithNotify<TKey, TInner> collection)
            : base(new WrapInnerCollection(collection))
        {
            _context = context;
            _inner = (WrapInnerCollection)base.Items;
            _inner.Converter = this;

            ((WrapInnerCollection)this.Items).ResetCollection();
        }

        protected IAnkhServiceProvider Context
        {
            get { return _context; }
        }

        protected void Dispose(bool disposing)
        {
            ((WrapInnerCollection)this.Items).InvokeDispose();
        }

        class WrapInnerCollection : KeyedCollectionWithNotify<TKey, TWrapped>
        {
            readonly ISupportsKeyedCollectionChanged<TKey, TInner> _sourceCollection;

            public WrapInnerCollection(ISupportsKeyedCollectionChanged<TKey, TInner> collection)
                : base(collection.Comparer)
            {
                if (collection == null)
                    throw new ArgumentNullException("collection");

                _sourceCollection = collection;

                _sourceCollection.CollectionChanged += OnSourceCollectionChanged;
                _sourceCollection.PropertyChanged += OnSourcePropertyChanged;
            }

            protected virtual void Dispose(bool disposing)
            {
                _sourceCollection.CollectionChanged -= OnSourceCollectionChanged;
                _sourceCollection.PropertyChanged -= OnSourcePropertyChanged;
            }

            private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                OnPropertyChanged(e);
            }

            private void OnSourceCollectionChanged(object sender, CollectionChangedEventArgs<TInner> e)
            {
                int n;
                switch (e.Action)
                {
                    case CollectionChange.Reset:
                        ResetCollection();
                        break;
                    case CollectionChange.Add:
                        n = e.NewStartingIndex;
                        foreach (TInner inner in e.NewItems)
                        {
                            Insert(n++, this.Converter.GetWrapItem(inner));
                        }
                        break;
                    case CollectionChange.Remove:
                        n = e.OldStartingIndex;
                        foreach (TInner inner in e.OldItems)
                        {
                            RemoveAt(n);
                        }
                        break;
                    case CollectionChange.Replace:
                        this[e.NewStartingIndex] = this.Converter.GetWrapItem(e.NewItems[0]);
                        break;
                    case CollectionChange.Move:
                        Move(e.OldStartingIndex, e.NewStartingIndex);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            protected internal virtual void ResetCollection()
            {
                using (BatchUpdate())
                {
                    Dictionary<TKey, TWrapped> map = new Dictionary<TKey,TWrapped>(Comparer);
                    foreach(TWrapped existing in Items)
                        map[GetKeyForItem(existing)] = existing; // Double key -> overwrite

                    Clear();
                    foreach (TInner inner in _sourceCollection)
                    {
                        TKey key = _sourceCollection.GetKeyForItem(inner);
                        TWrapped wrapped;

                        // Do we have a wrapped item via the key?
                        if (!map.TryGetValue(key, out wrapped))
                        {
                            wrapped = Converter.GetWrapItem(inner);

                            // Do we have a wrapped item via the wrapper key?
                            TWrapped wrapped2;
                            if (map.TryGetValue(GetKeyForItem(wrapped), out wrapped2))
                                wrapped = wrapped2;
                        }

                        Add(wrapped);
                    }
                }
            }

            internal void InvokeDispose()
            {
                Dispose(true);
            }

            protected override TKey GetKeyForItem(TWrapped item)
            {
                return Converter.GetKeyForItem(item);
            }

            internal KeyedWrapCollectionWithNotify<TInner, TKey, TWrapped> Converter { get; set; }
        }

        protected abstract TKey GetKeyForItem(TWrapped item);

        protected abstract TWrapped GetWrapItem(TInner inner);

        IDisposable ISupportsCollectionChanged.BatchUpdate()
        {
            throw new NotImplementedException();
        }

        TKey ISupportsKeyedCollectionChanged<TKey, TWrapped>.GetKeyForItem(TWrapped item)
        {
            return GetKeyForItem(item);
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
