using System;
using NSvn.Common;

namespace NSvn
{
	/// <summary>
	/// An exception that is thrown if the status for a resource cannot be determined.
	/// </summary>
	[Serializable]
	public class StatusException : SvnException
	{
		public StatusException( string message ) : base( message )
		{
            // empty
		}

        public StatusException( string message, Exception innerException ) : base( message, innerException )
        {
            // empty
        }

        public StatusException()
        {
            // empty
        }
	}
}
