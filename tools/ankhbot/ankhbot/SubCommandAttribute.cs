using System;
using System.Collections.Generic;
using System.Text;

namespace AnkhBot
{
	[AttributeUsage(AttributeTargets.Method)]
	class SubCommandAttribute : Attribute
	{
		public SubCommandAttribute( string name )
		{
			this.name = name;
		}

		public string Name
		{
			get { return name; }
		}

		private string name;	
	}
}
