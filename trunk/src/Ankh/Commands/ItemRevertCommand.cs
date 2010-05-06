// $Id$
//
// Copyright 2003-2009 The AnkhSVN Project
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
using System.Windows.Forms;
using SharpSvn;
using Ankh.Scc;
using Ankh.VS;
using Ankh.UI;
using System.Collections.Generic;
using Ankh.UI.PathSelector;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to revert current item to last updated revision.
    /// </summary>
    [Command(AnkhCommand.RevertItem)]
    [Command(AnkhCommand.ItemRevertBase, HideWhenDisabled = false)]
    class RevertItemCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsModified || (item.IsVersioned && item.IsDocumentDirty) || item.IsConflicted)
                    return;
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            List<SvnItem> toRevert = new List<SvnItem>();
            HybridCollection<string> contained = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            HybridCollection<string> checkedItems = null;

            foreach (SvnItem i in e.Selection.GetSelectedSvnItems(false))
            {
                if (contained.Contains(i.FullPath))
                    continue;

                contained.Add(i.FullPath);

                if (i.IsModified || (i.IsVersioned && i.IsDocumentDirty) || i.IsConflicted)
                    toRevert.Add(i);
            }

            Predicate<SvnItem> initialCheckedFilter = null;
            if (toRevert.Count > 0)
            {
                checkedItems = new HybridCollection<string>(contained, StringComparer.OrdinalIgnoreCase);

                initialCheckedFilter = delegate(SvnItem item)
                    {
                        return checkedItems.Contains(item.FullPath);
                    };
            }

            foreach (SvnItem i in e.Selection.GetSelectedSvnItems(true))
            {
                if (contained.Contains(i.FullPath))
                    continue;

                contained.Add(i.FullPath);

                if (i.IsModified || (i.IsVersioned && i.IsDocumentDirty))
                    toRevert.Add(i);
            }

            if (e.PromptUser || (!e.DontPrompt && !Shift))
            {
                using (PendingChangeSelector pcs = new PendingChangeSelector())
                {
                    pcs.Text = CommandStrings.RevertDialogTitle;
                    pcs.LoadItems(toRevert, null, initialCheckedFilter);

                    if (pcs.ShowDialog(e.Context) != DialogResult.OK)
                        return;

                    toRevert.Clear();
                    toRevert.AddRange(pcs.GetSelectedItems());
                }
            }


            IAnkhOpenDocumentTracker documentTracker = e.GetService<IAnkhOpenDocumentTracker>();

            ICollection<string> revertPaths = SvnItem.GetPaths(toRevert);
            documentTracker.SaveDocuments(revertPaths);

            // perform the actual revert 
            using (DocumentLock dl = documentTracker.LockDocuments(revertPaths, DocumentLockType.NoReload))
            using (dl.MonitorChangesForReload())
            {
                e.GetService<IProgressRunner>().RunModal(CommandStrings.Reverting,
                delegate(object sender, ProgressWorkerArgs a)
                {
                    SvnRevertArgs ra = new SvnRevertArgs();
                    ra.Depth = SvnDepth.Empty;
                    ra.AddExpectedError(SvnErrorCode.SVN_ERR_WC_NOT_DIRECTORY); // Parent revert invalidated this change

                    foreach (SvnItem item in toRevert)
                    {
                        a.Client.Revert(item.FullPath, ra);
                    }
                });
            }
        }
    }
}
