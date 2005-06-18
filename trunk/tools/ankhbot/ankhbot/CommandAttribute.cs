using System;
using System.Collections.Generic;
using System.Text;

namespace AnkhBot
{
	[AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=true)]
	class CommandAttribute : Attribute
	{
		public CommandAttribute( string command )
		{
			this.command = command;
		}

		public string Command
		{
			get { return command; }
		}

		private string command;	
	}
}
