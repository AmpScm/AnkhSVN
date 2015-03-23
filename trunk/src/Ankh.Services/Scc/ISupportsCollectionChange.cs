using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Ankh
{
    public interface ISupportsCollectionChanged
    {
        event EventHandler<CollectionChangedEventArgs> CollectionChanged;

        IDisposable BatchUpdate();
    }

    public interface ISupportsCollectionChanged<T> : ISupportsCollectionChanged where T : class
    {
        new event EventHandler<CollectionChangedEventArgs<T>> CollectionChanged;
    }

    public enum CollectionChange
    {
        /// <summary>One or more items were added to the collection.</summary>
        Add = 0 /* = NotifyCollectionChangedAction.Add */,
        /// <summary>One or more items were removed from the collection.</summary>
        Remove = 1 /* = NotifyCollectionChangedAction.Remove */,
        /// <summary>One or more items were replaced in the collection.</summary>
        Replace = 2 /* = NotifyCollectionChangedAction.Replace */,
        /// <summary>One or more items were moved within the collection.</summary>
        Move = 3 /* = NotifyCollectionChangedAction.Move */,
        /// <summary>The content of the collection changed dramatically.</summary>
        Reset = 4 /* = NotifyCollectionChangedAction.Reset */,
    }

    /* Our own clone of NotifyCollectionChangedEventArgs */
    public class CollectionChangedEventArgs : EventArgs
    {
        readonly CollectionChange _action;
        readonly object[] _newItems;
        readonly object[] _oldItems;
        readonly int _newStartIndex = -1;
        readonly int _oldStartIndex = -1;

        /// <summary>Initializes for Reset</summary>
        public CollectionChangedEventArgs(CollectionChange action)
        {
            if (action != CollectionChange.Reset)
                throw new ArgumentException();
            _action = action;
        }

        /// <summary>Initializes for Reset, Add or Remove</summary>
        public CollectionChangedEventArgs(CollectionChange action, object changedItem)
            : this(action, changedItem, -1)
        {
        }

        /// <summary>Initializes for Reset, Add or Remove</summary>
        public CollectionChangedEventArgs(CollectionChange action, object changedItem, int index)
        {
            if (action != CollectionChange.Reset
                && action != CollectionChange.Add
                && action != CollectionChange.Remove)
            {
                throw new ArgumentException();
            }
            else if (action != CollectionChange.Reset && changedItem == null)
                throw new ArgumentNullException();

            _action = action;
            if (action == CollectionChange.Add)
            {
                _newItems = MakeArray(changedItem);
                _newStartIndex = index;
            }
            else if (action == CollectionChange.Remove)
            {
                _oldItems = MakeArray(changedItem);
                _oldStartIndex = index;
            }
        }

        /// <summary>Initializes for Replace</summary>
        public CollectionChangedEventArgs(CollectionChange action, object newItem, object oldItem)
            : this(action, newItem, oldItem, -1)
        {

        }

        /// <summary>Initializes for Replace</summary>
        public CollectionChangedEventArgs(CollectionChange action, object newItem, object oldItem, int startingIndex)
        {
            if (action != CollectionChange.Replace)
                throw new ArgumentException();
            else if (newItem == null)
                throw new ArgumentNullException("newItem");
            else if (oldItem == null)
                throw new ArgumentNullException("newItem");

            _action = action;
            _newItems = MakeArray(newItem);
            _oldItems = MakeArray(oldItem);
            _oldStartIndex = _newStartIndex = startingIndex;
        }

        /// <summary>Initializes for Replace</summary>
        public CollectionChangedEventArgs(CollectionChange action, object[] newItems, object[] oldItems)
            : this(action, newItems, oldItems, -1)
        {

        }

        /// <summary>Initializes for Replace</summary>
        public CollectionChangedEventArgs(CollectionChange action, object[] newItems, object[] oldItems, int startingIndex)
        {
            if (action != CollectionChange.Replace)
                throw new ArgumentException();
            else if (newItems == null)
                throw new ArgumentNullException("newItems");
            else if (oldItems == null)
                throw new ArgumentNullException("oldItems");
            _action = action;
            _newItems = newItems;
            _oldItems = oldItems;
            _oldStartIndex = _newStartIndex = startingIndex;
        }

        /// <summary>Initializes for Move</summary>
        public CollectionChangedEventArgs(CollectionChange action, object changedItem, int index, int oldIndex)
        {
            if (action != CollectionChange.Move)
                throw new ArgumentException();
            else if (changedItem == null)
                throw new ArgumentNullException("changedItem");
            else if (index < 0 || oldIndex < 0)
                throw new ArgumentException();
            _action = action;
            _oldItems = _newItems = MakeArray(changedItem);
            _newStartIndex = index;
            _oldStartIndex = oldIndex;
        }

        /// <summary>Initializes for Move</summary>
        public CollectionChangedEventArgs(CollectionChange action, object[] changedItems, int index, int oldIndex)
        {
            if (action != CollectionChange.Move)
                throw new ArgumentException();
            else if (changedItems == null)
                throw new ArgumentNullException("changedItems");
            else if (index < 0 || oldIndex < 0)
                throw new ArgumentException();
            _action = action;
            _oldItems = _newItems = changedItems;
            _newStartIndex = index;
            _oldStartIndex = oldIndex;
        }

        public CollectionChange Action
        {
            get { return _action; }
        }

        public object[] NewItems
        {
            get { return _newItems; }
        }

        public object[] OldItems
        {
            get { return _oldItems; }
        }

        public int NewStartingIndex
        {
            get { return _newStartIndex; }
        }

        public int OldStartingIndex
        {
            get { return _oldStartIndex; }
        }

        internal sealed class CollectionMonitor : IDisposable
        {
            private int _busyCount;
            public bool Busy
            {
                get
                {
                    return this._busyCount > 0;
                }
            }
            public IDisposable Enter()
            {
                this._busyCount++;
                return this;
            }
            public void Dispose()
            {
                this._busyCount--;
            }
            public CollectionMonitor()
            {
            }

            internal void CheckReentrancy<TEventArgs>(EventHandler<TEventArgs> collectionChange)
                where TEventArgs : EventArgs
            {
                if (Busy && collectionChange != null)
                    throw new InvalidOperationException();
            }

            internal void CheckReentrancy<T1, T2>(EventHandler<T1> handlers1, EventHandler<T2> handlers2)
                where T1: EventArgs
                where T2: EventArgs
            {
                if (!Busy)
                    return;
                if (handlers1 != null)
                    throw new InvalidOperationException();
                if (handlers2 != null)
                    throw new InvalidOperationException();
            }

            sealed class BatchUpdateDisposer : IDisposable
            {
                readonly AnkhAction _action;

                public BatchUpdateDisposer(AnkhAction action)
                {
                    _action = action;
                }

                public void Dispose()
                {
                    _action();
                }
            }

            public IDisposable BatchUpdate(AnkhAction doneHandler)
            {
                return new BatchUpdateDisposer(doneHandler);
            }
        }

        internal virtual object[] MakeArray(object item)
        {
            return new object[] { item };
        }
    }

    public class CollectionChangedEventArgs<T> : CollectionChangedEventArgs where T : class
    {
        public CollectionChangedEventArgs(CollectionChange action)
            : base(action)
        {
        }

        /// <summary>Initializes for Reset, Add or Remove</summary>
        public CollectionChangedEventArgs(CollectionChange action, T changedItem)
            :  base(action, changedItem, -1)
        {
        }

        /// <summary>Initializes for Reset, Add or Remove</summary>
        public CollectionChangedEventArgs(CollectionChange action, T changedItem, int index)
            : base(action, changedItem, index)
        {
        }

        /// <summary>Initializes for Replace</summary>
        public CollectionChangedEventArgs(CollectionChange action, T newItem, T oldItem)
            : base(action, newItem, oldItem)
        {

        }

        /// <summary>Initializes for Replace</summary>
        public CollectionChangedEventArgs(CollectionChange action, T newItem, T oldItem, int startingIndex)
            : base(action, newItem, oldItem, startingIndex)
        {
        }

        /// <summary>Initializes for Replace</summary>
        public CollectionChangedEventArgs(CollectionChange action, T[] newItems, T[] oldItems)
            : base(action, newItems, oldItems)
        {

        }

        /// <summary>Initializes for Replace</summary>
        public CollectionChangedEventArgs(CollectionChange action, T[] newItems, T[] oldItems, int startingIndex)
            : base(action, newItems, oldItems, startingIndex)
        {
        }

        /// <summary>Initializes for Move</summary>
        public CollectionChangedEventArgs(CollectionChange action, T changedItem, int index, int oldIndex)
            : base(action, changedItem, index, oldIndex)
        {
        }

        /// <summary>Initializes for Move</summary>
        public CollectionChangedEventArgs(CollectionChange action, T[] changedItems, int index, int oldIndex)
            : base(action, changedItems, index, oldIndex)
        {
        }

        internal sealed override object[] MakeArray(object item)
        {
            return new T[] { (T)item };
        }

        public new T[] NewItems
        {
            get { return (T[])base.NewItems; }
        }

        public new T[] OldItems
        {
            get { return (T[])base.OldItems; }
        }
    }
}
