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

using Ankh.UI;
using SharpSvn;
using Ankh.UI.PathSelector;
using System.Windows.Forms;
using System.Collections.Generic;
using Ankh.Scc;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to add selected items to the working copy.
    /// </summary>
    [Command(AnkhCommand.AddItem, ArgumentDefinition = "d")]
    class AddItemCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsVersioned)
                    continue;
                if (item.IsVersionable)
                    return; // We found an add item
            }

            e.Visible = e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            string argumentFile = e.Argument as string;
            List<SvnItem> selection = new List<SvnItem>();

            if (string.IsNullOrEmpty(argumentFile))
            {
                if (e.PromptUser || (!e.DontPrompt && !Shift))
                {
                    selection.AddRange(e.Selection.GetSelectedSvnItems(true));

                    using (PendingChangeSelector pcs = new PendingChangeSelector())
                    {
                        pcs.Text = CommandStrings.AddDialogTitle;

                        pcs.LoadItems(selection,
                                      delegate(SvnItem item) { return !item.IsIgnored || !item.InSolution; },
                                    delegate(SvnItem item) { return !item.IsVersioned && item.IsVersionable; });

                        if (pcs.ShowDialog(e.Context) != DialogResult.OK)
                            return;

                        selection.Clear();
                        selection.AddRange(pcs.GetSelectedItems());
                    }
                }
                else
                {
                    foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
                    {
                        if (!item.IsVersioned && item.IsVersionable && !item.IsIgnored && item.InSolution)
                            selection.Add(item);
                    }
                }
            }
            else
            {
                selection.Add(e.GetService<IFileStatusCache>()[argumentFile]);
            }

            ICollection<string> paths = SvnItem.GetPaths(selection);
            IAnkhOpenDocumentTracker documentTracker = e.GetService<IAnkhOpenDocumentTracker>();
            documentTracker.SaveDocuments(paths); // Make sure all files are saved before updating/merging!

            using (DocumentLock lck = documentTracker.LockDocuments(paths, DocumentLockType.NoReload))
            using (lck.MonitorChangesForReload())
                e.GetService<IProgressRunner>().RunModal(CommandStrings.AddTaskDialogTitle,
                    delegate(object sender, ProgressWorkerArgs ee)
                    {
                        SvnAddArgs args = new SvnAddArgs();
                        args.Depth = SvnDepth.Empty;
                        args.AddParents = true;

                        foreach (SvnItem item in selection)
                        {
                            ee.Client.Add(item.FullPath, args);
                        }
                    });
        }
    }
}
