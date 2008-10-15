using System;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;


using System.IO;
using System.Collections;
using System.Diagnostics;
using SharpSvn;
using Ankh.Ids;
using System.Collections.Generic;
using Ankh.UI;
using Ankh.Scc;
using Ankh.UI.Blame;
using Ankh.Selection;
using Ankh.VS;
using System.Collections.ObjectModel;
using Ankh.Scc.UI;
using Ankh.UI.SvnLog.Commands;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to identify which users to blame for which lines.
    /// </summary>
    [Command(AnkhCommand.ItemAnnotate)]
    [Command(AnkhCommand.LogAnnotateRevision)]
    [Command(AnkhCommand.BlameShowBlame)]
    class ItemAnnotateCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            switch (e.Command)
            {
                case AnkhCommand.BlameShowBlame:
                    IBlameControl blameControl = e.Selection.ActiveDialogOrFrameControl as IBlameControl;
                    if (blameControl == null || !blameControl.HasWorkingCopyItems)
                    {
                        e.Enabled = e.Visible = false;
                        return;
                    }

                    int blameCount = 0;
                    foreach (IBlameSection blameItem in e.Selection.GetSelection<IBlameSection>())
                    {
                        blameCount++;
                    }
                    if (blameCount == 1)
                        return;
                    break;
                case AnkhCommand.ItemAnnotate:
                    foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                    {
                        if (item.IsVersioned && item.IsFile)
                            return;
                    }
                    break;
                case AnkhCommand.LogAnnotateRevision:
                    ILogControl logControl = e.Selection.ActiveDialogOrFrameControl as ILogControl;
                    if (logControl == null || !logControl.HasWorkingCopyItems)
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
                case AnkhCommand.BlameShowBlame:
                    BlameBlame(e);
                    break;
            }
        }

        void BlameBlame(CommandEventArgs e)
        {
            IBlameControl blameControl = e.Selection.ActiveDialogOrFrameControl as IBlameControl;

            IBlameSection blameSection = null;
            foreach (IBlameSection blame in e.Selection.GetSelection<IBlameSection>())
            {
                blameSection = blame;
                break;
            }

            SvnItem item = null;
            foreach(SvnItem i in blameControl.WorkingCopyItems)
            {
                item = i;
            }


            SvnRevision revisionStart = SvnRevision.Zero;
            SvnRevision revisionEnd = blameSection.Revision;

            DoBlame(e, item, revisionStart, revisionEnd); 
        }

        void BlameRevision(CommandEventArgs e)
        {
            ILogControl logControl = e.Selection.ActiveDialogOrFrameControl as ILogControl;
            IUIShell uiShell = e.GetService<IUIShell>();

            HybridCollection<string> changedPaths = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);

            ISvnLogChangedPathItem item = null;
            foreach (ISvnLogChangedPathItem logItem in e.Selection.GetSelection<ISvnLogChangedPathItem>())
            {
                if(!changedPaths.Contains(logItem.Path))
                    changedPaths.Add(logItem.Path);
                item = logItem;
                break;
            }

            SvnItem svnItem = null;
            IEnumerable<SvnItem> intersectedItems = LogHelper.IntersectWorkingCopyItemsWithChangedPaths(logControl.WorkingCopyItems, changedPaths);
            foreach (SvnItem i in intersectedItems)
            {
                svnItem = i;
                break;
            }
            if (svnItem == null)
                return;

            SvnRevision revisionStart = SvnRevision.Zero;
            SvnRevision revisionEnd = item.Revision;

            DoBlame(e, svnItem, revisionStart, revisionEnd);
        }

        void BlameItem(CommandEventArgs e)
        {
            IUIShell uiShell = e.GetService<IUIShell>();

            SvnRevision revisionStart = SvnRevision.Zero;
            SvnRevision revisionEnd = SvnRevision.Base;

            SvnItem firstItem = null;
            PathSelectorResult result = null;
            PathSelectorInfo info = new PathSelectorInfo("Annotate",
                e.Selection.GetSelectedSvnItems(true));

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

            DoBlame(e, blameItem, result.RevisionStart, result.RevisionEnd);
        }

        void DoBlame(CommandEventArgs e, SvnItem item, SvnRevision revisionStart, SvnRevision revisionEnd)
        {
            IAnkhPackage p = e.GetService<IAnkhPackage>();
            SvnExportArgs ea = new SvnExportArgs();
            ea.Revision = revisionEnd;

            SvnBlameArgs ba = new SvnBlameArgs();
            ba.Start = revisionStart;
            ba.End = revisionEnd;

            SvnTarget target = new SvnPathTarget(item.FullPath);

            IAnkhTempFileManager tempMgr = e.GetService<IAnkhTempFileManager>();
            string tempFile = tempMgr.GetTempFile();

            Collection<SvnBlameEventArgs> blameResult = null;
            e.GetService<IProgressRunner>().Run("Annotating", delegate(object sender, ProgressWorkerArgs ee)
            {

                ee.Client.Export(target, tempFile, ea);

                ee.Client.GetBlame(target, ba, out blameResult);
            });

            p.ShowToolWindow(AnkhToolWindow.Blame);
            BlameToolWindowControl blameToolControl = e.GetService<ISelectionContext>().ActiveFrameControl as BlameToolWindowControl;
            blameToolControl.Init();

            blameToolControl.LoadFile(item.FullPath, tempFile);
            blameToolControl.AddLines(item, blameResult);
        }
    }
}
