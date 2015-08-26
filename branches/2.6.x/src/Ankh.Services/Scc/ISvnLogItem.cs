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
using System.Collections.ObjectModel;
using System.Text;
using SharpSvn;
using SharpSvn.Implementation;

namespace Ankh.Scc
{
    public interface ISvnLogItem
    {
        DateTime Time { get; }
        string Author { get; }
        string LogMessage { get; }
        IEnumerable<Ankh.VS.TextMarker> Issues { get; }
        long Revision { get; }
        int Index { get; }

        /// <summary>
        /// List of changed paths for this revision or NULL if that information is not available
        /// </summary>
        KeyedCollection<string, SvnChangeItem> ChangedPaths { get; }

        Uri RepositoryRoot { get; }
    }
}
