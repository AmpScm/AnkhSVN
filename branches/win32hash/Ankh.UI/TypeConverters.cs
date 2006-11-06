using System;
using System.Text;
using System.ComponentModel;

namespace Ankh.UI
{
    /// <summary>
    /// Type converter that converts the string (none) to null and vice versa.
    /// </summary>
    public class NullableStringTypeConverter : StringConverter
    {
        public NullableStringTypeConverter()
        {
            Console.WriteLine();
        }

        public override object ConvertFrom( ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value )
        {            
            if ( ((string)value).ToLower() == "(none)" )
            {
                return null;
            }
            else
            {
                return value;
            }
        }

        public override object ConvertTo( ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType )
        {
            if ( value == null )
            {
                return "(none)";
            }
            else
            {
                return value;
            }
        }
    }

 

    /// <summary>
    /// A type converter that shows a dropdown of standard strings, which also can be nullable.
    /// </summary>
    public class StandardStringsTypeConverter : NullableStringTypeConverter
    {
        public StandardStringsTypeConverter( string[] strings )
        {
            this.strings = strings;
        }

        public StandardStringsTypeConverter( string[] strings, bool exclusive ) : this(strings)
        {
            this.exclusive = exclusive;
        }

        public override bool GetStandardValuesSupported( ITypeDescriptorContext context )
        {
            return true;
        }

        public override bool GetStandardValuesExclusive( ITypeDescriptorContext context )
        {
            return this.exclusive;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues( ITypeDescriptorContext context )
        {
            return new StandardValuesCollection( this.strings );
        }

        

        private string[] strings;
        private bool exclusive = false;
    }
}
