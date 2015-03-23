using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CollectionMonitor = Ankh.CollectionChangedEventArgs.CollectionMonitor;

namespace Ankh
{
    /// <summary>
    /// Our own implementation of ObservableCollection, compatible with .Net 2.0
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollectionWithNotify<T> : Collection<T>, ISupportsCollectionChanged, INotifyPropertyChanged
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
            _monitor.CheckReentrancy(CollectionChanged);
            base.ClearItems();
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(new CollectionChangedEventArgs(CollectionChange.Reset));
        }

        protected override void InsertItem(int index, T item)
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
            T t = base[index];
            base.RemoveItem(index);
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("Item[]");
            this.OnCollectionChanged(new CollectionChangedEventArgs(CollectionChange.Remove, t, index));
        }

        protected override void SetItem(int index, T item)
        {
            _monitor.CheckReentrancy(CollectionChanged);
            T t = base[index];
            base.SetItem(index, item);
            this.OnPropertyChanged("Item[]");
            this.OnCollectionChanged(new CollectionChangedEventArgs(CollectionChange.Replace, t, item, index));
        }

        public void Move(int oldIndex, int newIndex)
        {
            MoveItem(oldIndex, newIndex);
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            _monitor.CheckReentrancy(CollectionChanged);
            T t = base[oldIndex];
            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, t);
            this.OnPropertyChanged("Item[]");
            this.OnCollectionChanged(new CollectionChangedEventArgs(CollectionChange.Move, t, newIndex, oldIndex));
        }

        public event EventHandler<CollectionChangedEventArgs> CollectionChanged;

        protected void OnCollectionChanged(CollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                using (_monitor.Enter())
                {
                    CollectionChanged(this, e);
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
    }
}
