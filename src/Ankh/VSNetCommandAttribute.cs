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
            [System.Diagnostics.DebuggerStepThrough]
            get{ return name; }
        }

        /// <summary>
        /// The tooltip associated with this command.
        /// </summary>
        public string Tooltip
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return tooltip; }
            [System.Diagnostics.DebuggerStepThrough]
            set{ this.tooltip = value; }
        }

        /// <summary>
        /// The display name of this command.
        /// </summary>
        public string Text
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.text; }

            [System.Diagnostics.DebuggerStepThrough]
            set{ this.text = value; }
        }

        /// <summary>
        /// The ID of the bitmap associated with this command.
        /// </summary>
        public int Bitmap
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.bitmap; }

            [System.Diagnostics.DebuggerStepThrough]
            set{ this.bitmap = value; }
        }
        
        private string name;
        private string text = "This should have been replaced...";
        private string tooltip = "This is a tooltip.";
        private int bitmap = ResourceBitmaps.Default;
    }
}
