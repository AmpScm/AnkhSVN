// $Id$
//
// Copyright 2009 The AnkhSVN Project
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
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Ankh.Scc;
using Ankh.VS;
using Ankh.UI.PathSelector;

namespace Ankh.Commands
{
    [Command(AnkhCommand.CreatePatch)]
    class ItemCreatePatch : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem i in e.Selection.GetSelectedSvnItems(true))
            {
                if (i.IsVersioned)
                {
                    if (i.IsModified || i.IsDocumentDirty)
                        return; // There might be a new version of this file
                }
                else if (i.IsIgnored)
                    continue;
                else if (i.InSolution && i.IsVersionable)
                    return; // The file is 'to be added'
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IPendingChangesManager pcm = e.GetService<IPendingChangesManager>();
            Dictionary<string, PendingChange> changes = new Dictionary<string, PendingChange>(StringComparer.OrdinalIgnoreCase);

            foreach (PendingChange pc in pcm.GetAll())
            {
                if (!changes.ContainsKey(pc.FullPath))
                    changes.Add(pc.FullPath, pc);
            }

            Dictionary<string, SvnItem> selectedChanges = new Dictionary<string, SvnItem>(StringComparer.OrdinalIgnoreCase);
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (changes.ContainsKey(item.FullPath) &&
                    !selectedChanges.ContainsKey(item.FullPath))
                {
                    selectedChanges.Add(item.FullPath, item);
                }
            }

            Collection<SvnItem> resources = new Collection<SvnItem>();
            List<SvnItem> selectedItems = new List<SvnItem>(selectedChanges.Values);

            // TODO: Give the whole list to a refreshable dialog!
            foreach (SvnItem item in selectedItems)
            {
                PendingChange pc = changes[item.FullPath];

                if (pc.IsChangeForPatching())
                    continue;

                resources.Add(item);
            }
            if (resources.Count == 0)
                return;

            using (PendingChangeSelector pcs = new PendingChangeSelector())
            {
                pcs.Context = e.Context;
                pcs.Text = CommandStrings.CreatePatchTitle;
                pcs.LoadItems(e.Selection.GetSelectedSvnItems(true));

                DialogResult dr = pcs.ShowDialog(e.Context);

                if (dr != DialogResult.OK)
                    return;

                string fileName = GetFileName(e.Context.DialogOwner);
                if (string.IsNullOrEmpty(fileName))
                {
                    return;
                }

                PendingChangeCreatePatchArgs pca = new PendingChangeCreatePatchArgs();
                pca.FileName = fileName;
                IAnkhSolutionSettings ss = e.GetService<IAnkhSolutionSettings>();
                pca.RelativeToPath = ss.ProjectRoot;
                pca.AddUnversionedFiles = true;

                List<PendingChange> patchChanges = new List<PendingChange>(pcs.GetSelection());
                e.GetService<IPendingChangeHandler>().CreatePatch(patchChanges, pca);
            }            
        }

        private static string GetFileName(IWin32Window dialogOwner)
        {
            string fileName = null;
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Patch files( *.patch)|*.patch|Diff files (*.diff)|*.diff|" +
                    "Text files (*.txt)|*.txt|All files (*.*)|*";
                dlg.AddExtension = true;

                if (dlg.ShowDialog(dialogOwner) == DialogResult.OK)
                {
                    fileName = dlg.FileName;
                }
            }
            return fileName;
        }
    }
}
