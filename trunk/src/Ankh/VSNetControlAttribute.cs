// $Id$
using System;

namespace Ankh
{
    /// <summary>
    /// An attribute used to describe where a command appears in the VS.NET IDE.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class VSNetControlAttribute : Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commandBar">The name of the command bar.</param>
        public VSNetControlAttribute( string commandBar )
        {	
            this.commandBar = commandBar;
        }

        /// <summary>
        /// The name of the commandbar where the command should be placed.
        /// </summary>
        public string CommandBar
        {
            get{ return this.commandBar; }
        }


        /// <summary>
        /// The position in the command bar.
        /// </summary>
        public int Position
        {
            get{ return this.position; }
            set{ this.position = value; }
        }

        private string commandBar;
        private int position;
    }
}
