// $Id$
//
// Copyright 2003-2008 The AnkhSVN Project
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
using Ankh.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to add selected items to the working copy.
    /// </summary>
    [Command(AnkhCommand.AddItem)]
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
            IUIShell uiShell = e.GetService<IUIShell>();

            
            PathSelectorInfo info = new PathSelectorInfo("Select items to add",
                e.Selection.GetSelectedSvnItems(true));

            info.CheckedFilter += delegate(SvnItem item) { return !item.IsVersioned && !item.IsIgnored && item.IsVersionable; };
            info.VisibleFilter += delegate(SvnItem item) { return !item.IsVersioned && item.IsVersionable; };

            PathSelectorResult result;
            // are we shifted?
            if (!Shift)
            {
                info.EnableRecursive = false;

                result = uiShell.ShowPathSelector(info);
            }
            else
                result = info.DefaultResult;
                
            if (!result.Succeeded)
                return;

            e.GetService<IProgressRunner>().RunModal("Adding",
                delegate(object sender, ProgressWorkerArgs ee)
                {
                    SvnAddArgs args = new SvnAddArgs();
                    args.ThrowOnError = false;
                    args.Depth = SvnDepth.Empty;
                    args.AddParents = true;

                    foreach (SvnItem item in result.Selection)
                    {
                        ee.Client.Add(item.FullPath, args);
                    }
                });
        }
    }
}
