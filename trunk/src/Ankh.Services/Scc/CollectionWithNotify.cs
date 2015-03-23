using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using CollectionMonitor = Ankh.CollectionChangedEventArgs.CollectionMonitor;

namespace Ankh
{
    /// <summary>
    /// Our own implementation of ObservableCollection, compatible with .Net 2.0
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionWithNotify<T> : Collection<T>, ISupportsCollectionChanged<T>, INotifyPropertyChanged where T : class
    {
        readonly CollectionMonitor _monitor = new CollectionMonitor();

        public CollectionWithNotify()
            : base()
        { }

        public CollectionWithNotify(IList<T> list)
            : base(list)
        { }

        protected override void ClearItems()
        {
            _monitor.CheckReentrancy(_collectionChangedTyped, _collectionChangedUntyped);
            if (Count > 0)
            {
                base.ClearItems();
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                RaiseCollectionChanged(new CollectionChangedEventArgs<T>(CollectionChange.Reset));
            }
        }

        protected override void InsertItem(int index, T item)
        {
            _monitor.CheckReentrancy(_collectionChangedTyped, _collectionChangedUntyped);
            base.InsertItem(index, item);
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("Item[]");
            RaiseCollectionChanged(new CollectionChangedEventArgs<T>(CollectionChange.Add, item, index));
        }

        protected override void RemoveItem(int index)
        {
            _monitor.CheckReentrancy(_collectionChangedTyped, _collectionChangedUntyped);
            T t = base[index];
            base.RemoveItem(index);
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("Item[]");
            RaiseCollectionChanged(new CollectionChangedEventArgs<T>(CollectionChange.Remove, t, index));
        }

        protected override void SetItem(int index, T item)
        {
            _monitor.CheckReentrancy(_collectionChangedTyped, _collectionChangedUntyped);
            T t = base[index];
            base.SetItem(index, item);
            this.OnPropertyChanged("Item[]");
            RaiseCollectionChanged(new CollectionChangedEventArgs<T>(CollectionChange.Replace, t, item, index));
        }

        public void Move(int oldIndex, int newIndex)
        {
            MoveItem(oldIndex, newIndex);
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            _monitor.CheckReentrancy(_collectionChangedUntyped, _collectionChangedTyped);
            T t = base[oldIndex];
            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, t);
            this.OnPropertyChanged("Item[]");
            RaiseCollectionChanged(new CollectionChangedEventArgs<T>(CollectionChange.Move, t, newIndex, oldIndex));
        }

        EventHandler<CollectionChangedEventArgs> _collectionChangedUntyped;
        EventHandler<CollectionChangedEventArgs<T>> _collectionChangedTyped;

        public event EventHandler<CollectionChangedEventArgs<T>> CollectionChanged
        {
            add { _collectionChangedTyped += value; }
            remove { _collectionChangedTyped -= value; }
        }

        event EventHandler<CollectionChangedEventArgs> ISupportsCollectionChanged.CollectionChanged
        {
            add { _collectionChangedUntyped += value; }
            remove { _collectionChangedUntyped -= value; }
        }

        protected void OnCollectionChanged(CollectionChangedEventArgs<T> e)
        {
            if (_collectionChangedTyped != null || _collectionChangedUntyped != null)
                using (_monitor.Enter())
                {
                    if (_collectionChangedTyped != null)
                        _collectionChangedTyped(this, e);
                    if (_collectionChangedUntyped != null)
                        _collectionChangedUntyped(this, e);
                }
        }

        sbyte _updateMode;
        CollectionChangedEventArgs<T> _change;

        void RaiseCollectionChanged(CollectionChangedEventArgs<T> e)
        {
            if (_updateMode == 0)
                OnCollectionChanged(e);
            else if (_updateMode == 1)
            {
                if (_change == null)
                    _change = e;
                else
                {
                    _updateMode = -1;
                    _change = null;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public IDisposable BatchUpdate()
        {
            Debug.Assert(_updateMode == 0);
            _updateMode = 1;
            return _monitor.BatchUpdate(DoneUpdate);
        }

        void DoneUpdate()
        {
            int oldMode = _updateMode;
            _updateMode = 0;
            if (oldMode == 1)
            {
                CollectionChangedEventArgs<T> c = _change;
                _change = null;

                if (c != null)
                    RaiseCollectionChanged(c);
                /* else NOOP batch */
            }
            else
                RaiseCollectionChanged(new CollectionChangedEventArgs<T>(CollectionChange.Reset));
        }
    }
}
