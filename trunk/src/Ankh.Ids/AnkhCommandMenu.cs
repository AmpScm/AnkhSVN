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
using System.Runtime.InteropServices;

namespace Ankh.Ids
{
    // Disable Missing XML comment warning for this file
#pragma warning disable 1591

    /// <summary>
    /// List of ankh menus
    /// </summary>
    /// <remarks>
    /// <para>New items should be added at the end. Items should only be obsoleted between releases.</para>
    /// <para>The values of this enum are part of our interaction with other packages within Visual Studio</para>
    /// </remarks>
    [Guid(AnkhId.CommandSet)]
    public enum AnkhCommandMenu
    {
        None = 0,

        // These values live in the same numberspace as the other values within 
        // the command set. So we start countin at this number to make sure we
        // do not reuse values
        MenuFirst = 0x5FFFFFF,

        FileScc,

        RepositoryExplorerToolBar,
        RepositoryExplorerContextMenu,
        WorkingCopyExplorerToolBar,
        WorkingCopyExplorerContextMenu,

        SolutionExplorerSccForSolution,
        SolutionExplorerSccForProject,
        SolutionExplorerSccForItem,

        PendingCommitsContextMenu,
        PendingCommitsHeaderContextMenu,
        LogMessageEditorContextMenu,

        PendingChangesCommit,
        PendingChangesUpdate,
        PendingChangesView,

        PendingCommitsView,
        PendingCommitsSort,

        PendingCommitsGroup,

        ItemConflict,
        ItemCompare,
        ItemOpen,
        ItemRevert,
        ItemResolve,

        ItemIgnore,

        LogViewerContextMenu,
        LogChangedPathsContextMenu,

        ListViewSort,
        ListViewGroup,
        ListViewShow,
        ListViewHeader,

        SolutionFileScc,
        ProjectFileScc,
        
        BlameContextMenu,

        EditorScc,
    }
}
