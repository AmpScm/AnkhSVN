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
using System.Collections.ObjectModel;

namespace Ankh.UI
{
    /// <summary>
    /// To be implemented by the provider of focus/selection
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface ICurrentItemSource<TItem>
    {
        /// <summary>
        /// Occurs when the selection has been changed
        /// </summary>
        event EventHandler<CurrentItemEventArgs<TItem>> SelectionChanged;

        /// <summary>
        /// Occurs when the focus has been changed
        /// </summary>
        event EventHandler<CurrentItemEventArgs<TItem>> FocusChanged;

        /// <summary>
        /// Gets the focused item
        /// </summary>
        TItem FocusedItem { get; }

        /// <summary>
        /// Gets the selected items
        /// </summary>
        IList<TItem> SelectedItems { get; }
    }

    /// <summary>
    /// To be implemented by the consumer of focus/selection
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface ICurrentItemDestination<TItem>
    {
        /// <summary>
        /// Gets or sets the <see cref="ICurrentItemSource"/>
        /// </summary>
        ICurrentItemSource<TItem> ItemSource { get;set;}
    }

    public class CurrentItemEventArgs<TItem> : EventArgs
    {
        readonly ICurrentItemSource<TItem> _source;
        public CurrentItemEventArgs(ICurrentItemSource<TItem> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            _source = source;
        }

        public ICurrentItemSource<TItem> Source
        {
            get { return _source; }
        }
    }    
}
