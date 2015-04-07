using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Ankh.Collections;

namespace Ankh
{
    public class WrapNotifyCollection<TInner, TWrapped> : ReadOnlyNotifyCollection<TWrapped>, IWrapCollectionWithNotify<TInner, TWrapped>
        where TInner : class
        where TWrapped : class
    {
        readonly object _context;
        readonly WrapInnerCollection _inner;
        readonly WrapItem<TInner, TWrapped> _wrapper;

        public WrapNotifyCollection(INotifyCollection<TInner> collection, WrapItem<TInner, TWrapped> wrapper, object context)
            : base(new WrapInnerCollection(collection))
        {
            _context = context;
            _wrapper = wrapper;

            _inner = (WrapInnerCollection)this.Items;
            _inner.Converter = this;

            OnPreInitialize(context);
            _inner.ResetCollection();
        }

        public WrapNotifyCollection(INotifyCollection<TInner> collection, WrapItem<TInner, TWrapped> wrapper)
            : this(collection, wrapper, (IAnkhServiceProvider)null)
        { }

        protected WrapNotifyCollection(INotifyCollection<TInner> collection)
            : this(collection, null, null)
        { }

        /// <summary>
        /// Called from the constructor before copying the inner list
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual void OnPreInitialize(object context)
        {

        }

        protected object Context
        {
            get { return _context; }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing); // Unhooks our Disposing hook on inner
            if (disposing)
                _inner.Dispose();
        }

        class WrapInnerCollection : NotifyCollection<TWrapped>
        {
            readonly INotifyCollection<TInner> _sourceCollection;

            public WrapInnerCollection(INotifyCollection<TInner> collection)
            {
                if (collection == null)
                    throw new ArgumentNullException("collection");

                _sourceCollection = collection;
                _sourceCollection.CollectionChanged += OnSourceCollectionChanged;
                _sourceCollection.PropertyChanged += OnSourcePropertyChanged;
                _sourceCollection.Disposed += OnSourceCollectionDisposed;
            }

            private void OnSourceCollectionDisposed(object sender, EventArgs e)
            {
                Dispose(true);
            }

            protected override void Dispose(bool disposing)
            {
                try
                {
                    if (disposing)
                    {
                        _sourceCollection.CollectionChanged -= OnSourceCollectionChanged;
                        _sourceCollection.PropertyChanged -= OnSourcePropertyChanged;
                        _sourceCollection.Disposed -= OnSourceCollectionDisposed;
                    }
                }
                finally
                {
                    base.Dispose(disposing);
                }
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
                            Insert(n++, Converter.GetWrapItem(key));
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
                    Clear();
                    foreach (TInner key in _sourceCollection)
                    {
                        Add(Converter.GetWrapItem(key));
                    }
                }
            }

            public WrapNotifyCollection<TInner, TWrapped> Converter { get; set; }

            public INotifyCollection<TInner> GetWrappedCollection()
            {
                return _sourceCollection;
            }

            internal void Dispose()
            {
                Dispose(true);
            }
        }

        public sealed override IDisposable BatchUpdate()
        {
            return _inner.BatchUpdate();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual TWrapped GetWrapItem(TInner inner)
        {
            return _wrapper(inner);
        }

        public INotifyCollection<TInner> GetWrappedCollection()
        {
            return _inner.GetWrappedCollection();
        }

        TWrapped IWrapCollectionWithNotify<TInner, TWrapped>.GetWrapItem(TInner inner)
        {
            return GetWrapItem(inner);
        }
    }
}
