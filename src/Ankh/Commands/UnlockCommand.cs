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

using Ankh.Scc;
using Ankh.UI;
using Ankh.UI.PathSelector;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to unlock the selected items.
    /// </summary>
    [Command(AnkhCommand.Unlock, HideWhenDisabled = true)]
    class UnlockCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsLocked)
                    return;

            }
            e.Enabled = false; // No need to unlock anything if we are not versioned or not locked
        }

        public override void OnExecute(CommandEventArgs e)
        {
            PathSelectorInfo info = new PathSelectorInfo("Select Files to Unlock", e.Selection.GetSelectedSvnItems(true));

            info.VisibleFilter += delegate(SvnItem item)
                                     {
                                         return item.IsLocked;
                                     };

            info.CheckedFilter += delegate(SvnItem item)
                                     {
                                         return item.IsLocked;
                                     };

            PathSelectorResult result;
            if (!Shift)
            {
                using (PathSelector selector = new PathSelector(info))
                {
                    selector.Context = e.Context;

                    bool succeeded = selector.ShowDialog(e.Context) == DialogResult.OK;
                    result = new PathSelectorResult(succeeded, selector.CheckedItems);
                    result.Depth = selector.Recursive ? SvnDepth.Infinity : SvnDepth.Empty;
                    result.RevisionStart = selector.RevisionStart;
                    result.RevisionEnd = selector.RevisionEnd;
                }
            }
            else
                result = info.DefaultResult;

            if (!result.Succeeded)
                return;

            List<string> files = new List<string>();

            foreach (SvnItem item in result.Selection)
            {
                files.Add(item.FullPath);
            }

            if (files.Count == 0)
                return;

            e.GetService<IProgressRunner>().RunModal(
                "Unlocking",
                delegate(object sender, ProgressWorkerArgs ee)
                {
                    SvnUnlockArgs ua = new SvnUnlockArgs();

                    ee.Client.Unlock(files, ua);
                });
        }
    }
}
