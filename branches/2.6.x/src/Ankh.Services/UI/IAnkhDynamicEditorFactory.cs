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
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.UI
{
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public interface IAnkhDynamicEditorFactory
    {
        /// <summary>
        /// Creates the editor.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        IVsWindowFrame CreateEditor(string fullPath, VSEditorControl form);
    }

    [CLSCompliant(false)]
    public interface IAnkhDocumentHostService
    {
        /// <summary>
        /// Prepares the editor for hosting in a document window
        /// </summary>
        /// <param name="form">The form.</param>
        /// <param name="factoryId">The factory id.</param>
        /// <param name="doc">The doc.</param>
        /// <param name="pane">The pane.</param>
        void ProvideEditor(VSEditorControl form, Guid factoryId, out object doc, out object pane);

        /// <summary>
        /// Initializes the editor.
        /// </summary>
        /// <param name="frame">The frame.</param>
        void InitializeEditor(VSEditorControl form, IVsUIHierarchy hier, IVsWindowFrame frame, uint docid);
    }
}
