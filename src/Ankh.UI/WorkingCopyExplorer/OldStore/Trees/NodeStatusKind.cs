// $Id$
//
// Copyright 2006-2008 The AnkhSVN Project
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
using System.Text;

using SharpSvn;

namespace Ankh
{
    public enum NodeStatusKind
    {
        None = SvnStatus.None,
        Normal = SvnStatus.Normal,
        Added = SvnStatus.Added,
        Deleted = SvnStatus.Deleted,
        Conflicted = SvnStatus.Conflicted,
        Unversioned = SvnStatus.NotVersioned,
        Modified = SvnStatus.Modified,
        Ignored = SvnStatus.Ignored,
        Replaced = SvnStatus.Replaced,
        External = SvnStatus.External,
        Incomplete = SvnStatus.Incomplete,
        Merged = SvnStatus.Merged,
        Missing = SvnStatus.Missing,
        Obstructed = SvnStatus.Obstructed,
        IndividualStatusesConflicting
    }

}
