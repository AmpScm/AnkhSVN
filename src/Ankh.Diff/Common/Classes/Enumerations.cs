#region Copyright And Revision History

/*---------------------------------------------------------------------------

	Enumerations.cs
	Copyright © 2003 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	5.4.2003	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;

namespace Ankh.Diff
{
	/// <summary>
	/// The operating systems styles supported by the Menees library.
	/// </summary>
	/// <remarks>
	/// These values are ordered so I can compare them to check OS-levels.  I didn't break
	/// Win9x/Me down because I consider them toy OSes inferior to any NT variation.
	/// </remarks>
	public enum OSStyle 
	{ 
		/// <summary>
		/// Windows 95, 98, or Me.
		/// </summary>
		Win9x = 0,
		/// <summary>
		/// NT 4.0
		/// </summary>
		NT4 = 40, 
		/// <summary>
		/// Windows 2000
		/// </summary>
		Windows2000 = 50, 
		/// <summary>
		/// Windows XP
		/// </summary>
		WindowsXP = 51 
	};

	/// <summary>
	/// The supported types of system sounds that can be played by <see cref="Utilities.PlaySound"/>.
	/// </summary>
	public enum SystemSound 
	{ 
		/// <summary>
		/// The default sound.
		/// </summary>
		Default = 0,
		/// <summary>
		/// The question/confirmation sound.
		/// </summary>
		Question = 0x20, 
		/// <summary>
		/// The error sound.
		/// </summary>
		Error = 0x10, 
		/// <summary>
		/// The warning sound.
		/// </summary>
		Warning = 0x30, 
		/// <summary>
		/// The information sound.
		/// </summary>
		Information = 0x40, 
		/// <summary>
		/// A simple sound.
		/// </summary>
		Simple = -1 
	};
}
