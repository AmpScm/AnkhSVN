// $Id$
using System;

namespace Ankh
{
	/// <summary>
	/// Describes a menu item in the repository explorer.
	/// </summary>
    [AttributeUsage( AttributeTargets.Class )]
	public class RepositoryExplorerMenuAttribute : Attribute
	{
		public RepositoryExplorerMenuAttribute( string text )
		{
            this.text = text;			
		}

        public string Text
        {
            get{ return this.text; }
        }

        public int Position
        {
            get{ return this.position; }
            set{ this.position = value; }
        }

        private string text;       
        private int position;
	}
}
