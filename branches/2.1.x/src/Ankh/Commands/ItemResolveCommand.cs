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

using SharpSvn;

using Ankh.Scc;
using Ankh.UI;

namespace Ankh.Commands
{
    [Command(AnkhCommand.ItemResolveMerge)]
    [Command(AnkhCommand.ItemResolveMineFull)]
    [Command(AnkhCommand.ItemResolveTheirsFull)]
    [Command(AnkhCommand.ItemResolveMineConflict)]
    [Command(AnkhCommand.ItemResolveTheirsConflict)]
    [Command(AnkhCommand.ItemResolveBase)]
    [Command(AnkhCommand.ItemResolveWorking)]
    [Command(AnkhCommand.ItemResolveMergeTool)]
    class ItemResolveCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            bool foundOne = false;
            bool canDiff = true;
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (!item.IsConflicted)
                    continue;

                foundOne = true;

                if (!item.IsTextFile)
                {
                    canDiff = false;
                    break;
                }
            }

            if (!foundOne)
                e.Enabled = false;
            else if (!canDiff && (e.Command == AnkhCommand.ItemResolveTheirsConflict || e.Command == AnkhCommand.ItemResolveMineConflict))
                e.Enabled = false;
            else if (e.Command == AnkhCommand.ItemResolveMergeTool)
                e.Enabled = false;
            else if (e.Command == AnkhCommand.ItemResolveMergeTool && string.IsNullOrEmpty(e.GetService<IAnkhConfigurationService>().Instance.MergeExePath))
                e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            switch (e.Command)
            {
                case AnkhCommand.ItemResolveMerge:
                    Resolved(e);
                    break;
                case AnkhCommand.ItemResolveMergeTool:
                    throw new NotImplementedException();
                case AnkhCommand.ItemResolveMineFull:
                    Resolve(e, SvnAccept.MineFull);
                    break;
                case AnkhCommand.ItemResolveTheirsFull:
                    Resolve(e, SvnAccept.TheirsFull);
                    break;
                case AnkhCommand.ItemResolveWorking:
                    Resolve(e, SvnAccept.Merged);
                    break;
                case AnkhCommand.ItemResolveBase:
                    Resolve(e, SvnAccept.Base);
                    break;
                case AnkhCommand.ItemResolveMineConflict:
                    Resolve(e, SvnAccept.Mine);
                    break;
                case AnkhCommand.ItemResolveTheirsConflict:
                    Resolve(e, SvnAccept.Theirs);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        static void Resolve(CommandEventArgs e, SvnAccept accept)
        {
            HybridCollection<string> paths = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (!item.IsConflicted)
                    continue;

                if (!paths.Contains(item.FullPath))
                    paths.Add(item.FullPath);
            }


            IAnkhOpenDocumentTracker documentTracker = e.GetService<IAnkhOpenDocumentTracker>();
            documentTracker.SaveDocuments(paths); // Make sure all files are saved before updating/merging!

            using (DocumentLock lck = documentTracker.LockDocuments(paths, DocumentLockType.NoReload))
            using (lck.MonitorChangesForReload())
            using (SvnClient client = e.GetService<ISvnClientPool>().GetNoUIClient())
            {
                SvnResolveArgs a = new SvnResolveArgs();
                a.Depth = SvnDepth.Empty;

                foreach (string p in paths)
                {
                    client.Resolve(p, accept, a);
                }
            }
        }

        static void Resolved(CommandEventArgs e)
        {
            using (SvnClient client = e.GetService<ISvnClientPool>().GetNoUIClient())
            {
                foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
                {
                    if (!item.IsConflicted)
                        continue;

                    client.Resolved(item.FullPath);
                }
            }
        }
    }
}

