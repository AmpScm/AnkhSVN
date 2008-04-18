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
        // Two test commands

        FileSccMenuUpdateLatest,
        FileSccMenuUpdateSpecific,

        // Raw dump of old commands; to be sorted out
        AddItem,
        AddRepositoryRoot,
        AddSolutionToRepository,
        AddWorkingCopyExplorerRoot,
        Blame,
        ChangeAdminDirName,
        Checkout,
        CheckoutFolder,
        CheckoutSolution,
        Cleanup,
        CommitItem,
        CopyReposExplorerUrl,
        CreatePatch,
        DiffExternalLocalItem,
        DiffLocalItem,
        EditConfigFile,
        Export,
        ExportFolder,
        Lock,
        Log,
        LogItem,
        NewDirectory,
        Refresh,
        RefreshRepositoryItem,
        Relocate,
        RemoveRepositoryRoot,
        RemoveWorkingCopyExplorerRoot,
        ResolveConflict,
        ResolveConflictExternal,
        RevertToRevision,
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
        CheckForOrphanedTreeNodes,

        PendingChangesSpacer, // Whitespace command to move all buttons a bit

        CommitPendingChanges,
        CommitPendingChangesKeepingLocks,

        SolutionUpdateHead,
        SolutionUpdateSpecific,

        ProjectUpdateHead,
        ProjectUpdateSpecific,

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

        ItemRevertBase,
        ItemRevertSpecific,

        ItemIgnoreFile,
        ItemIgnoreFileType,
        ItemIgnoreFilesInFolder,

        RefreshPendingChanges,

        SolutionSwitchCombo,
        SolutionSwitchComboFill,
        SolutionSwitchDialog,

        RunSvnCommand,

        PcLogEditorPasteFileList,
        PcLogEditorPasteRecentLog,

        #region Pending Changes ListView
        PcSortPath,
        PcSortProject,
        PcSortChange,
        PcSortChangeList,
        PcSortFullPath,
        PcSortLocked,
        PcSortModified,
        PcSortName,
        PcSortRepository,
        PcSortType,
        PcSortAscending,
        PcSortDescending,

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
        #endregion
    }
}
