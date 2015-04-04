using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using CollectionMonitor = Ankh.CollectionChangedEventArgs.CollectionMonitor;

namespace Ankh
{
    public class ReadOnlyCollectionWithNotify<T> : ReadOnlyCollection<T>, ISupportsCollectionChanged<T> where T : class
    {
        EventHandler<CollectionChangedEventArgs<T>> _typedCollectionChanged;
        EventHandler<CollectionChangedEventArgs> _untypedCollectionChanged;
        PropertyChangedEventHandler _propertyChanged;
        readonly ISupportsCollectionChanged<T> _innerCollection;

        public ReadOnlyCollectionWithNotify(CollectionWithNotify<T> collection)
            : this(collection, collection)
        {
        }

        public ReadOnlyCollectionWithNotify(ReadOnlyCollectionWithNotify<T> collection)
            : this(collection._innerCollection.AsList(), collection._innerCollection)
        { }

        protected ReadOnlyCollectionWithNotify(IList<T> list, ISupportsCollectionChanged<T> collection)
            : base((list != null) ? list : collection.AsList())
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            _innerCollection = collection;
        }

        int _nHookChanges;
        protected void HookChanges()
        {
            _nHookChanges++;
            if (_nHookChanges == 1)
            {
                _innerCollection.CollectionChanged += HandleCollectionChanged;
                _innerCollection.PropertyChanged += HandlePropertyChanged;
            }
        }

        protected void UnhookChanges()
        {
            _nHookChanges--;
            if (_nHookChanges == 0)
            {
                _innerCollection.CollectionChanged -= HandleCollectionChanged;
                _innerCollection.PropertyChanged -= HandlePropertyChanged;
            }
        }

        private void HandleCollectionChanged(object sender, CollectionChangedEventArgs<T> e)
        {
            OnCollectionChanged(e);
        }

        protected virtual void OnCollectionChanged(CollectionChangedEventArgs<T> e)
        {
            if (_typedCollectionChanged != null)
                _typedCollectionChanged(this, e);
            if (_untypedCollectionChanged != null)
                _untypedCollectionChanged(this, e);
        }

        public event EventHandler<CollectionChangedEventArgs<T>> CollectionChanged
        {
            add { _typedCollectionChanged += value; HookChanges(); }
            remove { _typedCollectionChanged -= value; UnhookChanges(); }
        }

        event EventHandler<CollectionChangedEventArgs> ISupportsCollectionChanged.CollectionChanged
        {
            add { _untypedCollectionChanged += value; HookChanges(); }
            remove { _untypedCollectionChanged -= value; UnhookChanges(); }
        }

        IDisposable ISupportsCollectionChanged.BatchUpdate()
        {
            throw new NotSupportedException();
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged += value; HookChanges(); }
            remove { _propertyChanged -= value; UnhookChanges(); }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, e);
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }

        IList<T> ISupportsCollectionChanged<T>.AsList()
        {
            return this;
        }
    }
}
