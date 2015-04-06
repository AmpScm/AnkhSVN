using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Ankh.Collections;

namespace Ankh
{
    public class ReadOnlyNotifyCollection<T> : ReadOnlyCollection<T>, INotifyCollection<T> where T : class
    {
        EventHandler<CollectionChangedEventArgs<T>> _collectionChanged;
        EventHandler<CollectionChangedEventArgs> _collectionChangedUntyped;
        PropertyChangedEventHandler _propertyChanged;
        readonly INotifyCollection<T> _innerCollection;

        public ReadOnlyNotifyCollection(INotifyCollection<T> collection)
            : base(collection)
        {
            _innerCollection = (INotifyCollection<T>)base.Items;
        }

        protected new INotifyCollection<T> Items
        {
            get { return _innerCollection; }
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
            if (_collectionChanged != null)
                _collectionChanged(this, e);
            if (_collectionChangedUntyped != null)
                _collectionChangedUntyped(this, e);
        }

        public event EventHandler<CollectionChangedEventArgs<T>> CollectionChanged
        {
            add { _collectionChanged += value; HookChanges(); }
            remove { _collectionChanged -= value; UnhookChanges(); }
        }

        event EventHandler<CollectionChangedEventArgs> INotifyCollection.CollectionChanged
        {
            add { _collectionChangedUntyped += value; HookChanges(); }
            remove { _collectionChangedUntyped -= value; UnhookChanges(); }
        }

        public virtual IDisposable BatchUpdate()
        {
            return null;
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

        public T[] ToArray()
        {
            T[] array = new T[Count];
            CopyTo(array, 0);
            return array;
        }
    }
}
