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
using System.Collections.Generic;
using System.Windows.Forms;
using SharpSvn;
using System.Diagnostics;
using System.Drawing;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeResultsDialog : VSDialogForm
    {
        public MergeResultsDialog()
        {
            InitializeComponent();

            // Preserve the original merge-result ordering while using the
            // shared VS interaction palette from SmartListView.
            modifiedPathsListView.Sorting = SortOrder.None;
            modifiedPathsListView.ListViewItemSorter = null;
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
            List<MergeNotificationInfo> notifications =
                new List<MergeNotificationInfo>(MergeActions.Count);

            foreach (SvnNotifyEventArgs action in MergeActions)
                notifications.Add(MergeNotificationInfo.From(action));

            MergeResultModel model = MergeResultModel.Build(
                notifications,
                ResolvedMergeConflicts);

            // Update File Labels
            fileUpdatedValueLabel.Text = model.FileUpdated.ToString();
            fileAddedValueLabel.Text = model.FileAdded.ToString();
            fileExistedValueLabel.Text = model.FileExisted.ToString();
            fileDeletedValueLabel.Text = model.FileDeleted.ToString();
            fileMergedValueLabel.Text = model.FileMerged.ToString();
            fileConflictedValueLabel.Text = model.FileConflicted.ToString();
            fileResolvedValueLabel.Text = model.FileResolved.ToString();
            fileSkippedDirectoriesValueLabel.Text = model.FileSkippedDirectories.ToString();
            fileSkippedFilesValueLabel.Text = model.FileSkippedFiles.ToString();

            // Update Property Labels
            propertyUpdatedValueLabel.Text = model.PropertyUpdated.ToString();
            propertyMergedValueLabel.Text = model.PropertyMerged.ToString();
            propertyConflictedValueLabel.Text = model.PropertyConflicted.ToString();
            propertyResolvedValueLabel.Text = model.PropertyResolved.ToString();

            // Populate the Modified Paths ListView
            modifiedPathsListView.Items.Clear();

            foreach (MergePathResult item in model.Paths.Values)
            {
                string[] row = new string[3];
                row[0] = item.Path;
                row[1] = MergeResultModel.FormatActions(item.ContentActions);
                row[2] = MergeResultModel.FormatActions(item.PropertyActions);

                modifiedPathsListView.Items.Add(new ListViewItem(row));
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

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            headerTitle.Font = new Font(Font, FontStyle.Bold);
        }

        List<SvnNotifyEventArgs> _mergeActions;
        Dictionary<string, List<SvnConflictType>> _resolvedMergeConflicts;
    }
}
