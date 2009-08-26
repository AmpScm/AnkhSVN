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

        // Special task commands not used in menu's; only for use by AnkhSVN internally

        // Tick commands; one shot delayed task handlers
        TickFirst,
        MarkProjectDirty,
        FileCacheFinishTasks,
        SccFinishTasks,
        TickRefreshPendingTasks,
        TickRefreshSvnItems,
        // /Tick Commands
        TickLast,

        // Delayed implementation commands
        ActivateSccProvider,
        ActivateVsExtender,
        NotifyWcToNew,
        ForceUIShow,

        // /Private commands

        // These values live in the same numberspace as the other values within 
        // the command set. So we start countin at this number to make sure we
        // do not reuse values
        CommandFirst = 0x1FFFFFF,

        // Start of public commands; values shouldn't change between versions to
        // allow interop with other packages (like Collabnet Desktop and others)

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
        ItemResolveCasing,
        Cleanup,
        CommitItem,
        CopyReposExplorerUrl,
        CreatePatch,
        ItemShowChanges,
        DocumentShowChanges,
        DiffLocalItem,
        Export,
        ItemShowPropertyChanges,
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

        AnnotateShowLog,
        RevertItem,
        SaveToFile,
        DocumentConflictEdit,
        ShowPendingChanges,
        ShowRepositoryExplorer,
        ShowWorkingCopyExplorer,
        SwitchItem,
        RenameRepositoryItem,
        Unlock,
        UpdateItemSpecific,
        UpdateItemLatest,
        ViewInVsNet,
        ViewInWindows,
        SolutionBranch,

        ItemOpenFolderInRepositoryExplorer,
        ItemSelectInWorkingCopyExplorer,

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
        ItemOpenInRepositoryExplorer, // Unused

        ItemRevertBase,
        ItemRevertSpecific, // Unused

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

        RepositoryShowChanges,
        RepositoryCompareWithWc,
        UpdateItemLatestRecursive,
        SccLock,

        ItemRename,
        ItemDelete,

        ItemAddToPending,
        ItemRemoveFromPending,

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
        WorkingCopyAdd,

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

        LogShowRevisionProperties,

        MoveToNewChangeList,
        MoveToExistingChangeList0,
        MoveToExistingChangeListMax = MoveToExistingChangeList0 + 20,
        MoveToIgnoreChangeList,
        RemoveFromChangeList,

        SolutionIssueTrackerSetup,
        PcLogEditorOpenIssue,
    }
}
