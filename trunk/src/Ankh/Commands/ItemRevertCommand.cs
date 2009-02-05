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
using System.Collections;
using System.Windows.Forms;
using Ankh.Ids;
using SharpSvn;
using System.Collections.Generic;
using Ankh.Scc;
using Ankh.VS;
using Ankh.UI;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to revert current item to last updated revision.
    /// </summary>
    [Command(AnkhCommand.RevertItem)]
    [Command(AnkhCommand.ItemRevertBase, HideWhenDisabled=false)]
    class RevertItemCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsModified || (item.IsVersioned && item.IsDocumentDirty))
                    return;
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IUIShell uiShell = e.GetService<IUIShell>();
            IAnkhDialogOwner dialogOwner = e.GetService<IAnkhDialogOwner>();
            IAnkhOpenDocumentTracker documentTracker = e.GetService<IAnkhOpenDocumentTracker>();

            SvnDepth depth = SvnDepth.Empty;
            bool confirmed = false;

            PathSelectorResult result = null;
            PathSelectorInfo info = new PathSelectorInfo("Select items to revert",
                e.Selection.GetSelectedSvnItems(true));

            info.CheckedFilter += delegate(SvnItem item) { return item.IsModified || (item.IsVersioned && item.IsDocumentDirty); };
            info.VisibleFilter += delegate(SvnItem item) { return item.IsModified || (item.IsVersioned && item.IsDocumentDirty); };

            if (!CommandBase.Shift &&
                e.Command == AnkhCommand.RevertItem)
            {
                //if(e.Command == AnkhCommand.ItemRevertSpecific)
                //    info.RevisionStart = SvnRevision.Base;

                result = uiShell.ShowPathSelector(info);

                confirmed = true;
                depth = info.Depth;
            }
            else
            {
                result = info.DefaultResult;
            }

            if (!result.Succeeded)
                return;

            SaveAllDirtyDocuments(e.Selection, e.Context);

            string[] paths = new string[result.Selection.Count];
            for (int i = 0; i < paths.Length; i++)
                paths[i] = result.Selection[i].FullPath;

            // ask for confirmation if the Shift dialog hasn't been used
            if (!confirmed)
            {
                string msg = "Do you really want to revert these item(s)?" +
                    Environment.NewLine + Environment.NewLine;
                msg += string.Join(Environment.NewLine, paths);

                if (dialogOwner.MessageBox.Show(msg, "Revert", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information) != DialogResult.Yes)
                {
                    return;
                }
            }

            // perform the actual revert 
            using (DocumentLock dl = documentTracker.LockDocuments(paths, DocumentLockType.NoReload))
            {
                dl.MonitorChanges();

                SvnRevertArgs args = new SvnRevertArgs();
                
                args.Depth = depth;
                args.ThrowOnError = false;
                using (SvnClient client = e.Context.GetService<ISvnClientPool>().GetClient())
                {
                    client.Revert(paths, args);
                }

                dl.ReloadModified();
            }
        }
    }
}
