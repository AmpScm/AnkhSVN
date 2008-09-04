#region Copyright And Revision History

/*---------------------------------------------------------------------------

	Enumerations.cs
	Copyright (c) 2003 Bill Menees.  All rights reserved.
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
