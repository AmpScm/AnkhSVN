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

namespace Ankh
{
    public static class EnumTools
    {
        /// <summary>
        /// Gets the first item from an IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static T GetFirst<T>(IEnumerable<T> item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            
            foreach (T i in item)
            {
                return i;
            }

            return default(T);
        }

        /// <summary>
        /// Gets the first item from an IEnumerable or null if the item has 0 or more that 1 item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static T GetSingle<T>(IEnumerable<T> item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            T first = default(T);
            bool next = false;
            foreach (T i in item)
            {
                if (next)
                    return default(T);

                first = i;
                next = true;
            }

            return first;
        }

        public static bool IsNullOrEmpty<T>(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return true;

            foreach (T t in enumerable)
                return false;
            return true;
        }

        public static bool IsEmpty<T>(IEnumerable<T> enumerable)
        {
            if (enumerable == null) 
                throw new ArgumentNullException("enumerable");

            foreach(T t in enumerable)
                return false;
            return true;
        }
    }
}
