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

using Ankh.VS;
using Ankh.UI;
using Ankh.Scc.UI;
using System.Windows.Forms;

namespace Ankh.Commands
{
    [Command(Ids.AnkhCommand.SolutionApplyPatch)]
    [Command(Ids.AnkhCommand.PendingChangesApplyPatch, HideWhenDisabled=false)]
    public class ApplyPatch : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            IAnkhSolutionSettings ss = e.GetService<IAnkhSolutionSettings>();

            if (ss != null && !string.IsNullOrEmpty(ss.ProjectRoot) && ss.ProjectRootSvnItem.IsVersioned)
            {
                IAnkhConfigurationService cs = e.GetService<IAnkhConfigurationService>();

                if (!string.IsNullOrEmpty(cs.Instance.PatchExePath))
                    return;
            }

            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IAnkhSolutionSettings ss = e.GetService<IAnkhSolutionSettings>();
            IAnkhDiffHandler diff = e.GetService<IAnkhDiffHandler>();

            AnkhPatchArgs args = new AnkhPatchArgs();
            args.ApplyTo = ss.ProjectRoot;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Patch files( *.patch)|*.patch|Diff files (*.diff)|*.diff|" +
                    "Text files (*.txt)|*.txt|All files (*.*)|*";

                if (ofd.ShowDialog(e.Context.DialogOwner) != DialogResult.OK)
                    return;

                args.PatchFile = ofd.FileName;
            }

            diff.RunPatch(args);
        }
    }
}
