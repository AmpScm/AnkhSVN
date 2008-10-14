using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Ankh.Ids
{
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

        PendingChangesContextMenu,
        PendingCommitsSortContextMenu,
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
    }
}
