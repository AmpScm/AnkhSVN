// $Id$
//
// Copyright 2004-2008 The AnkhSVN Project
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

// $Id$
using System;
using Ankh.UI;
using System.Windows.Forms;
using Ankh.Scc;

namespace Ankh
{
    /// <summary>
    /// Represents the UI of the addin.
    /// </summary>
    public interface IUIShell
    {
        /// <summary>
        /// Displays HTML in some suitable view.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="html"></param>
        void DisplayHtml(string caption, string html, bool reuse);

        /// <summary>
        /// Show a "path selector dialog".
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        PathSelectorResult ShowPathSelector(PathSelectorInfo info);

        /// <summary>
        /// Edits the state of the enlistment.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        bool EditEnlistmentState(EnlistmentState state);

        /// <summary>
        /// Shows the dialog for adding a new root to the repository.
        /// </summary>
        /// <returns></returns>
        Uri ShowAddRepositoryRootDialog();

        /// <summary>
        /// Shows the add working copy explorer root dialog.
        /// </summary>
        /// <returns></returns>
        string ShowAddWorkingCopyExplorerRootDialog();
    }
}
