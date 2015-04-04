using System;
using System.Collections.Generic;
using System.ComponentModel;
using Ankh.Selection;

namespace Ankh
{
    public abstract class KeyedWrapCollectionWithNotify<TInner, TKey, TWrapped> : ReadOnlyKeyedCollectionWithNotify<TKey,TWrapped>, ISupportsKeyedCollectionChanged<TKey, TWrapped>
        where TInner : class
        where TWrapped : class
    {
        WrapInnerCollection _inner;

        public KeyedWrapCollectionWithNotify(KeyedCollectionWithNotify<TKey, TInner> collection)
            : base(new WrapInnerCollection(collection))
        {
            _inner = (WrapInnerCollection)base.Items;
            _inner.Converter = this;
        }

        public KeyedWrapCollectionWithNotify(ReadOnlyKeyedCollectionWithNotify<TKey, TInner> collection)
            : base(new WrapInnerCollection(collection))
        {
            _inner = (WrapInnerCollection)base.Items;
            _inner.Converter = this;
        }

        protected void Dispose(bool disposing)
        {
            ((WrapInnerCollection)this.Items).InvokeDispose();
        }

        class WrapInnerCollection : KeyedCollectionWithNotify<TKey, TWrapped>
        {
            readonly ISupportsKeyedCollectionChanged<TKey, TInner> _collection;
            readonly IList<TInner> _sourceList;

            public WrapInnerCollection(ISupportsKeyedCollectionChanged<TKey, TInner> collection)
                : base(collection.Comparer)
            {
                if (collection == null)
                    throw new ArgumentNullException("collection");

                _collection = collection;
                _sourceList = collection.AsList();

                ResetCollection();

                _collection.CollectionChanged += OnSourceCollectionChanged;
                _collection.PropertyChanged += OnSourcePropertyChanged;
            }

            protected virtual void Dispose(bool disposing)
            {
                _collection.CollectionChanged -= OnSourceCollectionChanged;
                _collection.PropertyChanged -= OnSourcePropertyChanged;
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
                    case CollectionChange.Move:
                        throw new NotImplementedException();
                }
            }

            protected virtual void ResetCollection()
            {
                using (BatchUpdate())
                {
                    Dictionary<TKey, TWrapped> map = new Dictionary<TKey,TWrapped>(Comparer);
                    foreach(TWrapped existing in this)
                        map[GetKeyForItem(existing)] = existing; // Double key -> overwrite

                    Clear();
                    foreach (TInner inner in _sourceList)
                    {
                        TKey key = _collection.GetKeyForItem(inner);
                        TWrapped wrapped;

                        if (!map.TryGetValue(key, out wrapped))
                            wrapped = Converter.GetWrapItem(inner);

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

        public override abstract TKey GetKeyForItem(TWrapped item);

        protected abstract TWrapped GetWrapItem(object inner);

        IDisposable ISupportsCollectionChanged.BatchUpdate()
        {
            throw new NotImplementedException();
        }

        TKey ISupportsKeyedCollectionChanged<TKey, TWrapped>.GetKeyForItem(TWrapped item)
        {
            return GetKeyForItem(item);
        }
    }
}
