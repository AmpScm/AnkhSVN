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

namespace Ankh.Scc
{
    public interface ISvnRepositoryItem
    {
        /// <summary>
        /// Gets the Uri of the item (Required)
        /// </summary>
        Uri Uri { get; }
        /// <summary>
        /// Gets the <see cref="SvnNodeKind"/> of the item (Optional)
        /// </summary>
        SvnNodeKind NodeKind { get; }
        /// <summary>
        /// Gets the <see cref="SvnRevision"/> of the item (Optional)
        /// </summary>
        SvnRevision Revision { get; }

        /// <summary>
        /// Gets the name of the item (its filename)
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Refreshes the item.
        /// </summary>
        void RefreshItem(bool refreshParent);

        /// <summary>
        /// Gets the origin.
        /// </summary>
        /// <value>The origin.</value>
        SvnOrigin Origin { get; }
    }
}
