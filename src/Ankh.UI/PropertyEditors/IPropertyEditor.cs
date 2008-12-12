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

//$Id$
using System;
using SharpSvn;

namespace Ankh.UI.PropertyEditors
{
    /// <summary>
    /// Interface which has to be implemented by property editor user controls
    /// </summary>
    internal interface IPropertyEditor
    {

        /// <summary>
        /// Whether the property editor is in a valid state
        /// </summary>
        bool Valid
        {
            get;
        }
    
        /// <summary>
        /// Gets or sets the property item
        /// </summary>
        PropertyItem PropertyItem
        {
            get;
            set;
        }

        /// <summary>
        /// Reset the property editor to its default state
        /// </summary>
        void Reset();

        /// <summary>
        /// Gets the allowed <code>SharpSvn.SvnNodeKind</code>s.
        /// </summary>
        /// <returns>SvnNodeKind</returns>
        SvnNodeKind GetAllowedNodeKind();

        /// <summary>
        /// Fired whenever the editor's state changes
        /// </summary>
        event EventHandler Changed;
    }
}

