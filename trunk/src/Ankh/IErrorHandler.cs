using System;

namespace Ankh
{
	/// <summary>
	/// Represents a class that handles an Ankh error.
	/// </summary>
	public interface IErrorHandler
	{
		void Handle( Exception ex );

        /// <summary>
        /// Send an report about a non-specific error.
        /// </summary>
        void SendReport();
	}
}
