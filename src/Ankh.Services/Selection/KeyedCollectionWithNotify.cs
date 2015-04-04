using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using CollectionMonitor = Ankh.CollectionChangedEventArgs.CollectionMonitor;

namespace Ankh
{
    interface ISupportsKeyedCollectionChanged<TKey, TItem> : ISupportsCollectionChanged<TItem>
        where TItem : class
    {
        IEqualityComparer<TKey> Comparer { get; }
        TKey GetKeyForItem(TItem item);
    }

    [Flags]
    enum RaisePropertyItems : byte
    {
        Count = 1,
        Items = 2,
    }

    public abstract class KeyedCollectionWithNotify<TKey, TItem> : KeyedCollection<TKey, TItem>, ISupportsKeyedCollectionChanged<TKey, TItem>, INotifyPropertyChanged
        where TItem : class
    {
        readonly CollectionMonitor _monitor = new CollectionMonitor();

        protected KeyedCollectionWithNotify()
            : base()
        { }

        protected KeyedCollectionWithNotify(IEqualityComparer<TKey> comparer)
            : base(comparer)
        { }

        protected KeyedCollectionWithNotify(IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold)
            : base(comparer, dictionaryCreationThreshold)
        { }

        protected override void ClearItems()
        {
            _monitor.CheckReentrancy(_collectionChangedTyped, _collectionChangedUntyped);
            if (Count > 0)
            {
                base.ClearItems();
                RaisePropertyChanged(RaisePropertyItems.Count | RaisePropertyItems.Items);
                RaiseCollectionChanged(new CollectionChangedEventArgs<TItem>(CollectionChange.Reset));
            }
        }

        protected override void InsertItem(int index, TItem item)
        {
            _monitor.CheckReentrancy(_collectionChangedTyped, _collectionChangedUntyped);
            base.InsertItem(index, item);
            RaisePropertyChanged(RaisePropertyItems.Count | RaisePropertyItems.Items);
            RaiseCollectionChanged(new CollectionChangedEventArgs<TItem>(CollectionChange.Add, item, index));
        }

        protected override void RemoveItem(int index)
        {
            _monitor.CheckReentrancy(_collectionChangedTyped, _collectionChangedUntyped);
            TItem t = base[index];
            base.RemoveItem(index);
            RaisePropertyChanged(RaisePropertyItems.Count | RaisePropertyItems.Items);
            RaiseCollectionChanged(new CollectionChangedEventArgs<TItem>(CollectionChange.Remove, t, index));
        }

        protected override void SetItem(int index, TItem item)
        {
            _monitor.CheckReentrancy(_collectionChangedTyped, _collectionChangedUntyped);
            TItem t = base[index];
            base.SetItem(index, item);
            RaisePropertyChanged(RaisePropertyItems.Items);
            RaiseCollectionChanged(new CollectionChangedEventArgs<TItem>(CollectionChange.Replace, t, item, index));
        }

        public void Move(int oldIndex, int newIndex)
        {
            MoveItem(oldIndex, newIndex);
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            _monitor.CheckReentrancy(_collectionChangedUntyped, _collectionChangedTyped);
            TItem t = base[oldIndex];
            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, t);
            RaisePropertyChanged(RaisePropertyItems.Items);
            RaiseCollectionChanged(new CollectionChangedEventArgs<TItem>(CollectionChange.Move, t, newIndex, oldIndex));
        }

        EventHandler<CollectionChangedEventArgs> _collectionChangedUntyped;
        EventHandler<CollectionChangedEventArgs<TItem>> _collectionChangedTyped;

        public event EventHandler<CollectionChangedEventArgs<TItem>> CollectionChanged
        {
            add { _collectionChangedTyped += value; }
            remove { _collectionChangedTyped -= value; }
        }

        event EventHandler<CollectionChangedEventArgs> ISupportsCollectionChanged.CollectionChanged
        {
            add { _collectionChangedUntyped += value; }
            remove { _collectionChangedUntyped -= value; }
        }

        protected void OnCollectionChanged(CollectionChangedEventArgs<TItem> e)
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
        RaisePropertyItems _updateProps;
        CollectionChangedEventArgs<TItem> _change;

        void RaiseCollectionChanged(CollectionChangedEventArgs<TItem> e)
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
                PropertyChanged(this, e);
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
            _updateMode = 1;
            return _monitor.BatchUpdate(DoneUpdate);
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
                CollectionChangedEventArgs<TItem> c = _change;
                _change = null;

                if (c != null)
                    RaiseCollectionChanged(c);
                /* else NOOP batch */
            }
            else
                RaiseCollectionChanged(new CollectionChangedEventArgs<TItem>(CollectionChange.Reset));
        }

        IDisposable ISupportsCollectionChanged.BatchUpdate()
        {
            throw new NotImplementedException();
        }

        TKey ISupportsKeyedCollectionChanged<TKey, TItem>.GetKeyForItem(TItem item)
        {
            return GetKeyForItem(item);
        }
    }
}
