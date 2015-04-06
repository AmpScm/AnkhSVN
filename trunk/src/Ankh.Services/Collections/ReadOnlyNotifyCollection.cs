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
        EventHandler _disposed;
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

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    InnerUnhookChanges();

                    if (_disposed != null)
                        _disposed(this, EventArgs.Empty);
                }
            }
            finally
            {
                _collectionChanged = null;
                _collectionChangedUntyped = null;
                _propertyChanged = null;
                _disposed = null;
            }
        }

        int _nHookChanges;
        protected void HookChanges()
        {
            _nHookChanges++;
            if (_nHookChanges == 1)
            {
                _innerCollection.CollectionChanged += HandleCollectionChanged;
                _innerCollection.PropertyChanged += HandlePropertyChanged;
                _innerCollection.Disposed += HandleCollectionDisposed;
            }
        }

        protected void UnhookChanges()
        {
            _nHookChanges--;
            if (_nHookChanges == 0)
            {
                InnerUnhookChanges();
            }
        }

        void InnerUnhookChanges()
        {
            _innerCollection.CollectionChanged -= HandleCollectionChanged;
            _innerCollection.PropertyChanged -= HandlePropertyChanged;
            _innerCollection.Disposed -= HandleCollectionDisposed;
            _nHookChanges = 0;
        }

        private void HandleCollectionChanged(object sender, CollectionChangedEventArgs<T> e)
        {
            OnCollectionChanged(e);
        }

        private void HandleCollectionDisposed(object sender, EventArgs e)
        {
            Dispose(true);
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

        public event EventHandler Disposed
        {
            add { _disposed += value; HookChanges(); }
            remove { _disposed -= value; UnhookChanges(); }
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
