using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Ankh.Scc.ProjectMap
{
    class SccProjectFileCollection : KeyedCollection<string, SccProjectFileReference>
    {
        public SccProjectFileCollection()
            : base(StringComparer.OrdinalIgnoreCase, 0)
        {
        }

        protected override string GetKeyForItem(SccProjectFileReference item)
        {
            return item.Filename;
        }
    }
}
