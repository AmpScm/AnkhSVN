// $Id$
//
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
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;
using Ankh.Commands;
using Ankh.Scc.UI;
using Ankh.Scc;
using SharpSvn;

namespace Ankh.UI.SvnLog.Commands
{
    [Command(AnkhCommand.LogRevertThisRevisions, AlwaysAvailable = true)]
    [Command(AnkhCommand.LogRevertTo, AlwaysAvailable = true)]
    class RevertChanges : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            ILogControl logWindow = e.Selection.ActiveDialogOrFrameControl as ILogControl;

            if (logWindow == null)
            {
                e.Enabled = false;
                return;
            }

            SvnOrigin origin = EnumTools.GetSingle(logWindow.Origins);

            if (origin == null || !(origin.Target is SvnPathTarget))
            {
                e.Enabled = false;
                return;
            }

            int count = 0;
            foreach (ISvnLogItem item in e.Selection.GetSelection<ISvnLogItem>())
            {
                count++;

                if (count > 1)
                    break;
            }

            switch (e.Command)
            {
                case AnkhCommand.LogRevertTo:
                    if (count == 1)
                        return;
                    break;
                case AnkhCommand.LogRevertThisRevisions:
                    if (count > 0)
                        return;
                    break;
            }
            e.Enabled = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            ILogControl logWindow = e.Selection.ActiveDialogOrFrameControl as ILogControl;
            IProgressRunner progressRunner = e.GetService<IProgressRunner>();

            if (logWindow == null)
                return;

            List<SvnRevisionRange> revisions = new List<SvnRevisionRange>();

            if (e.Command == AnkhCommand.LogRevertTo)
            {
                ISvnLogItem item = EnumTools.GetSingle(e.Selection.GetSelection<ISvnLogItem>());

                if (item == null)
                    return;

                // Revert to revision, is revert everything after
                revisions.Add(new SvnRevisionRange(SvnRevision.Working, item.Revision));
            }
            else
            {
                foreach (ISvnLogItem item in e.Selection.GetSelection<ISvnLogItem>())
                {
                    revisions.Add(new SvnRevisionRange(item.Revision, item.Revision - 1));
                }
            }

            if (revisions.Count == 0)
                return;

            IAnkhOpenDocumentTracker tracker = e.GetService<IAnkhOpenDocumentTracker>();

            HybridCollection<string> nodes = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);

            foreach (SvnOrigin o in logWindow.Origins)
            {
                SvnPathTarget pt = o.Target as SvnPathTarget;
                if (pt == null)
                    continue;

                foreach (string file in tracker.GetDocumentsBelow(pt.FullPath))
                {
                    if (!nodes.Contains(file))
                        nodes.Add(file);
                }
            }

            if (nodes.Count > 0)
                tracker.SaveDocuments(nodes); // Saves all open documents below all specified origins


            using (DocumentLock dl = tracker.LockDocuments(nodes, DocumentLockType.NoReload))
            using (dl.MonitorChangesForReload())
            {
                SvnMergeArgs ma = new SvnMergeArgs();

                progressRunner.RunModal("Reverting",
                delegate(object sender, ProgressWorkerArgs ee)
                {
                    foreach (SvnOrigin item in logWindow.Origins)
                    {
                        SvnPathTarget target = item.Target as SvnPathTarget;

                        if (target == null)
                            continue;

                        ee.Client.Merge(target.FullPath, target, revisions, ma);
                    }
                });
            }
        }
    }
}
