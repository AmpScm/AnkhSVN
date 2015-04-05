using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Ankh.Collections;
using CollectionMonitor = Ankh.Collections.CollectionChangedEventArgs.CollectionMonitor;

namespace Ankh
{
    public class ReadOnlyKeyedNotifyCollection<TKey, TItem> : ReadOnlyNotifyCollection<TItem>, IKeyedNotifyCollection<TKey, TItem>
        where TItem : class
    {
        readonly IKeyedNotifyCollection<TKey, TItem> _innerCollection;

        public ReadOnlyKeyedNotifyCollection(IKeyedNotifyCollection<TKey, TItem> collection)
            : base(Unwrap(collection))
        {
            _innerCollection = (IKeyedNotifyCollection<TKey, TItem>)base.Items;
        }

        internal static IKeyedNotifyCollection<TKey, TItem> Unwrap(IKeyedNotifyCollection<TKey, TItem> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

#if !DEBUG && UNWRAP
            ReadOnlyKeyedNotifyCollection<TKey, TItem> ro = collection as ReadOnlyKeyedNotifyCollection<TKey, TItem>;
            if (ro != null)
                return Unwrap(ro._innerCollection);
            else
#endif
            return collection;
        }

        protected new IKeyedNotifyCollection<TKey, TItem> Items
        {
            get { return _innerCollection; }
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

        TKey IKeyedNotifyCollection<TKey, TItem>.GetKeyForItem(TItem item)
        {
            return _innerCollection.GetKeyForItem(item);
        }

        public bool TryGetValue(TKey key, out TItem value)
        {
            return _innerCollection.TryGetValue(key, out value);
        }
    }
}
