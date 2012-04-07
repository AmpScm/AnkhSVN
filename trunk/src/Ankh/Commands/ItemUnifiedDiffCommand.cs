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
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using SharpSvn;

using Ankh.Scc;
using Ankh.UI.PathSelector;
using Ankh.VS;
using System.Collections.Generic;

namespace Ankh.Commands
{
    [Command(AnkhCommand.UnifiedDiff)]
    class ItemUnifiedDiffCommand : CommandBase
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
            List<SvnItem> items = new List<SvnItem>();

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsModified || item.IsDocumentDirty)
                    items.Add(item);
            }

            SvnRevision start = SvnRevision.Base;
            SvnRevision end = SvnRevision.Working;

            // should we show the path selector?
            if (!Shift)
            {
                using (CommonFileSelectorDialog dlg = new CommonFileSelectorDialog())
                {
                    dlg.Text = CommandStrings.UnifiedDiffTitle;
                    dlg.Items = items;
                    dlg.RevisionStart = start;
                    dlg.RevisionEnd = end;

                    if (dlg.ShowDialog(e.Context) != DialogResult.OK)
                        return;

                    items.Clear();
                    items.AddRange(dlg.GetCheckedItems());
                    start = dlg.RevisionStart;
                    end = dlg.RevisionEnd;
                }
            }

            if (items.Count == 0)
                return;

            SvnRevisionRange revRange = new SvnRevisionRange(start, end);

            IAnkhTempFileManager tempfiles = e.GetService<IAnkhTempFileManager>();
            string tempFile = tempfiles.GetTempFile(".patch");

            IAnkhSolutionSettings ss = e.GetService<IAnkhSolutionSettings>();
            string slndir = ss.ProjectRoot;
            string slndirP = slndir + "\\";

            SvnDiffArgs args = new SvnDiffArgs();
            args.IgnoreAncestry = true;
            args.NoDeleted = false;

            using (MemoryStream stream = new MemoryStream())
            {
                e.Context.GetService<IProgressRunner>().RunModal(CommandStrings.RunningDiff,
                    delegate(object sender, ProgressWorkerArgs ee)
                    {
                        foreach (SvnItem item in items)
                        {
                            SvnWorkingCopy wc;
                            if (!string.IsNullOrEmpty(slndir) &&
                                item.FullPath.StartsWith(slndirP, StringComparison.OrdinalIgnoreCase))
                                args.RelativeToPath = slndir;
                            else if ((wc = item.WorkingCopy) != null)
                                args.RelativeToPath = wc.FullPath;
                            else
                                args.RelativeToPath = null;

                            ee.Client.Diff(item.FullPath, revRange, args, stream);
                        }

                        stream.Flush();
                        stream.Position = 0;
                    });
                using (StreamReader sr = new StreamReader(stream))
                {
                    File.WriteAllText(tempFile, sr.ReadToEnd(), Encoding.UTF8);
                    VsShellUtilities.OpenDocument(e.Context, tempFile);
                }
            }
        }
    }
}
