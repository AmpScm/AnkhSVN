using System;
using System.Collections.Generic;
using System.ComponentModel;
using Ankh.Collections;

namespace Ankh
{
    public abstract class KeyedWrapNotifyCollection<TKey, TInner, TWrapped> : ReadOnlyKeyedNotifyCollection<TKey, TWrapped>, IKeyedNotifyCollection<TKey, TWrapped>, IWrapCollectionWithNotify<TInner, TWrapped>
        where TInner : class
        where TWrapped : class
    {
        readonly IAnkhServiceProvider _context;
        WrapInnerCollection _inner;

        public KeyedWrapNotifyCollection(IKeyedNotifyCollection<TKey, TInner> collection, IAnkhServiceProvider context)
            : base(new WrapInnerKeyedCollection(collection))
        {
            _context = context;
            _inner = (WrapInnerCollection)base.Items;
            _inner.Converter = this;

            _inner.ResetCollection();
        }

        public KeyedWrapNotifyCollection(IKeyedNotifyCollection<TKey, TInner> collection)
            : this(collection, (IAnkhServiceProvider)null)
        { }

        public KeyedWrapNotifyCollection(INotifyCollection<TInner> collection, IEqualityComparer<TKey> comparer, IAnkhServiceProvider context)
            : base(new WrapInnerCollection(collection, comparer))
        {
            _context = context;
            _inner = (WrapInnerKeyedCollection)base.Items;
            _inner.Converter = this;

            _inner.ResetCollection();
        }

        public KeyedWrapNotifyCollection(INotifyCollection<TInner> collection, IEqualityComparer<TKey> comparer)
            : this(collection, comparer, (IAnkhServiceProvider)null)
        { }

        protected IAnkhServiceProvider Context
        {
            get { return _context; }
        }

        protected void Dispose(bool disposing)
        {
            _inner.InvokeDispose();
        }

        class WrapInnerCollection : KeyedNotifyCollection<TKey, TWrapped>
        {
            readonly INotifyCollection<TInner> _sourceCollection;

            public WrapInnerCollection(INotifyCollection<TInner> collection, IEqualityComparer<TKey> comparer)
                : base(comparer)
            {
                if (collection == null)
                    throw new ArgumentNullException("collection");

                _sourceCollection = ReadOnlyNotifyCollection<TInner>.Unwrap(collection);

                _sourceCollection.CollectionChanged += OnSourceCollectionChanged;
                _sourceCollection.PropertyChanged += OnSourcePropertyChanged;
            }

            internal INotifyCollection<TInner> SourceCollection
            {
                get { return _sourceCollection;}
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
                            Insert(n++, Converter.GetWrapItem(inner));
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
                        n = e.NewStartingIndex;
                        foreach (TInner inner in e.NewItems)
                        {
                            this[n++] = Converter.GetWrapItem(inner);
                        }
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
                    Dictionary<TKey, TWrapped> map = new Dictionary<TKey, TWrapped>(Comparer);
                    foreach (TWrapped existing in Items)
                        map[GetKeyForItem(existing)] = existing; // Double key -> overwrite

                    Clear();
                    bool tryKey = true;
                    bool haveKey=true;
                    foreach (TInner inner in _sourceCollection)
                    {
                        TKey key;
                        if (tryKey || haveKey)
                        {
                            key = Converter.GetKeyForItem(inner);

                            if (tryKey)
                            {
                                tryKey = false;
                                haveKey = !Equals(key, default(TKey));
                            }
                        }
                        else
                            key = default(TKey);

                        TWrapped wrapped;

                        // Do we have a wrapped item via the key?
                        if (!haveKey || !map.TryGetValue(key, out wrapped))
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

            public KeyedWrapNotifyCollection<TKey, TInner, TWrapped> Converter { get; set; }

            ReadOnlyNotifyCollection<TInner> _roInner;
            public virtual INotifyCollection<TInner> GetWrappedCollection()
            {
                if (_roInner == null)
                {
                    _roInner = _sourceCollection as ReadOnlyNotifyCollection<TInner>;

                    if (_roInner == null)
                        _roInner = new ReadOnlyNotifyCollection<TInner>(_sourceCollection);
                }
                return _roInner;
            }
        }

        class WrapInnerKeyedCollection : WrapInnerCollection
        {
            IKeyedNotifyCollection<TKey, TInner> _sourceCollection;
            public WrapInnerKeyedCollection(IKeyedNotifyCollection<TKey, TInner> collection)
                : base(ReadOnlyKeyedNotifyCollection<TKey,TInner>.Unwrap(collection), collection.Comparer)
            {
                _sourceCollection = (IKeyedNotifyCollection<TKey, TInner>)base.SourceCollection;
            }

            protected internal override void ResetCollection()
            {
                using (BatchUpdate())
                {
                    Dictionary<TKey, TWrapped> map = new Dictionary<TKey, TWrapped>(Comparer);
                    foreach (TWrapped existing in Items)
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

            INotifyCollection<TInner> _roInner;
            public override INotifyCollection<TInner> GetWrappedCollection()
            {
                if (_roInner == null)
                {
                    _roInner = _sourceCollection as ReadOnlyKeyedNotifyCollection<TKey, TInner>;

                    if (_roInner == null)
                    {
                        KeyedNotifyCollection<TKey, TInner> rwInner = _sourceCollection as KeyedNotifyCollection<TKey, TInner>;

                        if (rwInner != null)
                            _roInner = new ReadOnlyKeyedNotifyCollection<TKey, TInner>(rwInner);
                        else
                            _roInner = base.GetWrappedCollection();
                    }
                }
                return _roInner;
            }
        }

        protected abstract TKey GetKeyForItem(TWrapped item);

        protected abstract TWrapped GetWrapItem(TInner inner);

        protected virtual TKey GetKeyForItem(TInner inner)
        {
            return default(TKey);
        }

        public IDisposable BatchUpdate()
        {
            return _inner.BatchUpdate();
        }

        TKey IKeyedNotifyCollection<TKey, TWrapped>.GetKeyForItem(TWrapped item)
        {
            return GetKeyForItem(item);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public INotifyCollection<TInner> GetWrappedCollection()
        {
            return _inner.GetWrappedCollection();
        }

        TWrapped IWrapCollectionWithNotify<TInner, TWrapped>.GetWrapItem(TInner inner)
        {
            TKey key = GetKeyForItem(inner);
            TWrapped value;
            if (Equals(key, default(TKey)) || TryGetValue(key, out value))
                return GetWrapItem(inner);
            else
                return value;
        }
    }
}
