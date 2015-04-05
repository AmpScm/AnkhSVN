using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Ankh.Collections;
using CollectionMonitor = Ankh.Collections.CollectionChangedEventArgs.CollectionMonitor;

namespace Ankh
{
    public class ReadOnlyNotifyCollection<T> : ReadOnlyCollection<T>, INotifyCollection<T> where T : class
    {
        EventHandler<CollectionChangedEventArgs<T>> _typedCollectionChanged;
        EventHandler<CollectionChangedEventArgs> _untypedCollectionChanged;
        PropertyChangedEventHandler _propertyChanged;
        readonly INotifyCollection<T> _innerCollection;

        public ReadOnlyNotifyCollection(INotifyCollection<T> collection)
            : base(Unwrap(collection))
        {
            _innerCollection = (INotifyCollection<T>)base.Items;
        }

        internal static INotifyCollection<T> Unwrap(INotifyCollection<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

#if !DEBUG && UNWRAP
            ReadOnlyNotifyCollection<T> ro = collection as ReadOnlyNotifyCollection<T>;
            if (ro != null)
                return Unwrap(ro._innerCollection);
            else
#endif
            return collection;
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

        event EventHandler<CollectionChangedEventArgs> INotifyCollection.CollectionChanged
        {
            add { _untypedCollectionChanged += value; HookChanges(); }
            remove { _untypedCollectionChanged -= value; UnhookChanges(); }
        }

        IDisposable INotifyCollection.BatchUpdate()
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
