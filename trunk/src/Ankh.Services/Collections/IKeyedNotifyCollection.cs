using System;
using System.Collections.Generic;

namespace Ankh.Collections
{
    public interface IKeyedNotifyCollection<TKey, TItem> : INotifyCollection<TItem>
        where TItem : class
    {
        IEqualityComparer<TKey> Comparer { get; }
        TKey GetKeyForItem(TItem item);

        bool TryGetValue(TKey key, out TItem value);

        TItem this[TKey key] { get; }

        bool Contains(TKey key);
    }
}
