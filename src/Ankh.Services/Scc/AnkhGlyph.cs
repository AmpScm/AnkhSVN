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
    /// This enum specified the usage of glyph icons by ankh
    /// </summary>
    /// <remarks>This enum contains 16 members which should map the VsStateIcon class if possible</remarks>
    public enum AnkhGlyph
    {
        /// <summary>
        /// Not supported / No Icon / Never visible (STATEICON_NOSTATEICON /0x0)
        /// </summary>
        None, // Icon 0 is drawn as icon 5

        /// <summary>
        /// The file must be locked before editing is allowed (STATEICON_CHECKEDIN /0x1)
        /// </summary>
        MustLock,

        /// <summary>
        /// Path is versioned and modified (STATEICON_CHECKEDOUT /0x2)
        /// </summary>
        Modified,

        /// <summary>
        /// File is deleted from scc (STATEICON_ORPHANED /0x3)
        /// </summary>
        Deleted,

        /// <summary>
        /// Item is versioned and dirty in memory (STATEICON_EDITABLE /0x4)
        /// </summary>
        FileDirty,

        /// <summary>
        /// Blank Icon / No Icon (STATEICON_BLANK /0x5)
        /// </summary>
        /// <remarks>This icon is applied to all tree items which don't retrieve scc status</remarks>
        Blank, // Icon 5 must be blank as it is used on all items without glyph

        /// <summary>
        /// Item is versioned and unmodified (STATEICON_READONLY /0x6)
        /// </summary>
        Normal,

        /// <summary>
        /// File is versioned; but is not available on disk (STATEICON_DISABLED /0x7)
        /// </summary>
        FileMissing,

        /// <summary>
        /// File is versioned but was inserted from an other location (STATEICON_CHECKEDOUTEXCLUSIVE /0x8)
        /// </summary>
        CopiedOrMoved,        

        /// <summary>
        /// File is versioned and locally locked but not modified (STATEICON_CHECKEDOUTSHAREDOTHER /0x9)
        /// </summary>
        LockedNormal,        

        /// <summary>
        /// File is versioned, locally locked and modified (STATEICON_CHECKEDOUTEXCLUSIVEOTHER /0xA)
        /// </summary>
        LockedModified,        

        /// <summary>
        /// File is marked as explicitly ignored (STATEICON_EXCLUDEDFROMSCC /0xB)
        /// </summary>
        Ignored,        

        /// <summary>
        /// File has been added but was never committed before (Last+1 /0xC)
        /// </summary>        
        Added,        

        /// <summary>
        /// File has not been added yet, but should be before committing (Last+2 /0xD)
        /// </summary>
        /// <remarks>Currently generated internally by the Scc provider</remarks>
        ShouldBeAdded,
        
        /// <summary>
        /// File is in conflict; must be resolved before continuing (Last+3 /0xE)
        /// </summary>
        InConflict,

        /// <summary>
        /// One of the Scc special/hidden files below this file is modified (Last+4 /0xF)
        /// </summary>
        ChildChanged
    }
}
