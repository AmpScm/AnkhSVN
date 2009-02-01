// $Id$
//
// Copyright 2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

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

        public HybridCollection(IEnumerable<T> items, IEqualityComparer<T> comparer)
            : base(comparer)
        {
            if (items != null)
                AddRange(items);
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
