using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Ankh.Collections;

namespace Ankh
{
    public class ReadOnlyKeyedNotifyCollection<TKey, TItem> : ReadOnlyNotifyCollection<TItem>, IKeyedNotifyCollection<TKey, TItem>
        where TItem : class
    {
        readonly IKeyedNotifyCollection<TKey, TItem> _innerCollection;

        public ReadOnlyKeyedNotifyCollection(IKeyedNotifyCollection<TKey, TItem> collection)
            : base(collection)
        {
            _innerCollection = (IKeyedNotifyCollection<TKey, TItem>)base.Items;
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
