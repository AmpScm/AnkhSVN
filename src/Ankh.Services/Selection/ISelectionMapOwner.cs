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
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Ankh.Selection
{
    public interface ISelectionMapOwner<T>
    {
        IList Selection { get; }
        IList AllItems { get; }

        bool SelectionContains(T item);

        IntPtr GetImageList();
        int GetImageListIndex(T item);
        string GetText(T item);

        object GetSelectionObject(T item);

        T GetItemFromSelectionObject(object item);
        void SetSelection(T[] items);
        event EventHandler HandleDestroyed;

        Control Control { get; }

        /// <summary>
        /// Gets the canonical (path / uri) of the item. Used by packages to determine a selected file
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A canonical name or null</returns>
        string GetCanonicalName(T item);
    }
}
