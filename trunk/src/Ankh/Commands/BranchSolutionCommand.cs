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
using Ankh.VS;
using Ankh.Scc;
using SharpSvn;
using Ankh.UI.SccManagement;
using System.Windows.Forms;
using Ankh.UI;

namespace Ankh.Commands
{
    [Command(AnkhCommand.ProjectBranch)]
    [Command(AnkhCommand.SolutionBranch)]
    class BranchSolutionCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            switch (e.Command)
            {
                case AnkhCommand.SolutionBranch:
                    IAnkhSolutionSettings ss = e.GetService<IAnkhSolutionSettings>();

                    string root = ss.ProjectRoot;

                    if (string.IsNullOrEmpty(root))
                    {
                        e.Enabled = false;
                        return;
                    }

                    SvnItem item = e.GetService<IFileStatusCache>()[root];

                    if (item == null || !item.IsVersioned || item.IsDeleteScheduled || item.Status.LocalContentStatus == SvnStatus.Added || item.Status.Uri == null)
                    {
                        e.Enabled = false;
                        return;
                    }
                    return;
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            string path;

            switch (e.Command)
            {
                case AnkhCommand.SolutionBranch:
                    IAnkhSolutionSettings ss = e.GetService<IAnkhSolutionSettings>();

                    path = ss.ProjectRoot;
                    break;
                default:
                    return;
            }

            if (string.IsNullOrEmpty(path))
                return;

            IFileStatusCache cache = e.GetService<IFileStatusCache>();
            SvnItem root;

            if (cache == null || null == (root = cache[path]) || root.Status.Uri == null)
                return;

            using (CreateBranchDialog dlg = new CreateBranchDialog())
            {
                dlg.SrcFolder = root.FullPath;
                dlg.SrcUri = root.Status.Uri;
                dlg.EditSource = false;

                dlg.Revision = root.Status.Revision;

                RepositoryLayoutInfo info;
                if (RepositoryUrlUtils.TryGuessLayout(e.Context, root.Status.Uri, out info))
                    dlg.NewDirectoryName = new Uri(info.BranchesRoot, ".");

                while(true)
                {
                    if (DialogResult.OK != dlg.ShowDialog(e.Context))
                        return;

                    string msg = dlg.LogMessage;

                    bool retry = false;
                    bool ok = false;
                    ProgressRunnerResult rr =
                        e.GetService<IProgressRunner>().RunModal("Creating Branch/Tag",
                        delegate(object sender, ProgressWorkerArgs ee)
                        {
                            SvnInfoArgs ia = new SvnInfoArgs();
                            ia.ThrowOnError =false;

                            if(ee.Client.Info(dlg.NewDirectoryName, ia, null))
                            {
                                DialogResult dr = DialogResult.Cancel;

                                ee.Synchronizer.Invoke((AnkhAction)
                                    delegate
                                    {
                                        AnkhMessageBox mb = new AnkhMessageBox(ee.Context);

                                        dr = mb.Show(string.Format("The Branch/Tag at Url {0} already exists, would you like to overwrite it with the specified branch?", dlg.NewDirectoryName),
                                            "Path Exists", MessageBoxButtons.YesNoCancel);
                                    }, null);

                                if (dr == DialogResult.No)
                                {
                                    retry = true;
                                    return;
                                }
                                else if (dr != DialogResult.Yes)
                                    return;
                            }

                            SvnCopyArgs ca = new SvnCopyArgs();
                            ca.CreateParents = true;
                            ca.LogMessage = msg;

                            if (dlg.CopyFromUri)
                                ok = ee.Client.RemoteCopy(new SvnUriTarget(dlg.SrcUri, dlg.SelectedRevision), dlg.NewDirectoryName, ca);
                            else
                                ok = ee.Client.RemoteCopy(new SvnPathTarget(dlg.SrcFolder), dlg.NewDirectoryName, ca);
                        });

                    if(rr.Succeeded && ok && dlg.SwitchToBranch)
                    {
                        e.GetService<IAnkhCommandService>().PostExecCommand(AnkhCommand.SolutionSwitchDialog, dlg.NewDirectoryName);
                    }

                    if (!retry)
                        break;
                }
                
            }
        }
    }
}
