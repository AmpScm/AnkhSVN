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
using System.Text;
using SharpSvn;

namespace Ankh.Scc
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>The default implementation of this service is thread safe</remarks>
    public interface IFileStatusCache : IAnkhServiceProvider
    {
        /// <summary>
        /// Gets the <see cref="Ankh.SvnItem"/> with the specified path.
        /// </summary>
        /// <value></value>
        SvnItem this[string path] { get; }

        /// <summary>
        /// Marks the specified path dirty
        /// </summary>
        /// <param name="path">A file of directory</param>
        /// <remarks>If the file is in the cache</remarks>
        void MarkDirty(string path);

        /// <summary>
        /// Marks the specified paths dirty
        /// </summary>
        /// <param name="paths">The paths.</param>
        void MarkDirty(IEnumerable<string> paths);

        /// <summary>
        /// Clears the whole statuscache; called when closing the solution
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Called from <see cref="SvnItem.Refresh()"/>
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="diskNodeKind">The on-disk node kind if it is known to be correct.</param>
        void RefreshItem(SvnItem item, SvnNodeKind diskNodeKind);

        /// <summary>
        /// Refreshes the nested status of the <see cref="SvnItem"/>
        /// </summary>
        /// <param name="item"></param>
        void RefreshNested(SvnItem item);

        /// <summary>
        /// Gets the <see cref="SvnDirectory"/> of the specified path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        SvnDirectory GetDirectory(string path);

        void MarkDirtyRecursive(string path);

        IList<SvnItem> GetCachedBelow(string path);
        IList<SvnItem> GetCachedBelow(IEnumerable<string> paths);

        void SetSolutionContained(string path, bool contained);
    }
}
