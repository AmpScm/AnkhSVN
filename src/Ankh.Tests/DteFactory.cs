using System;
using System.Reflection;

namespace Ankh.Tests
{
	/// <summary>
	/// Responsible for creating DTE objects.
	/// </summary>
	public class DteFactory
	{      

		private DteFactory( string progId )
		{
			this.progId = progId;
		}

        public static DteFactory Create2002()
        {
            return new DteFactory( "VisualStudio.DTE.7" );
        }

        public static DteFactory Create2003()
        {
            return new DteFactory( "VisualStudio.DTE.7.1" );
        }

        public EnvDTE._DTE Create()
        {
            Type t = Type.GetTypeFromProgID( this.progId, true );
            return (EnvDTE._DTE)Activator.CreateInstance( t );
        }

        string progId;
	}
}
