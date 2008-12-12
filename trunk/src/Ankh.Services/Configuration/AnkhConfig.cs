// $Id$
//
// Copyright 2003-2008 The AnkhSVN Project
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
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using Microsoft.Win32;
using System.Security.AccessControl;
namespace Ankh.Configuration
{
    /// <summary>
    /// Ankh configuration container. Read and written via <see cref="IAnkhConfigurationService"/>
    /// </summary>
    public class AnkhConfig
    {
        string _mergeExePathField;
        string _diffExePathField;
        bool _interactiveMergeOnConflict;

        /// <summary>
        /// Gets or sets the merge exe path.
        /// </summary>
        /// <value>The merge exe path.</value>
        [DefaultValue(null)]
        public string MergeExePath
        {
            get { return _mergeExePathField; }
            set { _mergeExePathField = value; }
        }

        /// <summary>
        /// Gets or sets the diff exe path.
        /// </summary>
        /// <value>The diff exe path.</value>
        [DefaultValue(null)]
        public string DiffExePath
        {
            get { return _diffExePathField; }
            set { _diffExePathField = value; }
        }

        [DefaultValue(false)]
        public bool InteractiveMergeOnConflict
        {
            get { return _interactiveMergeOnConflict; }
            set { _interactiveMergeOnConflict = value; }
        }
    }
}
