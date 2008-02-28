using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace AnkhSvn.Ids
{
	[Guid(AnkhId.CommandSet)]
	public enum AnkhCommandMenu
	{
		[Obsolete]
		None = 0,

		// These values live in the same numberspace as the other values within 
		// the command set. So we start countin at this number to make sure we
		// do not reuse values
		[Obsolete("Don't use this")]
		MenuFirst = 0x5FFFFFFF,

		FileScc,
	}
}
