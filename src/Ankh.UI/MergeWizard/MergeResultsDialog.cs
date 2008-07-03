using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SharpSvn;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeResultsDialog : Form
    {
        public MergeResultsDialog()
        {
            InitializeComponent();
        }

        public MergeResultsDialog(List<SvnNotifyEventArgs> mergeActions, Dictionary<string, List<SvnConflictType>> resolvedConflicts)
            : this()
        {
            MergeActions = mergeActions;
            ResolvedMergeConflicts = resolvedConflicts;
        }

        /// <summary>
        /// Gets the actions performed by the merge.
        /// </summary>
        public List<SvnNotifyEventArgs> MergeActions
        {
            get { return _mergeActions; }
            set { _mergeActions = value; }
        }

        /// <summary>
        /// Gets the paths of the conflicts resolved during the merge.
        /// </summary>
        public Dictionary<string, List<SvnConflictType>> ResolvedMergeConflicts
        {
            get { return _resolvedMergeConflicts; }
            set { _resolvedMergeConflicts = value; }
        }

        private void PopulateUI()
        {
            Dictionary<string, List<string>> knownFileActions = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> knownPropertyActions = new Dictionary<string, List<string>>();

            foreach (SvnNotifyEventArgs mergeAction in MergeActions)
            {
                // Handle Text Changes
                if (mergeAction.ContentState != SvnNotifyState.None &&
                    mergeAction.ContentState != SvnNotifyState.Unknown &&
                    mergeAction.ContentState != SvnNotifyState.Unchanged)
                {
                    bool shouldCount = true;
                    string mergeActionAsString = mergeAction.Action.ToString() + "-" +
                        mergeAction.ContentState.ToString();

                    if (knownFileActions.ContainsKey(mergeAction.FullPath))
                    {
                        if (knownFileActions[mergeAction.FullPath].Contains(mergeActionAsString))
                            shouldCount = false;
                        else
                            knownFileActions[mergeAction.FullPath].Add(mergeActionAsString);
                    }
                    else
                    {
                        List<string> action = new List<string>();

                        action.Add(mergeActionAsString);

                        knownFileActions.Add(mergeAction.FullPath, action);
                    }

                    if (shouldCount)
                    {
                        switch (mergeAction.Action)
                        {
                            // Handle Existed
                            case SvnNotifyAction.Exists:
                                fileExisted++;
                                break;
                            // Handle Dir/File Skips
                            case SvnNotifyAction.Skip:
                                if (mergeAction.NodeKind == SvnNodeKind.Directory)
                                    fileSkippedDirs++;
                                else if (mergeAction.NodeKind == SvnNodeKind.File)
                                    fileSkippedFiles++;
                                break;
                            // Handle Dir/File Added
                            case SvnNotifyAction.UpdateAdd:
                                fileAdded++;
                                break;
                            // Handle Dir/File Deleted
                            case SvnNotifyAction.UpdateDelete:
                                fileDeleted++;
                                break;
                            // Handle Dir/File Added
                            case SvnNotifyAction.UpdateReplace:
                                fileAdded++;
                                break;
                            case SvnNotifyAction.UpdateUpdate:
                                // Handle Dir/File Updated
                                if (mergeAction.ContentState == SvnNotifyState.Changed)
                                    fileUpdated++;
                                // Handle Dir/File Conflicted/Resolved
                                else if (mergeAction.ContentState == SvnNotifyState.Conflicted)
                                {
                                    if (ResolvedMergeConflicts.ContainsKey(mergeAction.FullPath) &&
                                        ResolvedMergeConflicts[mergeAction.FullPath].Contains(SvnConflictType.Content))
                                        fileResolved++;
                                    else
                                        fileConflicted++;
                                }
                                // Handle Dir/File Merged
                                else if (mergeAction.ContentState == SvnNotifyState.Merged)
                                    fileMerged++;
                                break;
                            default:
                                break;
                        }
                    }
                }

                // Handle Property Changes
                if (mergeAction.PropertyState != SvnNotifyState.None &&
                    mergeAction.PropertyState != SvnNotifyState.Unknown &&
                    mergeAction.PropertyState != SvnNotifyState.Unchanged)
                {
                    bool shouldCount = true;
                    string mergeActionAsString = mergeAction.Action.ToString() + "-" +
                        mergeAction.PropertyState.ToString();

                    if (knownPropertyActions.ContainsKey(mergeAction.FullPath))
                    {
                        if (knownPropertyActions[mergeAction.FullPath].Contains(mergeActionAsString))
                            shouldCount = false;
                        else
                            knownPropertyActions[mergeAction.FullPath].Add(mergeActionAsString);
                    }
                    else
                    {
                        List<string> action = new List<string>();

                        action.Add(mergeActionAsString);

                        knownPropertyActions.Add(mergeAction.FullPath, action);
                    }

                    if (shouldCount)
                    {
                        switch (mergeAction.Action)
                        {
                            case SvnNotifyAction.UpdateUpdate:
                                if (mergeAction.PropertyState == SvnNotifyState.Changed)
                                    propertyUpdated++;
                                else if (mergeAction.PropertyState == SvnNotifyState.Conflicted)
                                {
                                    if (ResolvedMergeConflicts.ContainsKey(mergeAction.FullPath) &&
                                        ResolvedMergeConflicts[mergeAction.FullPath].Contains(SvnConflictType.Property))
                                        fileResolved++;
                                    else
                                        fileConflicted++;
                                }
                                else if (mergeAction.PropertyState == SvnNotifyState.Merged)
                                    propertyMerged++;
                                break;
                            default:
                                // Do nothing.
                                break;
                        }
                    }
                }

                // Update File Labels
                fileUpdatedValueLabel.Text = fileUpdated.ToString();
                fileAddedValueLabel.Text = fileAdded.ToString();
                fileExistedValueLabel.Text = fileExisted.ToString();
                fileDeletedValueLabel.Text = fileDeleted.ToString();
                fileMergedValueLabel.Text = fileMerged.ToString();
                fileConflictedValueLabel.Text = fileConflicted.ToString();
                fileResolvedValueLabel.Text = fileResolved.ToString();
                fileSkippedDirectoriesValueLabel.Text = fileSkippedDirs.ToString();
                fileSkippedFilesValueLabel.Text = fileSkippedFiles.ToString();

                // Update Property Labels
                propertyUpdatedValueLabel.Text = propertyUpdated.ToString();
                propertyMergedValueLabel.Text = propertyMerged.ToString();
                propertyConflictedValueLabel.Text = propertyConflicted.ToString();
                propertyResolvedValueLabel.Text = propertyResolved.ToString();
            }
        }

        #region UI Events
        private void MergeResultsDialog_Load(object sender, EventArgs e)
        {
            if (MergeActions != null)
            {
                this.Visible = false;

                PopulateUI();

                this.Visible = true;
            }
        }
        #endregion

        long fileUpdated = 0;
        long fileAdded = 0;
        long fileExisted = 0;
        long fileDeleted = 0;
        long fileMerged = 0;
        long fileConflicted = 0;
        long fileResolved = 0;
        long fileSkippedDirs = 0;
        long fileSkippedFiles = 0;
        long propertyUpdated = 0;
        long propertyMerged = 0;
        long propertyConflicted = 0;
        long propertyResolved = 0;
        List<SvnNotifyEventArgs> _mergeActions;
        Dictionary<string, List<SvnConflictType>> _resolvedMergeConflicts;
    }
}
