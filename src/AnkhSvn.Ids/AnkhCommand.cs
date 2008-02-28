using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace AnkhSvn.Ids
{
	/// <summary>
	/// List of ankh commands
	/// </summary>
	/// <remarks>
	/// <para>New items should be added at the end. unused items should only be obsoleted.</para>
	/// <para>This list of commands is part of the public api for communicating within Visual Studio</para>
	/// </remarks>
	[Guid(AnkhId.CommandSet)]
	public enum AnkhCommand // : int
	{
		[Obsolete]
		None=0,

		// These values live in the same numberspace as the other values within 
		// the command set. So we start countin at this number to make sure we
		// do not reuse values
		CommandFirst = 0x1FFFFFFF,
		
		FileSccMenuUpdateLatest,
		FileSccMenuUpdateSpecific,

		AddItem,
		AddRepositoryRoot,
		AddSolutionToRepository
	}
}
