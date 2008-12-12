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
}
