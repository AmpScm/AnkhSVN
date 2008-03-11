using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc
{
	/// <summary>
	/// 
	/// </summary>
	public interface IAnkhSccService
	{
		/// <summary>
		/// Gets a value indicating whether the Ankh Scc service is active
		/// </summary>
		/// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
		bool IsActive { get; }
	}
}
