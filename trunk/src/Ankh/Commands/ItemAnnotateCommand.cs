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
using Ankh.Ids;
using Ankh.Scc;
using Ankh.Scc.UI;
using Ankh.UI;
using Ankh.UI.Annotate;
using Ankh.VS;
using SharpSvn;
using System.Collections.Generic;

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
                    ILogControl logControl = e.Selection.ActiveDialogOrFrameControl as ILogControl;
                    if (logControl == null || logControl.Origins == null)
                    {
                        e.Visible = e.Enabled = false;
                        return;
                    }

                    if(!EnumTools.IsEmpty(e.Selection.GetSelection<ISvnLogChangedPathItem>()))
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

            DoBlame(e, blameSection.Origin, revisionStart, revisionEnd);
        }

        static void BlameRevision(CommandEventArgs e)
        {
            HybridCollection<string> changedPaths = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);

            ISvnLogChangedPathItem item = null;
            foreach (ISvnLogChangedPathItem logItem in e.Selection.GetSelection<ISvnLogChangedPathItem>())
            {
                if (!changedPaths.Contains(logItem.Path))
                    changedPaths.Add(logItem.Path);
                item = logItem;
                break;
            }

            SvnRevision revisionStart = SvnRevision.Zero;
            SvnRevision revisionEnd = item.Revision;

            DoBlame(e, item.Origin, revisionStart, revisionEnd);
        }

        static void BlameItem(CommandEventArgs e)
        {
            IUIShell uiShell = e.GetService<IUIShell>();

            SvnRevision revisionStart = SvnRevision.Zero;
            SvnRevision revisionEnd = SvnRevision.Base;

            SvnItem firstItem = null;
            PathSelectorResult result;
            PathSelectorInfo info = new PathSelectorInfo("Annotate",
                e.Selection.GetSelectedSvnItems(false));

            info.CheckedFilter += delegate(SvnItem item)
            {
                if (firstItem == null && item.IsFile)
                    firstItem = item;

                return (item == firstItem);
            };
            info.VisibleFilter += delegate(SvnItem item) { return item.IsVersioned && item.IsFile; };

            // is shift depressed?
            if (!Shift)
            {

                info.RevisionStart = revisionStart;
                info.RevisionEnd = revisionEnd;
                info.EnableRecursive = false;
                info.Depth = SvnDepth.Empty;
                info.SingleSelection = true;

                // show the selector dialog
                result = uiShell.ShowPathSelector(info);
            }
            else
            {
                result = info.DefaultResult;
            }

            if (!result.Succeeded)
                return;

            SvnItem blameItem = null;
            foreach (SvnItem i in result.Selection)
                blameItem = i;

            DoBlame(e, new SvnOrigin(blameItem), result.RevisionStart, result.RevisionEnd);
        }

        static void DoBlame(CommandEventArgs e, SvnOrigin item, SvnRevision revisionStart, SvnRevision revisionEnd)
        {
            SvnWriteArgs wa = new SvnWriteArgs();
            wa.Revision = revisionEnd;

            SvnBlameArgs ba = new SvnBlameArgs();
            ba.Start = revisionStart;
            ba.End = revisionEnd;

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

            ProgressRunnerResult r = e.GetService<IProgressRunner>().RunModal("Annotating", delegate(object sender, ProgressWorkerArgs ee)
            {
                using (FileStream fs = File.Create(tempFile))
                {
                    ee.Client.Write(target, fs, wa);
                }

                ee.Client.GetBlame(target, ba, out blameResult);
            });

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
                    || (line.StartsWith("<") && line.Contains("xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\"")))
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
            IUIShell uiShell = e.GetService<IUIShell>();

            SvnRevision revisionStart = SvnRevision.Zero;
            SvnRevision revisionEnd = SvnRevision.Base;

            SvnItem firstItem = null;
            PathSelectorResult result;
            PathSelectorInfo info = new PathSelectorInfo("Annotate",
                new SvnItem[] { e.Selection.ActiveDocumentItem });

            info.CheckedFilter += delegate(SvnItem item)
            {
                if (firstItem == null && item.IsFile)
                    firstItem = item;

                return (item == firstItem);
            };
            info.VisibleFilter += delegate(SvnItem item) { return item.IsVersioned && item.IsFile; };

            // is shift depressed?
            if (!Shift)
            {

                info.RevisionStart = revisionStart;
                info.RevisionEnd = revisionEnd;
                info.EnableRecursive = false;
                info.Depth = SvnDepth.Empty;
                info.SingleSelection = true;

                // show the selector dialog
                result = uiShell.ShowPathSelector(info);
            }
            else
            {
                result = info.DefaultResult;
            }

            if (!result.Succeeded)
                return;

            SvnItem blameItem = null;
            foreach (SvnItem i in result.Selection)
                blameItem = i;

            DoBlame(e, new SvnOrigin(blameItem), result.RevisionStart, result.RevisionEnd);
        }
    }
}
