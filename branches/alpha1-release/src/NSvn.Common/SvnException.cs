// $Id$
using System;

namespace NSvn.Common
{
	/// <summary>
	/// Base class for all exceptions thrown from NSvn.
	/// </summary>
	public class SvnException : ApplicationException
	{
		public SvnException()
		{			
		}

        public SvnException( string message ) : base( message )
        {
        }

        public SvnException( string message, Exception innerException ) : 
            base( message, innerException )
        {
        }
	}
}
