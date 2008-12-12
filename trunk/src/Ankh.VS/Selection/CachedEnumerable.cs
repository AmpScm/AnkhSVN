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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Ankh.VS.Selection
{
    sealed class Disposer : IDisposable
    {
        bool _disposed;

        public void Dispose()
        {
            _disposed = true;
        }

        public bool IsDisposed
        {
            get { return _disposed; }
        }
    }
    /// <summary>
    /// Caching IEnumerable wrapper, which only loads whatever is really needed from its inner enumerable, noting
    /// that getting each extra item might be expensive and perhaps unnecessary (as when updating menus)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    sealed class CachedEnumerable<T> : IEnumerable<T>, IDisposable
        where T : class
    {
        readonly List<T> _cache;
        readonly IEnumerator<T> _enumerator;
        readonly Disposer _disposer;
        bool _atTheEnd;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedIEnumerable&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="enumerabled">The inner enumerable.</param>
        public CachedEnumerable(IEnumerable<T> enumerabled, Disposer disposer)
        {
            if (enumerabled == null)
                throw new ArgumentNullException("enumerabled");
            
            _cache = new List<T>();
            _enumerator = enumerabled.GetEnumerator();
            _disposer = disposer ?? new Disposer();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedIEnumerable&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="enumerator">The inner enumerator.</param>
        /// <remarks>The <see cref="CachedEnumerable "/> needs exclusive access to the enumerator</remarks>
        public CachedEnumerable(IEnumerator<T> enumerator)
        {
            if (enumerator == null)
                throw new ArgumentNullException("enumerator");

            _cache = new List<T>();
            _enumerator = enumerator;
        }


        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new Walker(this);
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the next.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="current">The current.</param>
        /// <returns></returns>
        internal bool GetNext(int index, out T value)
        {
            if (index < _cache.Count)
            {
                value = _cache[index];
                return true;
            }
            else if (index == _cache.Count)
            {
                if (!_atTheEnd)
                {
                    if (!_disposer.IsDisposed && _enumerator.MoveNext())
                    {
                        _cache.Add(value = _enumerator.Current);
                        return true;
                    }
                    else
                    {
                        _enumerator.Dispose();
                        _atTheEnd = true;
                    }
                }
            }

            value = null;
            return false;
        }

        #endregion

        sealed class Walker : IEnumerator<T>, IEnumerator
        {
            readonly CachedEnumerable<T> _cache;
            int _n;
            T _current;

            public Walker(CachedEnumerable<T> cache)
            {
                if (cache == null)
                    throw new ArgumentNullException("cache");

                _cache = cache;
            }

            public bool MoveNext()
            {
                return _cache.GetNext(_n++, out _current);
            }

            #region IEnumerator<T> Members

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <value></value>
            /// <returns>The element in the collection at the current position of the enumerator.</returns>
            public T Current
            {
                get { return _current; }
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <value></value>
            /// <returns>The element in the collection at the current position of the enumerator.</returns>
            object IEnumerator.Current
            {
                get { return _current; }
            }

            public void Reset()
            {
                _n = 0;
                _current = default(T);
            }

            #endregion

            #region IDisposable Members

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                _current = default(T);

            }

            #endregion
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!_atTheEnd)
                _enumerator.Dispose();
            _atTheEnd = true;
        }
        #endregion
    }
}
