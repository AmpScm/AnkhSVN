using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.UITypeEditors
{
    public class AdminDirectoryNameTypeConverter : StandardStringsTypeConverter
    {
        public AdminDirectoryNameTypeConverter()
            : base(new string[] { ".svn", "_svn", null }, true)
        {

        }
    }

    // dunno why this is necessary, but it doesn't work to use TypeConverters defined
    // in another assembly inside VS.
    public class NullableStringConverterProxy : NullableStringTypeConverter { }
}
