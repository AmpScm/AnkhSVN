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
using System.Text;
using Ankh.Scc;
using Ankh.Scc.UI;
using SharpSvn;

namespace Ankh.Commands.RepositoryExplorer
{
    [Command(AnkhCommand.RepositoryShowChanges, AlwaysAvailable = true)]
    [Command(AnkhCommand.RepositoryCompareWithWc, AlwaysAvailable = true)]
    class ShowRepositoryItemChanges : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            ISvnRepositoryItem reposItem = EnumTools.GetSingle(e.Selection.GetSelection<ISvnRepositoryItem>());

            if (reposItem != null && reposItem.Origin != null && reposItem.NodeKind != SharpSvn.SvnNodeKind.Directory
                && reposItem.Revision.RevisionType == SharpSvn.SvnRevisionType.Number)
            {
                if (e.Command == AnkhCommand.RepositoryCompareWithWc)
                {
                    if (!(reposItem.Origin.Target is SvnPathTarget))
                    {
                        e.Enabled = false;
                        return;
                    }
                }

                return;
            }

            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IAnkhDiffHandler diff = e.GetService<IAnkhDiffHandler>();
            ISvnRepositoryItem reposItem = EnumTools.GetSingle(e.Selection.GetSelection<ISvnRepositoryItem>());

            if (reposItem == null)
                return;

            SvnRevision from;
            SvnRevision to;
            if (e.Command == AnkhCommand.RepositoryCompareWithWc)
            {
                from = reposItem.Revision;
                to = SvnRevision.Working;
            }
            else
            {
                from = reposItem.Revision.Revision - 1;
                to = reposItem.Revision;
            }
            AnkhDiffArgs da = new AnkhDiffArgs();

            if (to == SvnRevision.Working)
            {
                da.BaseFile = diff.GetTempFile(reposItem.Origin.Target, from, true);

                if (da.BaseFile == null)
                    return; // User canceled

                da.MineFile = ((SvnPathTarget)reposItem.Origin.Target).FullPath;
            }
            else
            {
                string[] files = diff.GetTempFiles(reposItem.Origin.Target, from, to, true);

                if (files == null)
                    return; // User canceled
                da.BaseFile = files[0];
                da.MineFile = files[1];
                System.IO.File.SetAttributes(da.MineFile, System.IO.FileAttributes.ReadOnly | System.IO.FileAttributes.Normal);
            }

            da.BaseTitle = diff.GetTitle(reposItem.Origin.Target, from);
            da.MineTitle = diff.GetTitle(reposItem.Origin.Target, to);
            diff.RunDiff(da);
        }
    }
}
