using System;

namespace Ankh.Commands
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
        
        private string name;
        private string text = "This should have been replaced...";
        private string tooltip = "This is a tooltip.";
	}
}
