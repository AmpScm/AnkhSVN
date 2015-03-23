using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Ankh
{
    public abstract class KeyedCollectionWithNotify<TKey, TItem> : KeyedCollection<TKey, TItem>, ISupportsCollectionChanged, INotifyPropertyChanged
    {
        CollectionChangedEventArgs.SimpleMonitor _monitor = new CollectionChangedEventArgs.SimpleMonitor();
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
            _monitor.CheckReentrancy(CollectionChanged);
            base.ClearItems();
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(new CollectionChangedEventArgs(CollectionChange.Reset));
        }

        protected override void InsertItem(int index, TItem item)
        {
            _monitor.CheckReentrancy(CollectionChanged);
            base.InsertItem(index, item);
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("Item[]");
            this.OnCollectionChanged(new CollectionChangedEventArgs(CollectionChange.Add, item, index));
        }

        protected override void RemoveItem(int index)
        {
            _monitor.CheckReentrancy(CollectionChanged);
            TItem t = base[index];
            base.RemoveItem(index);
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("Item[]");
            this.OnCollectionChanged(new CollectionChangedEventArgs(CollectionChange.Remove, t, index));
        }

        protected override void SetItem(int index, TItem item)
        {
            _monitor.CheckReentrancy(CollectionChanged);
            TItem t = base[index];
            base.SetItem(index, item);
            this.OnPropertyChanged("Item[]");
            this.OnCollectionChanged(new CollectionChangedEventArgs(CollectionChange.Replace, t, item, index));
        }

        public event EventHandler<CollectionChangedEventArgs> CollectionChanged;

        protected void OnCollectionChanged(CollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, e);
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }
}
