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
    /// 
    /// </summary>
    public enum PendingChangeKind
    {
        /// <summary>
        /// Nothing modified
        /// </summary>
        None,
        /// <summary>
        /// The file is new and should be added to Scc
        /// </summary>
        New,
        /// <summary>
        /// The item is modified
        /// </summary>
        Modified,
        /// <summary>
        /// A property on the item is modified
        /// </summary>
        PropertyModified,
        /// <summary>
        /// The item is replaced
        /// </summary>
        Replaced,
        /// <summary>
        /// The item was copied from another location
        /// </summary>
        Copied,
        /// <summary>
        /// The item was added to scc
        /// </summary>
        Added,
        /// <summary>
        /// The item was deleted
        /// </summary>
        Deleted,
        /// <summary>
        /// The item was deleted on disk, but not in scc
        /// </summary>
        Missing,
        /// <summary>
        /// The item is in conflict. The conflict must be resolved before continuing
        /// </summary>
        Conflicted,
        /// <summary>
        /// The properties of the node are in conflict
        /// </summary>
        PropertyConflicted,
        /// <summary>
        /// The item is obstructed by an item of the wrong kind
        /// </summary>
        Obstructed,
        /// <summary>
        /// The item is unmodified but open and dirty in an editor
        /// </summary>
        EditorDirty,

        /// <summary>
        /// The item is locked locally
        /// </summary>
        LockedOnly,

        /// <summary>
        /// The item is in incomplete state
        /// </summary>
        Incomplete,


        /// <summary>
        /// The node has an invalid casing (FileName -> FILEname)
        /// </summary>
        WrongCasing

    }
}
