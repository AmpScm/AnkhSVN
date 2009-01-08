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
using Ankh.Scc;
using SharpSvn;
using Ankh.UI.RepositoryExplorer.Dialogs;
using System.Windows.Forms;

namespace Ankh.Commands.RepositoryExplorer
{
    [Command(AnkhCommand.RenameRepositoryItem, AlwaysAvailable=true)]
    class RenameNode : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            base.OnUpdate(e);

            ISvnRepositoryItem item = EnumTools.GetSingle(e.Selection.GetSelection<ISvnRepositoryItem>());

            if (item != null && item.Origin != null)
            {
                if (item.Origin.Target.Revision == SvnRevision.Head
                    && !item.Origin.IsRepositoryRoot)
                {
                    return;
                }
            }

            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            ISvnRepositoryItem item = EnumTools.GetSingle(e.Selection.GetSelection<ISvnRepositoryItem>());

            if (item == null)
                return;

            string newName = item.Origin.Target.FileName;

            if (e.Argument != null)
            {
                string[] items = e.Argument as string[];

                if (items != null)
                {
                    if (items.Length == 1)
                        newName = items[0];
                    else if (items.Length > 1)
                        newName = items[1];
                }
            }

            string logMessage;
            using (RenameDialog dlg = new RenameDialog())
            {
                dlg.Context = e.Context;
                dlg.OldName = item.Origin.Target.FileName;
                dlg.NewName = newName;

                if (DialogResult.OK != dlg.ShowDialog(e.Context))
                {
                    return;
                }
                newName = dlg.NewName;
                logMessage = dlg.LogMessage;
            }

            try
            {
                Uri itemUri = SvnTools.GetNormalizedUri(item.Origin.Uri);
                e.GetService<IProgressRunner>().RunModal(CommandStrings.RenamingNodes,
                    delegate(object sender, ProgressWorkerArgs we)
                    {
                        SvnMoveArgs ma = new SvnMoveArgs();
                        ma.LogMessage = logMessage;
                        we.Client.RemoteMove(itemUri, new Uri(itemUri, newName), ma);
                    });
            }
            finally
            {
                item.RefreshItem(true);
            }
        }
    }
}
