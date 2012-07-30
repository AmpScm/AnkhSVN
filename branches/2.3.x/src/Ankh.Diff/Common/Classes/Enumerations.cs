// $Id$
//
// Copyright 2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

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
