// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
        Exists                  = 0x00000001,

        /// <summary>
        /// File is in the current open solution
        /// </summary>
        InSolution              = 0x00000002,

        /// <summary>
        /// File is versioned in a working copy
        /// </summary>
        Versioned               = 0x00000004,

        /// <summary>
        /// File is locked locally
        /// </summary>
        HasLockToken            = 0x00000008,

        /// <summary>
        /// File is of the wrong kind
        /// </summary>
        Obstructed              = 0x00000010,

        /// <summary>
        /// Item is versioned or below a versioned directory
        /// </summary>
        Versionable             = 0x00000020,

        /// <summary>
        /// Gets a boolean indicating whether the item is a file
        /// </summary>
        /// <remarks>Contains the on disk status.. If obstructed this does not match</remarks>
        IsDiskFile              = 0x00000040,

        /// <summary>
        /// Gets a boolean indicating whether the item is a directory
        /// </summary>
        IsDiskFolder            = 0x00000080,

        /// <summary>
        /// The node is read only on disk
        /// </summary>
        ReadOnly                = 0x00000100,

        /// <summary>
        /// The file is marked as dirty by the editor that has the file open
        /// </summary>
        DocumentDirty           = 0x00000200,

        /// <summary>
        /// Somehow modified in subversion
        /// </summary>
        SvnDirty                = 0x00000800,

        #endregion

        #region SvnStates (When Versioned is set)

        /// <summary>
        /// The node is modified on disk
        /// </summary>
        Modified                = 0x00001000,
        /// <summary>
        /// The properties on disk are dirty
        /// </summary>
        PropertyModified        = 0x00002000,
        /// <summary>
        /// The node is scheduled for addition
        /// </summary>
        Added                   = 0x00004000,
        /// <summary>
        /// The node has a copy origin
        /// </summary>
        HasCopyOrigin           = 0x00008000,
        /// <summary>
        /// The node is scheduled for deletion
        /// </summary>
        Deleted                 = 0x00010000,
        /// <summary>
        /// The node is replaced
        /// </summary>
        Replaced                = 0x00020000,
        /// <summary>
        /// The file must be locked before editting
        /// </summary>
        MustLock                = 0x00040000,

        /// <summary>
        /// The node has properties
        /// </summary>
        HasProperties           = 0x00080000,

        /// <summary>
        /// The content is marked as conflicted
        /// </summary>
        ContentConflicted       = 0x00100000,
        /// <summary>
        /// The properties are marked as conflicted
        /// </summary>
        PropertiesConflicted    = 0x00200000,

        /*/// <summary>
        /// The SvnItem is part of a tree conflict
        /// </summary>
        InTreeConflict          = 0x00400000,*/

        /// <summary>
        /// The item is marked as ignored in subversion
        /// </summary>
        Ignored                 = 0x00800000,
        #endregion

        /// <summary>
        /// The item is the root of a nested working copy
        /// </summary>
        IsNested                = 0x01000000,

        /// <summary>
        /// The item is a textfile
        /// </summary>
        IsTextFile              = 0x02000000,

        /// <summary>
        /// The item is (part of) the administrative area
        /// </summary>
        IsAdministrativeArea    = unchecked((int)0x80000000)
    }
}
