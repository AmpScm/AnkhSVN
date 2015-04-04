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
        readonly KeyedCollectionWithNotify<TKey, TItem> _innerCollection;

        public ReadOnlyKeyedCollectionWithNotify(KeyedCollectionWithNotify<TKey, TItem> collection)
            : base(collection, collection)
        {
            _innerCollection = collection;
        }

        public ReadOnlyKeyedCollectionWithNotify(ReadOnlyKeyedCollectionWithNotify<TKey, TItem> collection)
            : base(collection._innerCollection, collection._innerCollection)
        {
            _innerCollection = collection._innerCollection;
        }

        public bool Contains(TKey key)
        {
            return _innerCollection.Contains(key);
        }

        public TItem this[TKey key]
        {
            get { return _innerCollection[key]; }
        }

        public IEqualityComparer<TKey> Comparer
        {
            get { return _innerCollection.Comparer; }
        }

        TKey ISupportsKeyedCollectionChanged<TKey, TItem>.GetKeyForItem(TItem item)
        {
            return ((ISupportsKeyedCollectionChanged<TKey, TItem>)_innerCollection).GetKeyForItem(item);
        }

        IDisposable ISupportsCollectionChanged.BatchUpdate()
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TItem value)
        {
            return _innerCollection.TryGetValue(key, out value);
        }
    }
}
