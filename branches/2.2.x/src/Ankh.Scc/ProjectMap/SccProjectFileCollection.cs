// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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

namespace Ankh.Scc.ProjectMap
{
    sealed class SccProjectFileCollection : KeyedCollection<string, SccProjectFileReference>
    {
        public SccProjectFileCollection()
            : base(StringComparer.OrdinalIgnoreCase, 0)
        {
        }

        protected override string GetKeyForItem(SccProjectFileReference item)
        {
            return item.Filename;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool TryGetValue(string key, out SccProjectFileReference value)
        {
            if (Dictionary != null)
                return Dictionary.TryGetValue(key, out value);

            foreach (SccProjectFileReference p in Items)
            {
                if (Comparer.Equals(GetKeyForItem(p), key))
                {
                    value = p;
                    return true;
                }
            }

            value = null;
            return false;
        }
    }
}
