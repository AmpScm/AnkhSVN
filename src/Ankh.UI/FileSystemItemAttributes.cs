using System;
using System.Text;

namespace Ankh.UI
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public class TextPropertyAttribute : Attribute
    {

        public TextPropertyAttribute( string text )
        {
            this.text = text;
        }
    
        /// <summary>
        /// The column header for this property.
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }
        }

        /// <summary>
        /// The number of characters this property requires.
        /// </summary>
        public int TextWidth
        {
            get
            {
                return this.textWidth;
            }
            set
            {
                this.textWidth = value;
            }
        }

        /// <summary>
        /// The order this property should be.
        /// </summary>
        public int Order
        {
            get
            {
                return this.order;
            }
            set
            {
                this.order = value;
            }
        }

        public override bool Match( object obj )
        {
            return base.Match( obj );
        }

        private int order = 0;
        private string text;
        private int textWidth = 40;
    }

    [System.AttributeUsage( AttributeTargets.Property, Inherited = true, AllowMultiple = false )]
    public sealed class StateImagePropertyAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconusingattributeclasses.asp

        public StateImagePropertyAttribute()
        {
        }
    }

}
