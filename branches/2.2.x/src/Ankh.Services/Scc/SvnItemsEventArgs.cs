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

namespace Ankh.Scc
{
    public class SvnItemsEventArgs : EventArgs
    {
        readonly ReadOnlyCollection<SvnItem> _changedItems;

        public SvnItemsEventArgs(IList<SvnItem> changedItems)
        {
            if(changedItems == null)
                throw new ArgumentNullException("changedItems");

            _changedItems = new ReadOnlyCollection<SvnItem>(changedItems);
        }

        public ReadOnlyCollection<SvnItem> ChangedItems
        {
            get { return _changedItems; }
        }
    }

    public interface ISvnItemChange
    {
        /// <summary>
        /// Occurs when the state of one or more <see cref="SvnItem"/> instances changes
        /// </summary>
        event EventHandler<SvnItemsEventArgs> SvnItemsChanged;

        /// <summary>
        /// Raises the <see cref="E:SvnItemsChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Ankh.Scc.SvnItemsEventArgs"/> instance containing the event data.</param>
        void OnSvnItemsChanged(SvnItemsEventArgs e);
    }    
}
