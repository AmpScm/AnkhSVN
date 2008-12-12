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
using Ankh.Scc;
using System.ComponentModel;
using Ankh.Scc.UI;

namespace Ankh.UI.Annotate
{
    class AnnotateSection : AnkhPropertyGridItem, IAnnotateSection
    {
        readonly SvnBlameEventArgs _args;
        readonly SvnOrigin _origin;
        long _endLine;

        // BH: TODO: We should copy the values to release the cached lines in the SvnBlameEventArgs

        internal bool Hovered;

        public AnnotateSection(SvnBlameEventArgs blameArgs, SvnOrigin origin)
        {
            _args = blameArgs;
            _endLine = _args.LineNumber;
            _origin = origin;
        }

        [Category("Subversion")]
        public long Revision
        {
            get { return _args.Revision; }
        }

        [Category("Subversion")]
        public string Author
        {
            get { return _args.Author; }
        }

        [Category("Subversion")]
        public DateTime Time
        {
            get { return _args.Time.ToLocalTime(); }
        }

        internal int StartLine
        {
            get { return (int)_args.LineNumber; }
        }

        internal int EndLine
        {
            get { return (int)_endLine; }
            set { _endLine = value; }
        }

        [Browsable(false)]
        public SvnOrigin Origin
        {
            get { return _origin; }
        }

        protected override string ClassName
        {
            get { return "Annotate Section"; }
        }

        protected override string ComponentName
        {
            get { return Origin.Target.FileName; }
        }
    }
}
