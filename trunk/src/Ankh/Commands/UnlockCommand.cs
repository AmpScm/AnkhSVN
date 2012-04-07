// $Id$
//
// Copyright 2005-2009 The AnkhSVN Project
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

using System.Collections.Generic;
using System.Windows.Forms;
using SharpSvn;

using Ankh.Configuration;
using Ankh.UI;
using Ankh.UI.PathSelector;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to unlock the selected items.
    /// </summary>
    [Command(AnkhCommand.Unlock)]
    class UnlockCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (item.IsLocked)
                    return;

            }
            e.Enabled = false; // No need to unlock anything if we are not versioned or not locked
        }

        public override void OnExecute(CommandEventArgs e)
        {
            AnkhConfig config = e.GetService<IAnkhConfigurationService>().Instance;
            ICollection<SvnItem> targets;

            if (!e.DontPrompt && (e.PromptUser || (!Shift && !config.SuppressLockingUI)))
            {
                using (PendingChangeSelector selector = new PendingChangeSelector())
                {
                    selector.Text = CommandStrings.UnlockTitle;
                    selector.PreserveWindowPlacement = true;

                    selector.LoadItems(new List<SvnItem>(e.Selection.GetSelectedSvnItems(true)),
                                       delegate(SvnItem i) { return i.IsLocked; },
                                       delegate(SvnItem i) { return i.IsLocked; });

                    if (selector.ShowDialog(e.Context) != DialogResult.OK)
                        return;

                    targets = new List<SvnItem>(selector.GetSelectedItems());
                }
            }
            else
            {
                List<SvnItem> toUnlock = new List<SvnItem>();
                foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
                {
                    if (item.IsLocked)
                        toUnlock.Add(item);
                }
                targets = toUnlock;
            }

            if (targets.Count == 0)
                return;

            List<string> files = new List<string>(SvnItem.GetPaths(targets));

            if (files.Count == 0)
                return;

            e.GetService<IProgressRunner>().RunModal(
                CommandStrings.UnlockTitle,
                delegate(object sender, ProgressWorkerArgs ee)
                {
                    SvnUnlockArgs ua = new SvnUnlockArgs();

                    ee.Client.Unlock(files, ua);
                });
        }
    }
}
