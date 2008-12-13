// $Id$
//
// Copyright 2005-2008 The AnkhSVN Project
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
                    if (ri != null && ri.Origin != null)
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

                    int count = 0;
                    foreach (ISvnLogChangedPathItem logItem in e.Selection.GetSelection<ISvnLogChangedPathItem>())
                    {
                        count++;
                        if (count > 1)
                            break;
                    }
                    if (count == 1)
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

        void SvnNodeBlame(CommandEventArgs e)
        {
            ISvnRepositoryItem blameSection = EnumTools.GetFirst(e.Selection.GetSelection<ISvnRepositoryItem>());

            SvnRevision revisionStart = SvnRevision.Zero;
            SvnRevision revisionEnd = blameSection.Revision;

            // TODO: Confirm revisions?

            DoBlame(e, blameSection.Origin, revisionStart, revisionEnd);
        }

        void BlameRevision(CommandEventArgs e)
        {
            ILogControl logControl = e.Selection.ActiveDialogOrFrameControl as ILogControl;
            IUIShell uiShell = e.GetService<IUIShell>();

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

        void BlameItem(CommandEventArgs e)
        {
            IUIShell uiShell = e.GetService<IUIShell>();

            SvnRevision revisionStart = SvnRevision.Zero;
            SvnRevision revisionEnd = SvnRevision.Base;

            SvnItem firstItem = null;
            PathSelectorResult result = null;
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
            if (!CommandBase.Shift)
            {

                info.RevisionStart = revisionStart;
                info.RevisionEnd = revisionEnd;
                info.EnableRecursive = false;
                info.Depth = SvnDepth.Empty;
                info.SingleSelection = true;

                // show the selector dialog
                result = uiShell.ShowPathSelector(info);
                if (info == null)
                    return;

                revisionStart = result.RevisionStart;
                revisionEnd = result.RevisionEnd;
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

        void DoBlame(CommandEventArgs e, SvnOrigin item, SvnRevision revisionStart, SvnRevision revisionEnd)
        {
            IAnkhPackage p = e.GetService<IAnkhPackage>();
            SvnExportArgs ea = new SvnExportArgs();
            ea.Revision = revisionEnd;

            SvnBlameArgs ba = new SvnBlameArgs();
            ba.Start = revisionStart;
            ba.End = revisionEnd;

            SvnTarget target = item.Target;

            IAnkhTempFileManager tempMgr = e.GetService<IAnkhTempFileManager>();
            string tempFile = tempMgr.GetTempFileNamed(target.FileName);

            Collection<SvnBlameEventArgs> blameResult = null;
            e.GetService<IProgressRunner>().RunModal("Annotating", delegate(object sender, ProgressWorkerArgs ee)
            {
                ee.Client.Export(target, tempFile, ea);

                ee.Client.GetBlame(target, ba, out blameResult);
            });

            AnnotateEditorControl btw = new AnnotateEditorControl();           

            string path = null;
            SvnPathTarget pt = target as SvnPathTarget;

            if (pt != null)
            {
                path = pt.FullPath;

                IProjectFileMapper pfm = e.GetService<IProjectFileMapper>();
                IAnkhOpenDocumentTracker odt = e.GetService<IAnkhOpenDocumentTracker>();

                if (pfm != null && pfm.IsProjectFileOrSolution(pt.FullPath))
                {
                    // Don't use real names for projects or the solution
                    // We don't want to crash VS here
                    path = null;
                }
                else if (odt != null && odt.IsDocumentOpen(pt.FullPath))
                {
                    // We would replace the existing buffer here
                    
                    // odt.IsDocumentOpenInTextEditor() should be safe for us but testing revieled
                    // External changes would be reflected in our buffer; not the original :(

                    path = null;
                }
            }

            if (string.IsNullOrEmpty(path))
                path = tempFile;

            btw.Create(e.Context, path);
            btw.LoadFile(path, tempFile);
            btw.AddLines(item, blameResult);
        }

        void BlameDocument(CommandEventArgs e)
        {
            IUIShell uiShell = e.GetService<IUIShell>();

            SvnRevision revisionStart = SvnRevision.Zero;
            SvnRevision revisionEnd = SvnRevision.Base;

            SvnItem firstItem = null;
            PathSelectorResult result = null;
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
            if (!CommandBase.Shift)
            {

                info.RevisionStart = revisionStart;
                info.RevisionEnd = revisionEnd;
                info.EnableRecursive = false;
                info.Depth = SvnDepth.Empty;
                info.SingleSelection = true;

                // show the selector dialog
                result = uiShell.ShowPathSelector(info);
                if (info == null)
                    return;

                revisionStart = result.RevisionStart;
                revisionEnd = result.RevisionEnd;
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
