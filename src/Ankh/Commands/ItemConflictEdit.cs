// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
using System.Text;
using Ankh.Ids;
using SharpSvn;
using Ankh.Scc.UI;
using System.IO;

namespace Ankh.Commands
{
    [Command(AnkhCommand.ItemConflictEdit)]
    [Command(AnkhCommand.DocumentConflictEdit)]
    [Command(AnkhCommand.ItemConflictEditVisualStudio)]
    class ItemConflictEdit : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (e.Command == AnkhCommand.DocumentConflictEdit)
            {
                SvnItem item = e.Selection.ActiveDocumentItem;

                if (item != null && item.IsConflicted)
                    return;
            }
            else
                foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                {
                    if (item.IsConflicted)
                        return;
                }

            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            // TODO: Choose which conflict to edit if we have more than one!
            SvnItem conflict = null;

            if (e.Command == AnkhCommand.DocumentConflictEdit)
            {
                conflict = e.Selection.ActiveDocumentItem;

                if (conflict == null || !conflict.IsConflicted)
                    return;
            }
            else
                foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                {
                    if (item.IsConflicted)
                    {
                        conflict = item;
                        break;
                    }
                }

            if (conflict == null)
                return;

            SvnInfoEventArgs conflictInfo = null;

            bool ok = false;
            ProgressRunnerResult r = e.GetService<IProgressRunner>().RunModal("Retrieving Conflict Information",
                delegate(object sender, ProgressWorkerArgs a)
                {
                    ok = a.Client.GetInfo(conflict.FullPath, out conflictInfo);
                });

            if (!ok || !r.Succeeded || conflictInfo == null)
                return;

            AnkhMergeArgs da = new AnkhMergeArgs();
            string dir = conflict.Directory;

            da.BaseFile = Path.Combine(dir, conflictInfo.ConflictOld ?? conflictInfo.ConflictNew);
            da.TheirsFile = Path.Combine(dir, conflictInfo.ConflictNew ?? conflictInfo.ConflictOld);

            if (!string.IsNullOrEmpty(conflictInfo.ConflictWork))
                da.MineFile = Path.Combine(dir, conflictInfo.ConflictWork);
            else
                da.MineFile = conflict.FullPath;

            da.MergedFile = conflict.FullPath;

            da.BaseTitle = "Base";
            da.TheirsTitle = "Theirs";
            da.MineTitle = "Mine";
            da.MergedTitle = conflict.Name;
            

            e.GetService<IAnkhDiffHandler>().RunMerge(da);
        }
    }
}
