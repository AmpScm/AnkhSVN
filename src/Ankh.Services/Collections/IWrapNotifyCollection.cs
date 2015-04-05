using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Ankh.Collections
{
    public delegate TWrapped WrapItem<TInner, TWrapped>(TInner item);

    public interface IWrapCollectionWithNotify<TInner, TWrapped> : INotifyCollection<TWrapped>, IDisposable
        where TInner : class
        where TWrapped : class
    {
        INotifyCollection<TInner> GetWrappedCollection();
        TWrapped GetWrapItem(TInner inner);
    }
}
