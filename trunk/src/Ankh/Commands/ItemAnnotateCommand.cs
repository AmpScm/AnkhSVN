// $Id$
//
// Copyright 2005-2009 The AnkhSVN Project
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
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.TextManager.Interop;
using Ankh.Scc;
using Ankh.Scc.UI;
using Ankh.UI;
using Ankh.UI.Annotate;
using Ankh.VS;
using SharpSvn;
using System.Collections.Generic;
using Ankh.UI.Commands;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to identify which users to blame for which lines.
    /// </summary>
    [SvnCommand(AnkhCommand.ItemAnnotate)]
    [SvnCommand(AnkhCommand.LogAnnotateRevision)]
    [SvnCommand(AnkhCommand.SvnNodeAnnotate)]
    [SvnCommand(AnkhCommand.DocumentAnnotate)]
    class ItemAnnotateCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            switch (e.Command)
            {
                case AnkhCommand.SvnNodeAnnotate:
                    ISvnRepositoryItem ri = EnumTools.GetSingle(e.Selection.GetSelection<ISvnRepositoryItem>());
                    if (ri != null && ri.Origin != null && ri.NodeKind != SvnNodeKind.Directory)
                        return;
                    break;
                case AnkhCommand.ItemAnnotate:
                    foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                    {
                        if (item.IsFile && item.IsVersioned && item.HasCopyableHistory)
                            return;
                    }
                    break;
                case AnkhCommand.DocumentAnnotate:
                    if (e.Selection.ActiveDocumentItem != null && e.Selection.ActiveDocumentItem.HasCopyableHistory)
                        return;
                    break;
                case AnkhCommand.LogAnnotateRevision:
                    ILogControl logControl = e.Selection.GetActiveControl<ILogControl>();
                    if (logControl == null || logControl.Origins == null)
                    {
                        e.Visible = e.Enabled = false;
                        return;
                    }

                    if (!EnumTools.IsEmpty(e.Selection.GetSelection<ISvnLogChangedPathItem>()))
                        return;
                    break;
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            List<SvnOrigin> targets = new List<SvnOrigin>();
            SvnRevision startRev = SvnRevision.Zero;
            SvnRevision endRev = null;
            switch (e.Command)
            {
                case AnkhCommand.ItemAnnotate:
                    endRev = SvnRevision.Working;
                    foreach (SvnItem i in e.Selection.GetSelectedSvnItems(false))
                    {
                        if (i.IsFile && i.IsVersioned && i.HasCopyableHistory)
                            targets.Add(new SvnOrigin(i));
                    }
                    break;
                case AnkhCommand.LogAnnotateRevision:
                    foreach (ISvnLogChangedPathItem logItem in e.Selection.GetSelection<ISvnLogChangedPathItem>())
                    {
                        targets.Add(logItem.Origin);
                        endRev = logItem.Revision;
                    }
                    break;
                case AnkhCommand.SvnNodeAnnotate:
                    foreach (ISvnRepositoryItem item in e.Selection.GetSelection<ISvnRepositoryItem>())
                    {
                        targets.Add(item.Origin);
                        endRev = item.Revision;
                    }
                    break;
                case AnkhCommand.DocumentAnnotate:
                    //TryObtainBlock(e);
                    targets.Add(new SvnOrigin(e.GetService<IFileStatusCache>()[e.Selection.ActiveDocumentFilename]));
                    endRev = SvnRevision.Working;
                    break;
            }

            if (targets.Count == 0)
                return;

            bool ignoreEols = true;
            SvnIgnoreSpacing ignoreSpacing = SvnIgnoreSpacing.IgnoreSpace;
            bool retrieveMergeInfo = false;
            SvnOrigin target;

            if ((!e.DontPrompt && !Shift) || e.PromptUser)
                using (AnnotateDialog dlg = new AnnotateDialog())
                {
                    dlg.SetTargets(targets);
                    dlg.StartRevision = startRev;
                    dlg.EndRevision = endRev;

                    if (dlg.ShowDialog(e.Context) != DialogResult.OK)
                        return;

                    target = dlg.SelectedTarget;
                    startRev = dlg.StartRevision;
                    endRev = dlg.EndRevision;
                    ignoreEols = dlg.IgnoreEols;
                    ignoreSpacing = dlg.IgnoreSpacing;
                    retrieveMergeInfo = dlg.RetrieveMergeInfo;
                }
            else
            {
                SvnItem one = EnumTools.GetFirst(e.Selection.GetSelectedSvnItems(false));

                if (one == null)
                    return;

                target = new SvnOrigin(one);
            }

            if (startRev == SvnRevision.Working || endRev == SvnRevision.Working && target.Target is SvnPathTarget)
            {
                IAnkhOpenDocumentTracker tracker = e.GetService<IAnkhOpenDocumentTracker>();
                if (tracker != null)
                    tracker.SaveDocument(((SvnPathTarget)target.Target).FullPath);
            }

            DoBlame(e, target, startRev, endRev, ignoreEols, ignoreSpacing, retrieveMergeInfo);
        }

        /*private void TryObtainBlock(CommandEventArgs e)
        {
            ISelectionContextEx ex = e.GetService<ISelectionContextEx>(typeof(ISelectionContext));

            if (ex == null)
                return;

            IVsTextView view = ex.ActiveDocumentFrameTextView;
            IVsTextLines lines;
            Guid languageService;
            IVsLanguageInfo info;

            if (view != null
                && VSErr.Succeeded(view.GetBuffer(out lines))
                && VSErr.Succeeded(lines.GetLanguageServiceID(out languageService))
                && null != (info = e.QueryService<IVsLanguageInfo>(languageService)))
            {
                GC.KeepAlive(info);
                IVsLanguageBlock b = info as IVsLanguageBlock;
                if (b != null)
                {
                    GC.KeepAlive(b);
                }
            }
            //IVsLanguageBlock


            GC.KeepAlive(ex);
        }*/

        static void DoBlame(CommandEventArgs e, SvnOrigin item, SvnRevision revisionStart, SvnRevision revisionEnd, bool ignoreEols, SvnIgnoreSpacing ignoreSpacing, bool retrieveMergeInfo)
        {
            SvnWriteArgs wa = new SvnWriteArgs();
            wa.Revision = revisionEnd;

            SvnBlameArgs ba = new SvnBlameArgs();
            ba.Start = revisionStart;
            ba.End = revisionEnd;
            ba.IgnoreLineEndings = ignoreEols;
            ba.IgnoreSpacing = ignoreSpacing;
            ba.RetrieveMergedRevisions = retrieveMergeInfo;

            SvnTarget target = item.Target;

            IAnkhTempFileManager tempMgr = e.GetService<IAnkhTempFileManager>();
            string tempFile = tempMgr.GetTempFileNamed(target.FileName);

            Collection<SvnBlameEventArgs> blameResult = null;

            bool retry = false;
            ProgressRunnerResult r = e.GetService<IProgressRunner>().RunModal(CommandStrings.Annotating, delegate(object sender, ProgressWorkerArgs ee)
            {
                using (FileStream fs = File.Create(tempFile))
                {
                    ee.Client.Write(target, fs, wa);
                }

                ba.SvnError +=
                    delegate(object errorSender, SvnErrorEventArgs errorEventArgs)
                    {
                        if (errorEventArgs.Exception is SvnClientBinaryFileException)
                        {
                            retry = true;
                            errorEventArgs.Cancel = true;
                        }
                    };
                ee.Client.GetBlame(target, ba, out blameResult);
            });

            if (retry)
            {
                using (AnkhMessageBox mb = new AnkhMessageBox(e.Context))
                {
                    if (DialogResult.Yes != mb.Show(
                                                CommandStrings.AnnotateBinaryFileContinueAnywayText,
                                                CommandStrings.AnnotateBinaryFileContinueAnywayTitle,
                                                MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                        return;

                    r = e.GetService<IProgressRunner>()
                            .RunModal(CommandStrings.Annotating,
                                      delegate(object sender, ProgressWorkerArgs ee)
                                      {
                                          ba.IgnoreMimeType = true;
                                          ee.Client.GetBlame(target, ba, out blameResult);
                                      });
                }
            }

            if (!r.Succeeded)
                return;

            AnnotateEditorControl annEditor = new AnnotateEditorControl();
            IAnkhEditorResolver er = e.GetService<IAnkhEditorResolver>();

            annEditor.Create(e.Context, tempFile);
            annEditor.LoadFile(tempFile);
            annEditor.AddLines(item, blameResult);

            // Detect and set the language service
            Guid language;
            if (er.TryGetLanguageService(Path.GetExtension(target.FileName), out language))
            {
                // Extension is mapped -> user
                annEditor.SetLanguageService(language);
            }
            else if (blameResult != null && blameResult.Count > 0 && blameResult[0].Line != null)
            {
                // Extension is not mapped -> Check if this is xml (like project files)
                string line = blameResult[0].Line.Trim();

                if (line.StartsWith("<?xml")
                    || (line.StartsWith("<") && line.Contains("xmlns=\"http://schemas.microsoft.com/developer/msbuild/")))
                {
                    if (er.TryGetLanguageService(".xml", out language))
                    {
                        annEditor.SetLanguageService(language);
                    }
                }
            }
        }
    }
}
