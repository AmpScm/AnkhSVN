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
using SharpSvn;
using System.Diagnostics;

namespace Ankh.UI
{
    public class PathSelectorResult
    {
        readonly bool _succeeded;
        readonly List<SvnItem> _selection;
        SvnDepth _depth = SvnDepth.Unknown;
        SvnRevision _start;
        SvnRevision _end;

        public PathSelectorResult(bool succeeded, IEnumerable<SvnItem> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            _succeeded = succeeded;
            _selection = new List<SvnItem>(items);
        }

        public SvnDepth Depth
        {
            [DebuggerStepThrough]
            get { return _depth; }
            set { _depth = value; }
        }

        public SvnRevision RevisionStart
        {
            [DebuggerStepThrough]
            get { return _start; }
            set { _start = value; }
        }

        public SvnRevision RevisionEnd
        {
            [DebuggerStepThrough]
            get { return _end; }
            set { _end = value; }
        }


        public IList<SvnItem> Selection
        {
            [DebuggerStepThrough]
            get { return _selection; }
        }

        public bool Succeeded
        {
            [DebuggerStepThrough]
            get { return _succeeded; }
        }
    }
}
