using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Ankh
{
    public class HybridCollection<T> : KeyedCollection<T, T>
    {
        public HybridCollection(IEqualityComparer<T> comparer, int dictionaryCreationTreshold)
            : base(comparer, dictionaryCreationTreshold)
        {
        }

        public HybridCollection(IEqualityComparer<T> comparer)
            : base(comparer)
        {
        }

        public HybridCollection()
        {
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            foreach (T item in items)
            {
                Add(item);
            }
        }

        protected override T GetKeyForItem(T item)
        {
            return item;
        }

        /// <summary>
        /// Adds all items which are not already in this collection to the collection
        /// </summary>
        /// <param name="items">The items to add.</param>
        public void UniqueAddRange(IEnumerable<T> items)
        {
            if(items == null)
                throw new ArgumentNullException("items");

            foreach (T item in items)
            {
                if (!Contains(item))
                    Add(item);
            }
        }
    }    
}
