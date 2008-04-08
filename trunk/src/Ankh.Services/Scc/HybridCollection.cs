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

        protected override T GetKeyForItem(T item)
        {
            return item;
        }
    }    
}
