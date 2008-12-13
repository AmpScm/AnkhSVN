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

using System;
using Ankh.UI;
using System.Windows.Forms;
using SharpSvn;
using Ankh.Ids;
using System.Windows.Forms.Design;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to export a Subversion repository or local folder.
    /// </summary>
    [Command(AnkhCommand.Export,HideWhenDisabled=false)]
    class ExportCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            bool foundOne = false;
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if(foundOne || !item.IsVersioned)
                {
                    e.Enabled = false;
                    break;
                }
                foundOne = true;
            }

            if(!foundOne)
                e.Enabled = false;
        }
        public override void OnExecute(CommandEventArgs e)
        {
            using (ExportDialog dlg = new ExportDialog(e.Context))
            {
                IUIService ui = e.GetService<IUIService>();

                foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                {
                    dlg.OriginUri = item.Status.Uri;
                    dlg.OriginPath = item.FullPath;
                }

                DialogResult dr;

                if (ui != null)
                    dr = ui.ShowDialog(dlg);
                else
                    dr = dlg.ShowDialog();

                if (dr != DialogResult.OK)
                    return;

                e.GetService<IProgressRunner>().RunModal("Exporting",
                    delegate(object sender, ProgressWorkerArgs wa)
                    {
                        SvnExportArgs args = new SvnExportArgs();
                        args.Depth = dlg.NonRecursive ? SvnDepth.Infinity : SvnDepth.Empty;
                        args.Revision = dlg.Revision;

                        wa.Client.Export(dlg.ExportSource, dlg.LocalPath, args);
                    });
            }
        }
    }
}
