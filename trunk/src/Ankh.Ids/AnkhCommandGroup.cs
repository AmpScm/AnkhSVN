using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Ankh.Ids
{
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

        SolutionExplorerSccForSolution,
        SolutionExplorerSccForProject,
        SolutionExplorerSccForItem,

        RepositoryExplorerToolBar,
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
        BlameContextMenu,

        CodeEditorScc,
    }
}
