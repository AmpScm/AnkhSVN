using System;

namespace Ankh
{
	/// <summary>
	/// Represents a class that handles an Ankh error.
	/// </summary>
	public interface IErrorHandler
	{
		void Handle( Exception ex );
	}
}
