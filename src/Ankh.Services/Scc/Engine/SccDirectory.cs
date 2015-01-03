using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Ankh.Scc.Engine
{
    public abstract partial class SccDirectory<T> : KeyedCollection<string, T>
        where T : SccItem<T>
    {
        readonly string _fullPath;

        protected SccDirectory(string fullPath)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentNullException("fullPath");

            _fullPath = fullPath;
        }

        protected sealed override string GetKeyForItem(T item)
        {
            return item.FullPath;
        }

        /// <summary>
        /// Gets the full path of the directory
        /// </summary>
        /// <value>The full path.</value>
        public string FullPath
        {
            [DebuggerStepThrough]
            get
            {
                if (Contains(_fullPath))
                    return this[_fullPath].FullPath;
                else
                    return _fullPath;
            }
        }
    }
}
