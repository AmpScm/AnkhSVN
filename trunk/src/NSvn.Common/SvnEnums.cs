using System;
using System.Collections.Generic;
using System.Text;

namespace NSvn.Common
{
	/// <summary>
	/// Indicates how to recurse subdirectories
	/// </summary>
	public enum Recurse
	{
		/// <summary>
		/// Do not recurse at all
		/// </summary>
		None,

		/// <summary>
		/// Recurse everything
		/// </summary>
		Full,
	}
}
