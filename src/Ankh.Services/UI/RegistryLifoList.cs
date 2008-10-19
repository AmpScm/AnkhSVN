using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using Ankh.VS;
using Ankh.UI;
using System.Globalization;

namespace Ankh.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class RegistryLifoList : AnkhService, ICollection<string>, IEnumerable<KeyValuePair<string, string>>
    {
        readonly string _name;
        readonly int _defaultSize;
        bool _allowWhiteSpace;
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryLifoList"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultSize">The default size.</param>
        public RegistryLifoList(IAnkhServiceProvider context, string name, int defaultSize)
            : base(context)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            else if (defaultSize < 1)
                defaultSize = 16;

            _name = "FifoTables\\" + name;
            _defaultSize = defaultSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryLifoList"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultSize">The default size.</param>
        /// <param name="allowWhitespace">if set to <c>true</c> [allow whitespace].</param>
        public RegistryLifoList(IAnkhServiceProvider context, string name, int defaultSize, bool allowWhitespace)
            : this(context, name, defaultSize)
        {
            _allowWhiteSpace = allowWhitespace;
        }

        IAnkhConfigurationService ConfigService
        {
            get { return GetService<IAnkhConfigurationService>(); }
        }

        RegistryKey OpenFifoKey(bool readOnly)
        {
            return ConfigService.OpenUserInstanceKey(_name);
        }

        public bool AllowWhitespace
        {
            get { return _allowWhiteSpace; }
        }

        public void Add(string item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            else if (!_allowWhiteSpace && string.IsNullOrEmpty(item))
                return; // Don't add whitepace

            using (RegistryKey key = OpenFifoKey(false))
            {
                int nSize;
                int nPos;
                if (!TryGetIntValue(key, "_size", out nSize) || nSize < 1)
                {
                    nSize = _defaultSize;
                    key.SetValue("_size", _defaultSize);
                }

                // This gives a very tiny race condition if used at the same time in 
                // two VS instances.
                // We ignore this as it is just UI helper code and a user can't edit
                //  two windows at the same time
                if (!TryGetIntValue(key, "_pos", out nPos) || (nPos < 0) || (nPos >= nSize))
                {
                    nPos = 0;
                }

                if (nPos < 0 || nPos >= nSize)
                {
                    List<string> names = new List<string>(key.GetValueNames());

                    int nX = nSize;
                    for (int i = nSize - 1; i >= 0; i--)
                    {
                        if (names.Contains("#" + i.ToString(CultureInfo.InvariantCulture)))
                            break;

                        nX = i;
                    }

                    if (nX < nSize)
                        nPos = nX;
                    else
                        nPos = 0;
                }


                nPos++;

                if (nPos > nSize)
                    nPos = 0;

                key.SetValue("_pos", nPos);
                key.SetValue("#" + nPos.ToString(CultureInfo.InvariantCulture), item);
            }
        }

        public void Clear()
        {
            using (RegistryKey key = OpenFifoKey(false))
            {
                key.SetValue("_pos", 0);

                string[] names = key.GetValueNames();
                Array.Sort<string>(names);

                foreach (string name in names)
                {
                    if (name.StartsWith("#"))
                        key.DeleteValue(name);
                }
            }
        }

        public bool Contains(string item)
        {
            using (RegistryKey key = OpenFifoKey(false))
            {
                foreach (string name in key.GetValueNames())
                {
                    if (name.StartsWith("#"))
                    {
                        if (String.Equals(item, key.GetValue(name) as string))
                            return true;
                    }
                }
            }
            return false;
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            foreach (string value in this)
            {
                array[arrayIndex++] = value;
            }
        }

        int ICollection<string>.Count
        {
            get
            {
                int n = 0;

                foreach (string value in this)
                {
                    n++;
                }

                return n;
            }
        }

        bool ICollection<string>.IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(string item)
        {
            foreach (KeyValuePair<string, string> kv in (IEnumerable<KeyValuePair<string, string>>)this)
            {
                if (kv.Value == item)
                {
                    using (RegistryKey key = OpenFifoKey(false))
                    {
                        key.DeleteValue(kv.Key);
                        return true;
                    }
                }
            }
            return false;
        }

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            foreach (KeyValuePair<string, string> kv in (IEnumerable<KeyValuePair<string, string>>)this)
            {
                yield return kv.Value;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        bool TryGetIntValue(RegistryKey key, string name, out int value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            object val = key.GetValue(name);

            value = -1;

            if (val != null)
            {
                if (val is int)
                {
                    value = (int)val;
                    return true;
                }
                else if (int.TryParse(value.ToString(), out value))
                    return true;
            }

            return false;
        }

        #region IEnumerable<KeyValuePair<string,string>> Members

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            using (RegistryKey key = OpenFifoKey(true))
            {
                int nSize;
                int nPos;

                if (!TryGetIntValue(key, "_size", out nSize) || nSize < 1)
                {
                    nSize = _defaultSize;
                }

                // This gives a very tiny race condition if used at the same time in 
                // two VS instances.
                // We ignore this as it is just UI helper code and a user can't edit
                //  two windows at the same time
                if (!TryGetIntValue(key, "_pos", out nPos) || (nPos < 0) || (nPos >= nSize))
                {
                    nPos = 0;
                }

                HybridCollection<string> hs = new HybridCollection<string>();
                hs.AddRange(key.GetValueNames());

                for (int i = 0; i < nSize; i++)
                {
                    int n = nPos - i;
                    while (n < 0)
                        n += nSize;

                    string s = "#" + n.ToString(CultureInfo.InvariantCulture);

                    if (hs.Contains(s))
                    {
                        string v = key.GetValue(s) as string;

                        if (v != null && (_allowWhiteSpace || !string.IsNullOrEmpty(v)))
                            yield return new KeyValuePair<string, string>(s, v);
                    }
                }
            }
        }

        #endregion
    }
}
