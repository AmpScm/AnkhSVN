// Copyright 2009 The AnkhSVN Project
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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;

namespace Ankh.Scc.Native
{
    [CLSCompliant(false)]
    public sealed class PropertyBag : IPropertyMap, IDisposable
    {
        readonly IPropertyBag _bag;
        readonly SortedList<string, string> _toWrite = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);

        public PropertyBag(IPropertyBag bag)
        {
            if (bag == null)
                throw new ArgumentNullException("bag");
            _bag = bag;
        }

        /// <summary>
        /// Tries to get the value.
        /// </summary>
        /// <param name="propName">Name of the prop.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool TryGetValue(string propName, out string value)
        {
            if (string.IsNullOrEmpty(propName))
                throw new ArgumentNullException("propName");

            try
            {
                _bag.Read(propName, out var rawValue, null, 0, null);
                value = rawValue as string;
                return value != null;
            }
            catch (Exception ex)
            {
                Trace.TraceError("PropertyBag.TryGetValue failed for property '{0}': {1}", propName, ex);
                value = null;
                return false;
            }
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="propName">Name of the prop.</param>
        /// <param name="value">The value.</param>
        public void SetValue(string propName, string value)
        {
            if (string.IsNullOrEmpty(propName))
                throw new ArgumentNullException("propName");
            else if (value == null)
                throw new ArgumentNullException("value");

            _toWrite[propName] = value;
        }

        public void SetQuoted(string propName, string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            SetValue(propName, Quote(value));
        }

        public bool TryGetQuoted(string propName, out string value)
        {
            if (!TryGetValue(propName, out value))
                return false;

            value = Unquote(value);
            return true;
        }

        public void SetRawValue(string propName, string value)
        {
            // COM IPropertyBag.Write requires a ref object, not a ref string.
            object propertyValue = value;
            _bag.Write(propName, ref propertyValue);
        }

        public void Flush()
        {
            foreach (KeyValuePair<string, string> kv in _toWrite)
            {
                SetRawValue(kv.Key, kv.Value);
            }
            _toWrite.Clear();
        }

        public void Dispose()
        {
            Flush();
        }

        static string Quote(string value)
        {
            return '\"' + value.Replace("\"", "\"\"") + '\"';
        }

        static string Unquote(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            value = value.Trim();

            if (string.IsNullOrEmpty(value))
                return "";

            if (value.Length >= 2 && value[0] == '\"' && value[value.Length - 1] == '\"')
            {
                value = value.Substring(1, value.Length - 2).Replace("\"\"", "\"");
            }

            return value;
        }

        public IEnumerable<string> WrittenKeys
        {
            get { return _toWrite.Keys; }
        }

        public bool WrittenKey(string key)
        {
            return _toWrite.ContainsKey(key);
        }
    }
}
