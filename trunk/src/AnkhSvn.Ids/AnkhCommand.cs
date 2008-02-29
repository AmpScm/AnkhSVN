using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace AnkhSvn.Ids
{
	/// <summary>
	/// List of ankh commands
	/// </summary>
	/// <remarks>
	/// <para>New items should be added at the end. unused items should only be obsoleted.</para>
	/// <para>This list of commands is part of the public api for communicating within Visual Studio</para>
	/// </remarks>
	[Guid(AnkhId.CommandSet)]
	public enum AnkhCommand // : int
	{
		[Obsolete]
		None = 0,

		// These values live in the same numberspace as the other values within 
		// the command set. So we start countin at this number to make sure we
		// do not reuse values
		CommandFirst = 0x1FFFFFFF,

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
		MakeDirectory,
		Refresh,
		RefreshRepositoryItem,
		Relocate,
		RemoveRepositoryRoot,
		RemoveWorkingCopyExplorerRoot,
		ResolveConflict,
		ResolveConflictExternal,
		ReverseMerge,
		RevertItem,
		SaveToFile,
		SendErrorReport,
		ShowCommitDialog,
		ShowRepositoryExplorer,
		ShowWorkingCopyExplorer,
		SwitchItem,
		ToggleAnkh,
		Unlock,
		UpdateItem,
		ViewInVsNet,
		ViewInWindows,
		ViewRepositoryFile,
		CheckForOrphanedTreeNodes,
	}
}
