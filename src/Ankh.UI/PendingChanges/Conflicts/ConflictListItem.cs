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
using Ankh.Scc;
using Ankh.UI.VSSelectionControls;
using Ankh.UI.PendingChanges.Commits;
using Ankh.VS;
using SharpSvn;

namespace Ankh.UI.PendingChanges.Conflicts
{
    class ConflictListItem : SmartListViewItem
    {
        readonly PendingChange _change;

        public ConflictListItem(ConflictListView view, PendingChange change)
            : base(view)
        {
            if (change == null)
                throw new ArgumentNullException("change");

            _change = change;
            RefreshText(view.Context);
        }

        internal PendingChange PendingChange
        {
            get { return _change; }
        }

        public void RefreshText(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            ImageIndex = PendingChange.IconIndex;
            SvnItem item = PendingChange.SvnItem;
            if (item == null)
                throw new InvalidOperationException();

            bool textConflict =
                item.Status != null
                && item.Status.LocalTextStatus == SvnStatus.Conflicted;
            bool propertyConflict =
                item.Status != null
                && item.Status.LocalPropertyStatus == SvnStatus.Conflicted;

            string conflictType = PendingConflictLogic.GetConflictType(
                PendingChange.Kind,
                textConflict,
                propertyConflict);
            string description = PendingConflictLogic.GetConflictDescription(
                PendingChange.Kind,
                textConflict,
                propertyConflict);

            PendingChangeStatus pcs =
                PendingChange.Change
                ?? new PendingChangeStatus(PendingChangeKind.None);

            // SetValues follows AllColumns order, not visible-column order.
            // The old unfinished conflict page omitted the two conflict fields,
            // shifting every value after ChangeList into the wrong column.
            SetValues(
                pcs.PendingCommitText,                                      // Change
                PendingChange.ChangeList,                                  // ChangeList
                conflictType,                                              // ConflictType
                description,                                               // ConflictDescription
                GetDirectory(item),                                        // Folder
                PendingChange.FullPath,                                    // FullPath
                item.IsLocked ? PCResources.LockedValue : "",              // Locked
                PendingChangeDisplayLogic.FormatModifiedDate(item.Modified),                                   // Modified
                PendingChange.Name,                                        // Name
                string.IsNullOrEmpty(PendingChange.RelativePath)
                    ? PendingChange.Name
                    : PendingChange.RelativePath,                          // Path
                PendingChange.Project,                                     // Project
                PendingChange.FileType,                                    // Type
                SafeWorkingCopy(item));                                    // WorkingCopy
        }

        private string GetDirectory(SvnItem svnItem)
        {
            return svnItem.IsDirectory ? svnItem.FullPath : svnItem.Directory;
        }

        static string SafeWorkingCopy(SvnItem svnItem)
        {
            SvnWorkingCopy wc = svnItem.WorkingCopy;
            return wc == null ? "" : wc.FullPath;
        }

        public string FullPath
        {
            get { return _change.FullPath; }
        }
    }
}
