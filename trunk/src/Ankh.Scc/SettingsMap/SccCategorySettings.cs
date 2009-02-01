using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc.SettingMap
{
    class SccCategorySettings
    {
        readonly SortedList<string, string> _props = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);
        readonly AnkhSccSettingStorage _store;
        readonly string _id;

        public SccCategorySettings(AnkhSccSettingStorage store, string id)
        {
            if (store == null)
                throw new ArgumentNullException("store");
            else if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id");

            _store = store;
            _id = id;

            store.AddCategory(this);
        }

        public string CategoryId
        {
            get { return _id; }
        }

        public IDictionary<string, string> Properties
        {
            get { return _props; }
        }
    }
}
