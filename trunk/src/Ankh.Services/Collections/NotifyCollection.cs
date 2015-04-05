using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Ankh.Collections;
using CollectionMonitor = Ankh.Collections.CollectionChangedEventArgs.CollectionMonitor;

namespace Ankh
{
    /// <summary>
    /// Our own implementation of ObservableCollection, compatible with .Net 2.0
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NotifyCollection<T> : Collection<T>, INotifyCollection<T> where T : class
    {
        readonly CollectionMonitor _monitor = new CollectionMonitor();

        public NotifyCollection()
            : base()
        { }

        public NotifyCollection(IList<T> list)
            : base(list)
        { }

        protected override void ClearItems()
        {
            _monitor.CheckReentrancy(_collectionChanged, _collectionChangedUntyped);
            if (Count > 0)
            {
                base.ClearItems();
                RaisePropertyChanged(RaisePropertyItems.Count | RaisePropertyItems.Items);
                RaiseCollectionChanged(new CollectionChangedEventArgs<T>(CollectionChange.Reset));
            }
        }

        protected override void InsertItem(int index, T item)
        {
            _monitor.CheckReentrancy(_collectionChanged, _collectionChangedUntyped);
            base.InsertItem(index, item);
            RaisePropertyChanged(RaisePropertyItems.Count | RaisePropertyItems.Items);
            RaiseCollectionChanged(new CollectionChangedEventArgs<T>(CollectionChange.Add, item, index));
        }

        protected override void RemoveItem(int index)
        {
            _monitor.CheckReentrancy(_collectionChanged, _collectionChangedUntyped);
            T t = base[index];
            base.RemoveItem(index);
            RaisePropertyChanged(RaisePropertyItems.Count | RaisePropertyItems.Items);
            RaiseCollectionChanged(new CollectionChangedEventArgs<T>(CollectionChange.Remove, t, index));
        }

        protected override void SetItem(int index, T item)
        {
            _monitor.CheckReentrancy(_collectionChanged, _collectionChangedUntyped);
            T t = base[index];
            base.SetItem(index, item);
            RaisePropertyChanged(RaisePropertyItems.Items);
            RaiseCollectionChanged(new CollectionChangedEventArgs<T>(CollectionChange.Replace, t, item, index));
        }

        public void Move(int oldIndex, int newIndex)
        {
            MoveItem(oldIndex, newIndex);
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            _monitor.CheckReentrancy(_collectionChangedUntyped, _collectionChanged);
            T t = base[oldIndex];
            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, t);
            RaisePropertyChanged(RaisePropertyItems.Items);
            RaiseCollectionChanged(new CollectionChangedEventArgs<T>(CollectionChange.Move, t, newIndex, oldIndex));
        }

        EventHandler<CollectionChangedEventArgs> _collectionChangedUntyped;
        EventHandler<CollectionChangedEventArgs<T>> _collectionChanged;

        public event EventHandler<CollectionChangedEventArgs<T>> CollectionChanged
        {
            add { _collectionChanged += value; }
            remove { _collectionChanged -= value; }
        }

        event EventHandler<CollectionChangedEventArgs> INotifyCollection.CollectionChanged
        {
            add { _collectionChangedUntyped += value; }
            remove { _collectionChangedUntyped -= value; }
        }

        protected void OnCollectionChanged(CollectionChangedEventArgs<T> e)
        {
            if (_collectionChanged != null || _collectionChangedUntyped != null)
                using (_monitor.Enter())
                {
                    if (_collectionChanged != null)
                        _collectionChanged(this, e);
                    if (_collectionChangedUntyped != null)
                        _collectionChangedUntyped(this, e);
                }
        }

        sbyte _updateMode;
        RaisePropertyItems _updateProps;
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

        void RaisePropertyChanged(RaisePropertyItems which)
        {
            if (_updateMode == 0)
            {
                if ((which & RaisePropertyItems.Count) != 0)
                    OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                if ((which & RaisePropertyItems.Items) != 0)
                    OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            }
            else
            {
                _updateProps |= which;
            }
        }

        public IDisposable BatchUpdate()
        {
            Debug.Assert(_updateMode == 0);
            if (_updateMode == 0)
            {
                _updateMode = 1;
                return _monitor.BatchUpdate(DoneUpdate);
            }
            else
                return null;
        }

        void DoneUpdate()
        {
            int oldMode = _updateMode;
            _updateMode = 0;

            if (_updateProps != 0)
            {
                RaisePropertyItems props = _updateProps;
                _updateProps = 0;
                RaisePropertyChanged(props);
            }

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

        public T[] ToArray()
        {
            T[] array = new T[Count];
            CopyTo(array, 0);
            return array;
        }
    }
}
