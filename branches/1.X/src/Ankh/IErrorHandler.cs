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

        /// <summary>
        /// Display an error in the output pane.
        /// </summary>
        void Write( string message, Exception ex, System.IO.TextWriter writer );

        /// <summary>
        /// Log the exception, but take no further action.
        /// </summary>
        void LogException( Exception exception, string message, params object[] args );
    }
}
