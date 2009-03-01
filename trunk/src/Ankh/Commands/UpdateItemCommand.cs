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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using SharpSvn;

using Ankh.UI;
using Ankh.Ids;
using Ankh.VS;
using Ankh.Selection;
using System.Collections.ObjectModel;
using System.Windows.Forms.Design;
using Ankh.Scc;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that updates an item.
    /// </summary>
    [Command(AnkhCommand.UpdateItemSpecific)]
    [Command(AnkhCommand.UpdateItemLatest)]
    [Command(AnkhCommand.UpdateItemLatestRecursive)]
    class UpdateItem : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsVersioned)
                    return;
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            SvnRevision updateTo;
            SvnDepth depth = SvnDepth.Empty;
            List<string> files = new List<string>();
            if (e.Command == AnkhCommand.UpdateItemSpecific)
            {
                IUIShell uiShell = e.GetService<IUIShell>();

                PathSelectorResult result = null;
                PathSelectorInfo info = new PathSelectorInfo("Select Items to Update",
                    e.Selection.GetSelectedSvnItems(true));

                info.CheckedFilter += delegate(SvnItem item) { return item.IsVersioned; };
                info.VisibleFilter += delegate(SvnItem item) { return item.IsVersioned; };
                info.EnableRecursive = true;
                info.RevisionStart = SvnRevision.Head;

                if (!CommandBase.Shift)
                {
                    result = uiShell.ShowPathSelector(info);
                }
                else
                {
                    result = info.DefaultResult;
                }

                if (!result.Succeeded)
                    return;

                updateTo = result.RevisionStart;

                foreach (SvnItem item in result.Selection)
                {
                    if (item.IsVersioned)
                        files.Add(item.FullPath);
                }

                depth = result.Depth;
            }
            else
            {
                updateTo = SvnRevision.Head;
                depth = e.Command == AnkhCommand.UpdateItemLatestRecursive ? SvnDepth.Infinity : SvnDepth.Empty;

                foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
                {
                    if (item.IsVersioned)
                        files.Add(item.FullPath);
                }
            }


            IAnkhOpenDocumentTracker tracker = e.GetService<IAnkhOpenDocumentTracker>();            
            tracker.SaveDocuments(e.Selection.GetSelectedFiles(true));

            SvnUpdateResult ur = null;

            e.GetService<IProgressRunner>().RunModal(CommandStrings.UpdatingTitle,
                delegate(object sender, ProgressWorkerArgs ee)
                {                    
                    SvnUpdateArgs ua = new SvnUpdateArgs();
                    ua.Depth = depth;
                    ua.Revision = updateTo;
                    e.GetService<IConflictHandler>().RegisterConflictHandler(ua, ee.Synchronizer);
                    ee.Client.Update(files, ua, out ur);
                });
        }
    }
}
