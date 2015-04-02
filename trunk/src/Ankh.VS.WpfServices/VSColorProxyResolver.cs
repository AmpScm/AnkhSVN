using System;
using System.Windows;

namespace Ankh.VS.WpfServices
{
    sealed class VSColorProxyResolver : ResourceDictionary
    {
        public VSColorProxyResolver()
        {
            // TODO: Load values for all keys
            //foreach(PropertyInfo p in typeof(VSColorProxy).GetProperties(BindingFlags.Public | BindingFlags.Static))
            //{
            //    object v = p.GetValue(null);
            //    Add(p.GetValue(null), v);
            //}
        }

        sealed class VSColorProxyKey : IEquatable<VSColorProxyKey>
        {
            readonly Guid _id;
            readonly string _name;
            readonly VSColorType _key;
            
            public VSColorProxyKey(Guid id, string name, VSColorType key)
            {
                _id = id;
                _name = name;
                _key = key;
            }

            public override int GetHashCode()
            {
                return StringComparer.Ordinal.GetHashCode(_name) ^ (int)_key;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as VSColorProxyKey);
            }

            public bool Equals(VSColorProxyKey other)
            {
                return (other != null) && (other._name == _name) && (other._key == _key) && (other._id == _id);
            }
        }

        internal object MakeKey(Guid id, string name, VSColorType key)
        {
            VSColorProxyKey k = new VSColorProxyKey(id, name, key);

            // Add + resolve value for k

            return k;
        }
    }
}
