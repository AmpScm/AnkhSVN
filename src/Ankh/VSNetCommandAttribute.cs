// $Id$
using System;

namespace Ankh
{
    /// <summary>
    /// An attribute that describes a VS.NET command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class VSNetCommandAttribute : Attribute
    {
        public VSNetCommandAttribute( string name )
        {
            this.name = name;
        }

        /// <summary>
        /// The registered name of the command.
        /// </summary>
        public string Name
        {
            get{ return name; }
        }

        /// <summary>
        /// The tooltip associated with this command.
        /// </summary>
        public string Tooltip
        {
            get{ return tooltip; }
            set{ this.tooltip = value; }
        }

        /// <summary>
        /// The display name of this command.
        /// </summary>
        public string Text
        {
            get{ return this.text; }
            set{ this.text = value; }
        }

        /// <summary>
        /// The ID of the bitmap associated with this command.
        /// </summary>
        public int Bitmap
        {
            get{ return this.bitmap; }
            set{ this.bitmap = value; }
        }
        
        private string name;
        private string text = "This should have been replaced...";
        private string tooltip = "This is a tooltip.";
        private int bitmap = ResourceBitmaps.Default;
    }
}
