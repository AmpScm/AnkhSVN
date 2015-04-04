using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Ankh
{
    public delegate TWrapped WrapItem<TInner, TWrapped>(TInner item);

    public class WrapCollectionWithNotify<TInner, TWrapped> : ReadOnlyCollectionWithNotify<TWrapped>, IDisposable
        where TInner : class
        where TWrapped : class
    {
        public WrapCollectionWithNotify(CollectionWithNotify<TInner> collection, WrapItem<TInner, TWrapped> wrapper)
            : base(new WrapInnerCollection(collection, wrapper))
        {
        }

        public WrapCollectionWithNotify(ReadOnlyCollectionWithNotify<TInner> collection, WrapItem<TInner, TWrapped> wrapper)
            : base(new WrapInnerCollection(collection, wrapper))
        {
        }

        protected void Dispose(bool disposing)
        {
            ((WrapInnerCollection)this.Items).Dispose(disposing);
        }

        class WrapInnerCollection : CollectionWithNotify<TWrapped>
        {
            readonly ISupportsCollectionChanged<TInner> _collection;
            readonly IList<TInner> _sourceList;
            readonly WrapItem<TInner, TWrapped> _wrapper;

            public WrapInnerCollection(ISupportsCollectionChanged<TInner> collection, WrapItem<TInner, TWrapped> wrapper)
            {
                if (collection == null)
                    throw new ArgumentNullException("collection");
                if (wrapper == null)
                    throw new ArgumentNullException("wrapper");

                _collection = collection;
                _sourceList = collection.AsList();
                _wrapper = wrapper;

                ResetCollection();

                _collection.CollectionChanged += OnSourceCollectionChanged;
                _collection.PropertyChanged += OnSourcePropertyChanged;
            }

            internal void Dispose(bool disposing)
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
                        foreach (TInner key in e.NewItems)
                        {
                            Insert(n++, _wrapper(key));
                        }
                        break;
                    case CollectionChange.Remove:
                        n = e.OldStartingIndex;
                        foreach (TInner key in e.OldItems)
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
                    Clear();
                    foreach (TInner key in _sourceList)
                    {
                        Add(_wrapper(key));
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
