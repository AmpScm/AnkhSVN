using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Ankh.Ids
{
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
        BlameShowBlame,
        Checkout,
        CheckoutFolder,
        CheckoutSolution,
        Cleanup,
        CommitItem,
        CopyReposExplorerUrl,
        CreatePatch,
        ItemShowChanges,
        [Obsolete("", true)]
        DiffExternalLocalItem,
        DiffLocalItem,
        Export,
        ExportFolder,
        Lock,
        Log,
        LogItem,
        NewDirectory,
        Refresh,
        RefreshRepositoryItem,
        RemoveRepositoryRoot,
        RemoveWorkingCopyExplorerRoot,
        ResolveConflict,
        ResolveConflictExternal,

        BlameShowLog,
        RevertItem,
        SaveToFile,
        SendFeedback,
        ShowPendingChanges,
        ShowRepositoryExplorer,
        ShowWorkingCopyExplorer,
        SwitchItem,
        ToggleAnkh,
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

        SolutionUpdateHead,
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

        // Currently unused block; values can be reused as they were never really handled                

        PcGroupPath,
        PcGroupProject,
        PcGroupChange,
        PcGroupChangeList,
        PcGroupFullPath,
        PcGroupLocked,
        PcGroupModified,
        PcGroupName,
        PcGroupRepository,
        PcGroupType,
        PcGroupAscending,
        PcGroupDescending,

        PcColViewPath,
        PcColViewProject,
        PcColViewChange,
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
    }
}
