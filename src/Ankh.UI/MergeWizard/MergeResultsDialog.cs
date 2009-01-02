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
using System.Windows.Forms;
using SharpSvn;
using System.Diagnostics;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeResultsDialog : VSDialogForm
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
            Dictionary<string, List<string>> model = new Dictionary<string, List<string>>();

            foreach (SvnNotifyEventArgs snea in MergeActions)
            {
                string contentAction = "";
                string propertyAction = "";

                switch (snea.Action)
                {
                    case SvnNotifyAction.Exists:
                        if (!model.ContainsKey(snea.FullPath) ||
                            (model.ContainsKey(snea.FullPath) && !model[snea.FullPath].Contains("Text-" + Resources.Existed)))
                        {
                            fileExisted++;
                            contentAction = Resources.Existed;
                        }
                        break;
                    case SvnNotifyAction.Skip:
                        if (!model.ContainsKey(snea.FullPath) ||
                            (model.ContainsKey(snea.FullPath) && !model[snea.FullPath].Contains("Text-" + Resources.Skipped)))
                        {
                            if (snea.NodeKind == SvnNodeKind.Directory)
                                fileSkippedDirs++;
                            else if (snea.NodeKind == SvnNodeKind.File)
                                fileSkippedFiles++;
                            contentAction = Resources.Skipped;
                        }
                        break;
                    case SvnNotifyAction.UpdateAdd:
                        if (!model.ContainsKey(snea.FullPath) ||
                            (model.ContainsKey(snea.FullPath) && !model[snea.FullPath].Contains("Text-" + Resources.Added)))
                        {
                            fileAdded++;
                            contentAction = Resources.Added;
                        }
                        break;
                    case SvnNotifyAction.UpdateDelete:
                        if (!model.ContainsKey(snea.FullPath) ||
                            (model.ContainsKey(snea.FullPath) && !model[snea.FullPath].Contains("Text-" + Resources.Deleted)))
                        {
                            fileDeleted++;
                            contentAction = Resources.Deleted;
                        }
                        break;
                    case SvnNotifyAction.UpdateReplace:
                        if (!model.ContainsKey(snea.FullPath) ||
                            (model.ContainsKey(snea.FullPath) && !model[snea.FullPath].Contains("Text-" + Resources.Replaced)))
                        {
                            fileAdded++;
                            contentAction = Resources.Replaced;
                        }
                        break;
                    case SvnNotifyAction.UpdateUpdate:
                        if (snea.ContentState != SvnNotifyState.None &&
                            snea.ContentState != SvnNotifyState.Unchanged &&
                            snea.ContentState != SvnNotifyState.Unknown)
                        {
                            switch (snea.ContentState)
                            {
                                case SvnNotifyState.Changed:
                                    if (!model.ContainsKey(snea.FullPath) ||
                                        (model.ContainsKey(snea.FullPath) && !model[snea.FullPath].Contains("Text-" + Resources.Modified)))
                                    {
                                        fileUpdated++;
                                        contentAction = Resources.Modified;
                                    }
                                    break;
                                case SvnNotifyState.Conflicted:
                                    if (!model.ContainsKey(snea.FullPath) ||
                                        (model.ContainsKey(snea.FullPath) && !model[snea.FullPath].Contains("Text-" + Resources.Conflicted)))
                                    {
                                        fileConflicted++;
                                        contentAction = Resources.Conflicted;
                                    }
                                    break;
                                case SvnNotifyState.Merged:
                                    if (!model.ContainsKey(snea.FullPath) ||
                                        (model.ContainsKey(snea.FullPath) && !model[snea.FullPath].Contains("Text-" + Resources.Merged)))
                                    {
                                        fileMerged++;
                                        contentAction = Resources.Merged;
                                    }
                                    break;
                                default:
                                    // Do nothing.
                                    break;
                            }
                        }

                        if (snea.PropertyState != SvnNotifyState.None &&
                            snea.PropertyState != SvnNotifyState.Unchanged &&
                            snea.PropertyState != SvnNotifyState.Unknown)
                        {
                            switch (snea.PropertyState)
                            {
                                case SvnNotifyState.Changed:
                                    if (!model.ContainsKey(snea.FullPath) ||
                                        (model.ContainsKey(snea.FullPath) && !model[snea.FullPath].Contains("Prop-" + Resources.Modified)))
                                    {
                                        propertyUpdated++;
                                        propertyAction = Resources.Modified;
                                    }
                                    break;
                                case SvnNotifyState.Conflicted:
                                    if (!model.ContainsKey(snea.FullPath) ||
                                            (model.ContainsKey(snea.FullPath) && !model[snea.FullPath].Contains("Prop-" + Resources.Conflicted)))
                                    {
                                        propertyConflicted++;
                                        propertyAction = Resources.Conflicted;
                                    }
                                    break;
                                case SvnNotifyState.Merged:
                                    if (!model.ContainsKey(snea.FullPath) ||
                                        (model.ContainsKey(snea.FullPath) && !model[snea.FullPath].Contains("Prop-" + Resources.Merged)))
                                    {
                                        propertyMerged++;
                                        propertyAction = Resources.Merged;
                                    }
                                    break;
                                default:
                                    // Do nothing.
                                    break;
                            }
                        }
                        break;
                    default:
                        // Do nothing
                        break;
                }

                if (model.ContainsKey(snea.FullPath))
                {
                    if (contentAction.Length > 0)
                    {
                        if (!model[snea.FullPath].Contains("Text-" + contentAction))
                            model[snea.FullPath].Add("Text-" + contentAction);
                    }

                    if (propertyAction.Length > 0)
                    {
                        if (!model[snea.FullPath].Contains("Prop-" + propertyAction))
                            model[snea.FullPath].Add("Prop-" + propertyAction);
                    }
                }
                else
                {
                    if (contentAction.Length > 0 || propertyAction.Length > 0)
                    {
                        List<string> l = new List<string>();

                        if (contentAction.Length > 0)
                            l.Add("Text-" + contentAction);

                        if (propertyAction.Length > 0)
                            l.Add("Prop-" + propertyAction);

                        model.Add(snea.FullPath, l);
                    }
                }
            }

            // Calculate Resolved
            foreach (KeyValuePair<string, List<SvnConflictType>> resolution in ResolvedMergeConflicts)
            {
                if (resolution.Value.Contains(SvnConflictType.Content))
                    fileResolved++;
                else if (resolution.Value.Contains(SvnConflictType.Property))
                    propertyResolved++;
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

            // Populate the Modified Paths ListView
            foreach (KeyValuePair<string, List<string>> item in model)
            {
                ListViewItem lvi;
                string[] row = new string[3];
                string contents = "";
                string properties = "";

                row[0] = item.Key;

                foreach (string action in item.Value)
                {
                    if (action.StartsWith("Text"))
                    {
                        if (contents.Length == 0)
                            contents = action.Split('-')[1];
                        else
                            contents += (", " + action.Split('-')[1]);
                    }
                    else if (action.StartsWith("Prop"))
                    {
                        if (properties.Length == 0)
                            properties = action.Split('-')[1];
                        else
                            properties += (", " + action.Split('-')[1]);
                    }
                }

                if (contents.Length == 0)
                    row[1] = Resources.Unchanged;
                else
                    row[1] = contents;

                if (properties.Length == 0)
                    row[2] = Resources.Unchanged;
                else
                    row[2] = properties;

                lvi = new ListViewItem(row);

                modifiedPathsListView.Items.Add(lvi);
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

        private void MergeResultsDialog_Resize(object sender, EventArgs e)
        {
            modifiedPathsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
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
