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
using System.ComponentModel;
using System.Globalization;
using Ankh.Scc;
using Ankh.Scc.UI;
using SharpSvn;

namespace Ankh.UI.Annotate
{
    /// <summary>
    /// 
    /// </summary>
    class AnnotateRegion
    {
        readonly AnnotateSource _source;
        readonly int _startLine;
        int _endLine;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnotateRegion"/> class.
        /// </summary>
        /// <param name="startLine">The start line.</param>
        /// <param name="endLine">The end line.</param>
        /// <param name="source">The source.</param>
        public AnnotateRegion(int line, AnnotateSource source)
        {
            if(source == null)
                throw new ArgumentNullException("source");

            _source = source;
            _startLine = _endLine = line;
        }

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>The source.</value>
        public AnnotateSource Source
        {
            get { return _source; }
        }

        public int StartLine
        {
            get { return _startLine; }
        }

        /// <summary>
        /// Gets the end line.
        /// </summary>
        /// <value>The end line.</value>
        public int EndLine
        {
            get { return _endLine; }
            internal set { _endLine = value; }
        }

        #region Internal State
        internal bool Hovered;
        #endregion
    }

    class AnnotateSource : AnkhPropertyGridItem, IAnnotateSection, ISvnRepositoryItem, ISvnLogItem
    {
        readonly SvnBlameEventArgs _args;
        readonly SvnOrigin _origin;
        string _logMessage;

        public AnnotateSource(SvnBlameEventArgs blameArgs, SvnOrigin origin)
        {
            _args = blameArgs;
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

        [Browsable(false)]
        public SvnOrigin Origin
        {
            get { return _origin; }
        }

        [Browsable(false)]
        public string LogMessage
        {
            get
            {
                if (_logMessage == null)
                {
                    if (_args.RevisionProperties != null && _args.RevisionProperties.Contains(SvnPropertyNames.SvnLog))
                    {
                        _logMessage = _args.RevisionProperties[SvnPropertyNames.SvnLog].StringValue ?? "";
                    }
                    else
                        _logMessage = "";
                }
                return _logMessage;
            }
        }

        protected override string ClassName
        {
            get { return string.Format(CultureInfo.InvariantCulture, "r{0}", Revision); }
        }

        protected override string ComponentName
        {
            get { return Origin.Target.FileName; }
        }        

        #region ISvnRepositoryItem Members

        Uri ISvnRepositoryItem.Uri
        {
            get { return Origin.Uri; }
        }

        SvnNodeKind ISvnRepositoryItem.NodeKind
        {
            get { return SvnNodeKind.File; }
        }

        SvnRevision ISvnRepositoryItem.Revision
        {
            get { return Revision >= 0 ? Revision : SvnRevision.Working; }
        }

        public void RefreshItem(bool refreshParent)
        {
            // Ignore
        }

        #endregion Members

        #region ISvnLogItem
        IEnumerable<VS.TextMarker> ISvnLogItem.Issues
        {
            get { return new VS.TextMarker[0]; }
        }

        int ISvnLogItem.Index
        {
            get { return -1; }
        }

        KeyedCollection<string, SvnChangeItem> ISvnLogItem.ChangedPaths
        {
            get { return null; }
        }

        Uri ISvnLogItem.RepositoryRoot
        {
            get { return Origin.RepositoryRoot; }
        }
        #endregion Members
    }
}
