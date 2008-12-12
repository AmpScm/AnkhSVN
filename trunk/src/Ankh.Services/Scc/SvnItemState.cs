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

using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc
{
    /// <summary>
    /// This flag enum specifies the full UI state of a <see cref="SvnItem"/> in all live updating windows
    /// </summary>
    /// <remarks>
    /// It is a superset of the <see cref="PendingChangeKind"/> and the <see cref="AnkhGlyph"/>. 
    /// Those two values can be 100% calculated from just this state
    /// </remarks>
    [Flags]
    public enum SvnItemState
    {
        #region State Flags

        /// <summary>
        /// None; not calculated
        /// </summary>
        None                    = 0,

        /// <summary>
        /// File is on disk
        /// </summary>
        Exists                  = 0x000001,

        /// <summary>
        /// File is in the current open solution
        /// </summary>
        InSolution              = 0x000002,

        /// <summary>
        /// File is versioned in a working copy
        /// </summary>
        Versioned               = 0x000004,

        /// <summary>
        /// File is locked locally
        /// </summary>
        HasLockToken            = 0x000008,

        /// <summary>
        /// File is of the wrong kind
        /// </summary>
        Obstructed              = 0x000010,

        /// <summary>
        /// Item is versioned or below a versioned directory
        /// </summary>
        Versionable             = 0x000020,

        /// <summary>
        /// Gets a boolean indicating whether the item is a file
        /// </summary>
        /// <remarks>Contains the on disk status.. If obstructed this does not match</remarks>
        IsDiskFile              = 0x000040,

        /// <summary>
        /// Gets a boolean indicating whether the item is a directory
        /// </summary>
        IsDiskFolder            = 0x000080,

        /// <summary>
        /// 
        /// </summary>
        ReadOnly                = 0x000100,

        /// <summary>
        /// The file is marked as dirty by the editor that has the file open
        /// </summary>
        DocumentDirty           = 0x000200,

        /// <summary>
        /// Somehow modified in subversion
        /// </summary>
        SvnDirty                = 0x000800,

        #endregion

        #region SvnStates (When Versioned is set)

        /// <summary>
        /// The file is modified on disk
        /// </summary>
        Modified                = 0x001000,
        /// <summary>
        /// The properties on disk are dirty
        /// </summary>
        PropertyModified        = 0x002000,
        /// <summary>
        /// The file is scheduled for addition
        /// </summary>
        Added                   = 0x004000,
        /// <summary>
        /// The file has a copy origin
        /// </summary>
        HasCopyOrigin           = 0x008000,
        /// <summary>
        /// The file is scheduled for deletion
        /// </summary>
        Deleted                 = 0x010000,
        /// <summary>
        /// 
        /// </summary>
        Replaced                = 0x020000,
        /// <summary>
        /// The file must be locked before editting
        /// </summary>
        MustLock                = 0x040000,

        /// <summary>
        /// 
        /// </summary>
        HasProperties           = 0x080000,

        /// <summary>
        /// The content is marked as conflicted
        /// </summary>
        ContentConflicted       = 0x100000,
        /// <summary>
        /// The properties are marked as conflicted
        /// </summary>
        PropertiesConflicted    = 0x200000,

        /*/// <summary>
        /// The SvnItem is part of a tree conflict
        /// </summary>
        InTreeConflict          = 0x400000,*/


        /// <summary>
        /// 
        /// </summary>
        Ignored                   =  0x800000,
        #endregion

        IsNested                  = 0x1000000,

        IsTextFile                = 0x2000000,


    }
}
