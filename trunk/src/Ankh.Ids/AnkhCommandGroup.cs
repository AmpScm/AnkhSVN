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
using System.Runtime.InteropServices;

namespace Ankh
{
    // Disable Missing XML comment warning for this file
#pragma warning disable 1591 

    /// <summary>
    /// List of ankh command groups
    /// </summary>
    /// <remarks>
    /// <para>New items should be added at the end. Items should only be obsoleted between releases.</para>
    /// <para>The values of this enum are part of our interaction with other packages within Visual Studio</para>
    /// </remarks>
    [Guid(AnkhId.CommandSet)]
    public enum AnkhCommandGroup
    {
        None = 0,

        // These values live in the same numberspace as the other values within 
        // the command set. So we start countin at this number to make sure we
        // do not reuse values
        GroupFirst = 0x3FFFFFF,

        FileSourceControl,
        FileMenuScc,
        FileSccMenuTasks,
        FileSccItem,
        FileSccOpen,
        FileSccAdd,
        FileSccManagement,

        NoUI,

        SccSlnAdvanced,
        SccProjectAdvanced,
        SccItemAdvanced,

        RepositoryExplorerTbBrowse,
        RepositoryExplorerTbEdit,
        RepositoryExplorerContext,
        WorkingCopyExplorerToolBar,
        WorkingCopyExplorerContext,

        PendingChangesToolBar,
        PendingChangesLogMessageCommands,
        PendingChangesLogEditor,
        PendingChangesLogEditor2,
        PendingChangesContextMenu,

        PendingChangesSolutionCommands,
        PendingChangesView,
        PendingChangesItems,
        PendingChangesAdmin,
        PendingChangesSwitch,

        PendingChangesOpenEx,
        PendingCommitsMenu,
        PendingCommitsSortActions,
        PendingCommitsGroupActions,
        PendingCommitsActions,

        LogMessageCommands,

        FileFileOpen, // File->Open->*
        FileFileAdd, // File->Add->*

        ItemResolveCommand,
        ItemResolveFile,
        ItemResolveAutoFull,
        ItemResolveAutoConflict,

        SccTbManage,
        SccTbItem,
        SccTbExtra,

        LogViewerToolbar,
        LogViewerContextMenu,

        SccExplore,

        LogChangedPathsContextMenu,

        ListViewState,

        ListViewSort,
        ListViewSortOrder,
        ListViewGroup,
        ListViewGroupOrder,
        ListViewShow,

        ItemIgnore,
        AnnotateContextMenu,

        CodeEditorScc,

        PendingChangesCommit1,
        PendingChangesCommit2,
        PendingChangesCommit3,

        PendingChangesUpdate1,
        PendingChangesUpdate2,
        PendingChangesUpdate3,

        RepositoryExplorerOpen,
        RepositoryExplorerReview,
        RepositoryExplorerEdit,
        RepositoryExplorerRefer,

        RepositoryExplorerTbOpen,
        RepositoryExplorerTbAction,

        OpenWith,
        OpenWith2,

        /// <summary>
        /// Special parent group for VS Menu Designer support
        /// </summary>
        AnkhContextMenus,
        SccLock,
        SccAddIgnore,

        SccCleanupRefresh,

        SccItemUpdate,
        SccItemAction,
        SccItemChanges,
        //SccItemAdvanced,
        SccItemBranch,
        SccItemPatch,

        SccProjectUpdate,
        SccProjectAction,
        SccProjectChanges,        
        //SccProjectAdvanced,
        SccProjectBranch,
        SccProjectPatch,

        SccSlnUpdate,
        SccSlnAction,
        SccSlnChanges,        
        //SccSlnAdvanced,
        SccSlnBranch,
        SccSlnPatch,

        SccPrjFileUpdate,
        SccPrjFileAction,
        SccPrjFileChanges,
        SccPrjFileAdvanced,
        SccPrjFileBranch,

        SccSlnFileUpdate,
        SccSlnFileAction,
        SccSlnFileChanges,
        SccSlnFileAdvanced,
        SccSlnFileBranch,

        WceUpdate,
        WceAction,
        WceChanges,
        WceAdvanced,
        WceOpen,
        WceBranch,
        WceEdit,

        PendingChangesLogEditor3,
        MdiDocumentScc,
        DocumentPending,
    }
}
