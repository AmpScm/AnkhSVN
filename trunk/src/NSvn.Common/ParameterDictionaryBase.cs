using System;
using System.Collections;

namespace NSvn.Common
{
	/// <summary>
	/// This class is intended for internal use.
	/// </summary>
	public abstract class ParameterDictionaryBase : IDictionary
	{
        #region Implementation of IDictionary
        public System.Collections.IDictionaryEnumerator GetEnumerator()
        {
            return this.InnerDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.InnerDictionary.GetEnumerator();
        }

        public void Remove(object key)
        {
            this.InnerDictionary.Remove( key );
        
        }
        public bool Contains(object key)
        {
            return this.InnerDictionary.Contains( key );
        }
        public void Clear()
        {
            this.InnerDictionary.Clear();
        }
        public void Add(object key, object value)
        {
            this.InnerDictionary.Add( key, value );
        }
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public object this[object key]
        {
            get
            {
                return this.InnerDictionary[key];
            }
            set
            {
                this.InnerDictionary[key] = value;
            }
        }

        public System.Collections.ICollection Values
        {
            get
            {
                return this.InnerDictionary.Values;
            }
        }

        public System.Collections.ICollection Keys
        {
            get
            {
                return this.InnerDictionary.Keys;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return this.InnerDictionary.IsFixedSize;
            }
        }
        #endregion

        #region Implementation of ICollection
        public void CopyTo(System.Array array, int index)
        {
            this.InnerDictionary.CopyTo(array, index);
        }

        public bool IsSynchronized
        {
            get
            {
                return this.InnerDictionary.IsSynchronized;
            }
        }

        public int Count
        {
            get
            {
                return this.InnerDictionary.Count;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this.InnerDictionary.SyncRoot;
            }
        }
        #endregion

        protected abstract IDictionary InnerDictionary
        {
            get;
        }
    }
}
