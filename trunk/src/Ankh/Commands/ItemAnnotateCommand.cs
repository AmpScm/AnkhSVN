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
    [Command(AnkhCommand.ItemAnnotate)]
    [Command(AnkhCommand.LogAnnotateRevision)]
    [Command(AnkhCommand.SvnNodeAnnotate)]
    [Command(AnkhCommand.DocumentAnnotate)]
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
                        if (item.IsVersioned && item.IsFile)
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
            switch (e.Command)
            {
                case AnkhCommand.ItemAnnotate:
                    BlameItem(e);
                    break;
                case AnkhCommand.LogAnnotateRevision:
                    BlameRevision(e);
                    break;
                case AnkhCommand.SvnNodeAnnotate:
                    SvnNodeBlame(e);
                    break;
                case AnkhCommand.DocumentAnnotate:
                    BlameDocument(e);
                    break;
            }
        }

        static void SvnNodeBlame(CommandEventArgs e)
        {
            ISvnRepositoryItem blameSection = EnumTools.GetFirst(e.Selection.GetSelection<ISvnRepositoryItem>());

            SvnRevision revisionStart = SvnRevision.Zero;
            SvnRevision revisionEnd = blameSection.Revision;

            // TODO: Confirm revisions?

            DoBlame(e, blameSection.Origin, revisionStart, revisionEnd, false, SvnIgnoreSpacing.None, false);
        }

        static void BlameRevision(CommandEventArgs e)
        {
            ISvnLogChangedPathItem item = null;
            foreach (ISvnLogChangedPathItem logItem in e.Selection.GetSelection<ISvnLogChangedPathItem>())
            {
                item = logItem;
                break;
            }

            SvnRevision revisionStart = SvnRevision.Zero;
            SvnRevision revisionEnd = item.Revision;

            DoBlame(e, item.Origin, revisionStart, revisionEnd, false, SvnIgnoreSpacing.None, false);
        }

        static void BlameItem(CommandEventArgs e)
        {
            SvnOrigin target;
            SvnRevision startRev = SvnRevision.One;
            SvnRevision endRev = SvnRevision.Base;
			bool ignoreEols = true;
			SvnIgnoreSpacing ignoreSpacing = SvnIgnoreSpacing.IgnoreSpace;
			bool retrieveMergeInfo = false;

            if ((!e.DontPrompt && !Shift) || e.PromptUser)
                using (AnnotateDialog dlg = new AnnotateDialog())
                {
                    dlg.SetTargets(e.Selection.GetSelectedSvnItems(false));
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

            DoBlame(e, target, startRev, endRev, ignoreEols, ignoreSpacing, retrieveMergeInfo);
        }

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
            Dictionary<long, string> logMessages = new Dictionary<long, string>();

            ba.Notify += delegate(object sender, SvnNotifyEventArgs ee)
            {
                if (ee.Action == SvnNotifyAction.BlameRevision && ee.RevisionProperties != null)
                {
                    if (ee.RevisionProperties.Contains(SvnPropertyNames.SvnLog))
                        logMessages[ee.Revision] = ee.RevisionProperties[SvnPropertyNames.SvnLog].StringValue;
                }
            };

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
                    if (DialogResult.Yes == mb.Show(
                                                CommandStrings.AnnotateBinaryFileContinueAnywayText,
                                                CommandStrings.AnnotateBinaryFileContinueAnywayTitle,
                                                MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                    {
                        r = e.GetService<IProgressRunner>()
                            .RunModal(CommandStrings.Annotating,
                                      delegate(object sender, ProgressWorkerArgs ee)
                                      {
                                          ba.IgnoreMimeType = true;
                                          ee.Client.GetBlame(target, ba, out blameResult);
                                      });
                    }
                }
            }

            if (!r.Succeeded)
                return;

            AnnotateEditorControl annEditor = new AnnotateEditorControl();
            IAnkhEditorResolver er = e.GetService<IAnkhEditorResolver>();

            annEditor.Create(e.Context, tempFile);
            annEditor.LoadFile(tempFile);
            annEditor.AddLines(item, blameResult, logMessages);

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

        static void BlameDocument(CommandEventArgs e)
        {
			SvnOrigin target;
			SvnRevision startRev = SvnRevision.One;
			SvnRevision endRev = SvnRevision.Base;
			bool ignoreEols = true;
			SvnIgnoreSpacing ignoreSpacing = SvnIgnoreSpacing.IgnoreSpace;
			bool retrieveMergeInfo = false;

			if ((!e.DontPrompt && !Shift) || e.PromptUser)
				using (AnnotateDialog dlg = new AnnotateDialog())
				{
					dlg.SetTargets(new SvnItem[] {e.Selection.ActiveDocumentItem });
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

			DoBlame(e, target, startRev, endRev, ignoreEols, ignoreSpacing, retrieveMergeInfo);
        }
    }
}
