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
        readonly IList _newItems;
        readonly IList _oldItems;
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
                _newItems = new object[] { changedItem };
                _newStartIndex = index;
            }
            else if (action == CollectionChange.Remove)
            {
                _oldItems = new object[] { changedItem };
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
            if (action == CollectionChange.Add)
            {
                _newItems = new object[] { newItem };
                _newStartIndex = startingIndex;
            }
            else if (action == CollectionChange.Remove)
            {
                _oldItems = new object[] { oldItem };
                _oldStartIndex = startingIndex;
            }
        }

        /// <summary>Initializes for Replace</summary>
        public CollectionChangedEventArgs(CollectionChange action, IList newItems, IList oldItems)
            : this(action, newItems, oldItems, -1)
        {

        }

        /// <summary>Initializes for Replace</summary>
        public CollectionChangedEventArgs(CollectionChange action, IList newItems, IList oldItems, int startingIndex)
        {
            if (action != CollectionChange.Replace)
                throw new ArgumentException();
            else if (newItems == null)
                throw new ArgumentNullException("newItems");
            else if (oldItems == null)
                throw new ArgumentNullException("oldItems");
            _action = action;
            _newItems = ArrayList.ReadOnly(newItems);
            _oldItems = ArrayList.ReadOnly(oldItems);
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
            _oldItems = _newItems = new object[] { changedItem };
            _newStartIndex = index;
            _oldStartIndex = oldIndex;
        }

        /// <summary>Initializes for Move</summary>
        public CollectionChangedEventArgs(CollectionChange action, IList changedItems, int index, int oldIndex)
        {
            if (action != CollectionChange.Move)
                throw new ArgumentException();
            else if (changedItems == null)
                throw new ArgumentNullException("changedItems");
            else if (index < 0 || oldIndex < 0)
                throw new ArgumentException();
            _action = action;
            _oldItems = _newItems = ArrayList.ReadOnly(changedItems);
            _newStartIndex = index;
            _oldStartIndex = oldIndex;
        }

        public CollectionChange Action
        {
            get { return _action; }
        }

        public IList NewItems
        {
            get { return _newItems; }
        }

        public IList OldItems
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

        internal class CollectionMonitor : IDisposable
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

            internal void CheckReentrancy(EventHandler<CollectionChangedEventArgs> collectionChange)
            {
                if (Busy && collectionChange != null)
                    throw new InvalidOperationException();
            }
        }
    }
}
