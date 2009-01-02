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
using System.Collections.ObjectModel;

namespace Ankh.Scc.ProjectMap
{
    class SccTranslateEnlistData : SccTranslateData
    {
        public SccTranslateEnlistData(AnkhSccProvider provider, Guid projectId)
            : base(provider, projectId)
        {
        }        

        class SccEnlistData
        {
            readonly IAnkhServiceProvider _context;
            readonly Guid _projectId;
            string _uniqueName;
            string _displayPath;
            string _uncPath;
            SccEnlistMode _enlistMode;

            public SccEnlistData(IAnkhServiceProvider context, Guid projectId)
            {
                if (context == null)
                    throw new ArgumentNullException("context");

                _context = context;
                _projectId = projectId;
            }

            public Guid ProjectGuid
            {
                get { return _projectId; }
            }

            public SccEnlistMode EnlistMode
            {
                get { return _enlistMode; }
                internal set { _enlistMode = value; }
            }

            /// <summary>
            /// Gets or sets the name of the project as stored in the solution file
            /// </summary>
            public string UniqueName
            {
                get { return _uniqueName; }
                internal set { _uniqueName = value; }
            }

            /// <summary>
            /// Gets or sets the display path of the Enlist data
            /// </summary>
            /// <remarks>In most implementations the same as the Unc path</remarks>
            public string DisplayPath
            {
                get { return _displayPath; }
                internal set { _displayPath = value; }
            }

            /// <summary>
            /// Gets or sets the real path used to access the project on disk
            /// </summary>
            public string UncPath
            {
                get { return _uncPath; }
                internal set { _uncPath = value; }
            }

            internal sealed class EnlistDataCollection : KeyedCollection<string, SccEnlistData>
            {
                public EnlistDataCollection()
                    : base(StringComparer.OrdinalIgnoreCase)
                {
                }

                protected override string GetKeyForItem(SccEnlistData item)
                {
                    return item.ProjectGuid.ToString("B");
                }
            }

            internal void LoadUserData(List<string> values)
            {
                throw new NotImplementedException();
            }

            internal bool ShouldSerialize()
            {
                throw new NotImplementedException();
            }

            internal string[] GetUserData()
            {
                return new string[0];
            }

            internal bool HasPaths()
            {
                throw new NotImplementedException();
            }
        }


    }
}
