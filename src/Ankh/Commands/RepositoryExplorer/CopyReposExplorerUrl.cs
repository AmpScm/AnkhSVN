// $Id$
//
// Copyright 2004-2008 The AnkhSVN Project
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

// $Id$
using System;
using Ankh.UI;
using Clipboard = System.Windows.Forms.Clipboard;
using Ankh.Ids;
using Ankh.WorkingCopyExplorer;
using Ankh.Scc;

namespace Ankh.Commands.RepositoryExplorer
{
    /// <summary>
    /// Command to copy the URL of this item to the clipboard in Repository Explorer.
    /// </summary>
    [Command(AnkhCommand.CopyReposExplorerUrl)]
    class CopyReposExplorerUrl : CommandBase
    {
        

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            int n = 0;
            foreach (ISvnRepositoryItem i in e.Selection.GetSelection<ISvnRepositoryItem>())
            {
                n++;
                if (n > 1 || i.Uri == null)
                {
                    e.Enabled = false;
                    return;
                }
            }
            if (n == 1)
                return;

            foreach(SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                n++;
                if(n > 1 || item.Status.Uri == null)
                {
                    e.Enabled = false;
                    return;
                }
            }

            if (n != 1)
                e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            foreach (ISvnRepositoryItem i in e.Selection.GetSelection<ISvnRepositoryItem>())
            {
                if (i.Uri != null)
                    Clipboard.SetText(i.Uri.AbsoluteUri);

                return;
            }

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (item.Status.Uri != null)
                    Clipboard.SetText(item.Status.Uri.AbsoluteUri);

                return;
            }
        }
    }
}
