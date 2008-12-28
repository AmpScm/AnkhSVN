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
    /// List of ankh commands
    /// </summary>
    /// <remarks>
    /// <para>New items should be added at the end. Items should only be obsoleted between releases.</para>
    /// <para>The values of this enum are part of our interaction with other packages within Visual Studio</para>
    /// </remarks>
    [Guid(AnkhId.CommandSet)]
    public enum AnkhCommand // : int
    {
        None = 0,

        // Special task commands not used in menu's
        MarkProjectDirty,
        FileCacheFinishTasks,
        SccFinishTasks,
        TickRefreshPendingTasks,
        TickRefreshSvnItems,
        ActivateSccProvider,
        ActivateVsExtender,

        // These values live in the same numberspace as the other values within 
        // the command set. So we start countin at this number to make sure we
        // do not reuse values
        CommandFirst = 0x1FFFFFF,

        // Always visible+enabled entrance points to AnkhSvn
        FileFileOpenFromSubversion,
        FileFileAddFromSubversion,

        FileSccOpenFromSubversion,
        FileSccAddFromSubversion,

        FileSccAddSolutionToSubversion,
        FileSccAddProjectToSubversion,

        FileSccChangeSourceControl,

        FileSccMenuUpdateLatest,
        FileSccMenuUpdateSpecific,

        // Raw dump of old commands; to be sorted out
        AddItem,
        RepositoryBrowse,
        CheckForUpdates,
        WorkingCopyBrowse,
        ItemAnnotate,
        /// <summary>
        /// Execute blame command from blame window
        /// </summary>
        SvnNodeAnnotate,
        Checkout,
        CopyToWorkingCopy,
        [Obsolete]
        CheckoutSolution,
        Cleanup,
        CommitItem,
        CopyReposExplorerUrl,
        CreatePatch,
        ItemShowChanges,
        DocumentShowChanges,
        DiffLocalItem,
        Export,
        [Obsolete]
        ExportFolder,
        Lock,
        Log,
        LogItem,
        NewDirectory,
        Refresh,
        RefreshRepositoryItem,
        RemoveRepositoryRoot,
        RemoveWorkingCopyExplorerRoot,
        DocumentAnnotate,
        DocumentHistory,

        BlameShowLog,
        RevertItem,
        SaveToFile,
        DocumentConflictEdit,
        ShowPendingChanges,
        ShowRepositoryExplorer,
        ShowWorkingCopyExplorer,
        SwitchItem,
        RenameRepositoryItem,
        Unlock,
        UpdateItem,
        UpdateItemSpecific,
        ViewInVsNet,
        ViewInWindows,
        SolutionBranch,

        ItemOpenFolderInRepositoryExplorer,
        ItemOpenFolderInWorkingCopyExplorer,

        PendingChangesSpacer, // Whitespace command to move all buttons a bit

        CommitPendingChanges,
        CommitPendingChangesKeepingLocks,

        PendingChangesUpdateHead,
        SolutionUpdateSpecific,
        SolutionCommit,
        SolutionHistory,
        SolutionMerge,

        ProjectUpdateHead,
        ProjectUpdateSpecific,
        ProjectCommit,
        ProjectHistory,
        ProjectMerge,

        PendingChangesViewFlat,
        PendingChangesViewProject,
        PendingChangesViewChangeList,
        PendingChangesViewFolder,

        PendingChangesViewAll,
        PendingChangesViewLogMessage,

        ItemConflictEdit,
        ItemConflictEditVisualStudio,
        ItemConflictResolvedMerged,
        ItemConflictResolvedMineFull,
        ItemConflictResolvedTheirsFull,

        ItemCompareBase,
        ItemCompareHead,
        ItemCompareCommitted,
        ItemComparePrevious,
        ItemCompareSpecific,

        ItemOpenVisualStudio,
        ItemOpenWindows,
        ItemOpenTextEditor,
        ItemOpenFolder,
        ItemOpenSolutionExplorer,
        ItemOpenInRepositoryExplorer,

        ItemRevertBase,
        ItemRevertSpecific,

        ItemIgnoreFile,
        ItemIgnoreFileType,
        ItemIgnoreFilesInFolder,

        ItemResolveMerge,

        ItemResolveMineFull,
        ItemResolveTheirsFull,
        ItemResolveMineConflict,
        ItemResolveTheirsConflict,
        ItemResolveBase,
        ItemResolveWorking,
        ItemResolveMergeTool,

        ItemMerge,

        RefreshPendingChanges,

        SolutionSwitchCombo,
        SolutionSwitchComboFill,
        SolutionSwitchDialog,

        PcLogEditorPasteFileList,
        PcLogEditorPasteRecentLog,

        LogCompareWithWorkingCopy,
        LogCompareWithPrevious,
        LogCompareRevisions,
        LogRevertThisRevisions,
        LogOpen,
        LogOpenInVs,
        LogOpenWith,
        LogUpdateTo,
        LogRevertTo,
        LogMergeTo,
        LogShowProperties,

        ItemIgnoreFolder,

        LockMustLock,
        SvnNodeDelete,
        SolutionUpdateHead,

        PendingChangesApplyWorkingCopy,
        PendingChangesCreatePatch,
        PendingChangesApplyPatch,
        SolutionApplyPatch,

        ExplorerOpen,
        ExplorerUp,
        ReposExplorerOpenWith,        
        ReposExplorerShowPrevChanges,        
        ViewInVsText,
        ViewInWindowsWith,

        ReposCopyTo,
        ReposMoveTo,

        // Currently unused block; values can be reused as they were never really handled                
        PcColViewChangeList,
        PcColViewDate,
        PcColViewFullPath,
        PcColViewLocked,
        PcColViewModified,
        PcColViewName,
        PcColViewRepository,
        PcColViewType,
        // End of unused block

        LogStrictNodeHistory,
        LogIncludeMergedRevisions,
        LogFetchAll,
        LogShowChangedPaths,
        LogShowLogMessage,

        LogChangeLogMessage,
        LogShowChanges,

        MigrateSettings,
        ItemEditProperties,
        ReposExplorerLog,
        LogAnnotateRevision,


        UnifiedDiff,
        [Obsolete("Unused command", true)]
        RevertSolutionToRevision,

        SwitchProject,
        ProjectBranch,

        ListViewSortAscending,
        ListViewSortDescending,
        ListViewSort0,
        ListViewSortMax = ListViewSort0+64,        
        ListViewGroup0,
        ListViewGroupMax = ListViewGroup0 + 64,
        ListViewShow0,
        ListViewShowMax = ListViewShow0 + 64,

        SolutionEditProperties,
        ProjectEditProperties,

        LogShowRevisionProperties
    }
}
