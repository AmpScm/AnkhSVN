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
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using SharpSvn;
using Ankh.Scc;
using Ankh.UI.PathSelector;
using Ankh.VS;

namespace Ankh.Commands
{
    [SvnCommand(AnkhCommand.UnifiedDiff)]
    class UnifiedDiffCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsVersioned)
                    return;
                else if (item.IsFile && item.IsVersionable && item.InSolution && !item.IsIgnored && !item.IsSccExcluded)
                    return; // New files can be added
            }

            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            List<string> toAdd = new List<string>();
            List<SvnItem> items = new List<SvnItem>();

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsVersioned)
                {
                    items.Add(item);
                }
                else if (item.IsFile && item.IsVersionable && item.InSolution && !item.IsIgnored && !item.IsSccExcluded)
                {
                    toAdd.Add(item.FullPath); // Add new files  ### Alternative: Show them as added
                    items.Add(item);
                }
            }

            if (items.Count == 0)
                return;

            SvnRevision start = SvnRevision.Base;
            SvnRevision end = SvnRevision.Working;

            // should we show the path selector?
            if (e.ShouldPrompt(true))
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

            using (MemoryStream stream = new MemoryStream())
            {
                e.Context.GetService<IProgressRunner>().RunModal(CommandStrings.RunningDiff,
                    delegate(object sender, ProgressWorkerArgs ee)
                    {
                        SvnAddArgs aa = new SvnAddArgs();
                        aa.ThrowOnError = false;
                        aa.AddParents = false;
                        foreach (string item in toAdd)
                        {
                            ee.Client.Add(item, aa);
                        }

                        SvnDiffArgs diffArgs = new SvnDiffArgs();
                        diffArgs.IgnoreAncestry = true;
                        diffArgs.NoDeleted = false;
                        diffArgs.ThrowOnError = false;

                        foreach (SvnItem item in items)
                        {
                            SvnWorkingCopy wc;
                            if (!string.IsNullOrEmpty(slndir) && item.IsBelowPath(slndir))
                                diffArgs.RelativeToPath = slndir;
                            else if ((wc = item.WorkingCopy) != null)
                                diffArgs.RelativeToPath = wc.FullPath;
                            else
                                diffArgs.RelativeToPath = null;

                            if (!ee.Client.Diff(item.FullPath, revRange, diffArgs, stream))
                            {
                                if (diffArgs.LastException != null)
                                {
                                    StreamWriter sw = new StreamWriter(stream);
                                    sw.WriteLine();
                                    sw.WriteLine(string.Format("# {0}: {1}", item.FullPath, diffArgs.LastException.Message));
                                    sw.Flush();
                                    // Don't dispose the writer as that might close the stream
                                }

                                if (diffArgs.IsLastInvocationCanceled)
                                    break;
                            }
                        }

                        stream.Flush();
                    });

                stream.Position = 0;
                using (StreamReader sr = new StreamReader(stream))
                {
                    File.WriteAllText(tempFile, sr.ReadToEnd(), Encoding.UTF8);
                    VsShellUtilities.OpenDocument(e.Context, tempFile);
                }
            }
        }
    }
}
