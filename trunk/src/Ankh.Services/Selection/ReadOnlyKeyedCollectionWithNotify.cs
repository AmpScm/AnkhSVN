using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using CollectionMonitor = Ankh.CollectionChangedEventArgs.CollectionMonitor;

namespace Ankh
{
    public class ReadOnlyKeyedCollectionWithNotify<TKey, TItem> : ReadOnlyCollectionWithNotify<TItem>, ISupportsKeyedCollectionChanged<TKey, TItem>
        where TItem : class
    {
        readonly KeyedCollectionWithNotify<TKey, TItem> _collection;

        public ReadOnlyKeyedCollectionWithNotify(KeyedCollectionWithNotify<TKey, TItem> collection)
            : base(collection, collection)
        {
            _collection = collection;
        }

        public ReadOnlyKeyedCollectionWithNotify(ReadOnlyKeyedCollectionWithNotify<TKey, TItem> collection)
            : base(collection._collection, collection._collection)
        {
            _collection = collection._collection;
        }

        public bool Contains(TKey key)
        {
            return _collection.Contains(key);
        }

        public TItem this[TKey key]
        {
            get { return _collection[key]; }
        }

        public IEqualityComparer<TKey> Comparer
        {
            get { return _collection.Comparer; }
        }

        public virtual TKey GetKeyForItem(TItem item)
        {
            return ((ISupportsKeyedCollectionChanged<TKey, TItem>)_collection).GetKeyForItem(item);
        }

        public IDisposable BatchUpdate()
        {
            throw new NotImplementedException();
        }
    }
}
