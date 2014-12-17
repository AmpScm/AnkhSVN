// $Id$
//
// Copyright 2008 The AnkhGit Project
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

namespace Ankh.Scc
{
    public class GitItemsEventArgs : EventArgs
    {
        readonly ReadOnlyCollection<GitItem> _changedItems;

        public GitItemsEventArgs(IList<GitItem> changedItems)
        {
            if(changedItems == null)
                throw new ArgumentNullException("changedItems");

            _changedItems = new ReadOnlyCollection<GitItem>(changedItems);
        }

        public ReadOnlyCollection<GitItem> ChangedItems
        {
            get { return _changedItems; }
        }
    }

    public interface IGitItemChange
    {
        /// <summary>
        /// Occurs when the state of one or more <see cref="GitItem"/> instances changes
        /// </summary>
        event EventHandler<GitItemsEventArgs> GitItemsChanged;

        /// <summary>
        /// Raises the <see cref="E:GitItemsChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Ankh.Scc.GitItemsEventArgs"/> instance containing the event data.</param>
        void OnGitItemsChanged(GitItemsEventArgs e);
    }    
}
