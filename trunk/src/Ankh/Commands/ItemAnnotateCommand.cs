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
    [Command(AnkhCommand.BlameShowBlame)]
    [Command(AnkhCommand.DocumentAnnotate)]
    class ItemAnnotateCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            switch (e.Command)
            {
                case AnkhCommand.BlameShowBlame:
                    if (null == EnumTools.GetSingle(e.Selection.GetSelection<IAnnotateSection>()))
                        e.Enabled = false;
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
                case AnkhCommand.BlameShowBlame:
                    BlameBlame(e);
                    break;
                case AnkhCommand.DocumentAnnotate:
                    BlameDocument(e);
                    break;
            }
        }

        void BlameBlame(CommandEventArgs e)
        {
            IAnnotateSection blameSection = EnumTools.GetFirst(e.Selection.GetSelection<IAnnotateSection>());

            SvnRevision revisionStart = SvnRevision.Zero; // Copy from original blame?
            SvnRevision revisionEnd = blameSection.Revision;

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
            string tempFile = tempMgr.GetTempFile();

            Collection<SvnBlameEventArgs> blameResult = null;
            e.GetService<IProgressRunner>().RunModal("Annotating", delegate(object sender, ProgressWorkerArgs ee)
            {
                ee.Client.Export(target, tempFile, ea);

                ee.Client.GetBlame(target, ba, out blameResult);
            });

            //AnnotateViewForm vf = new AnnotateViewForm();

            //vf.Create(e.Context, "c:\\test.txt");
            //vf.Context = e.Context;

            AnnotateEditorControl btw = new AnnotateEditorControl();           

            string path;

            SvnPathTarget pt = target as SvnPathTarget;

            if (pt != null)
            {
                path = pt.FullPath;
            }
            else
            {
                path = tempMgr.GetTempFileNamed(target.FileName);
            }

            btw.Create(e.Context, path);
            btw.Init();

            //p.ShowToolWindow(AnkhToolWindow.Blame);
            //BlameToolWindowControl blameToolControl = e.GetService<ISelectionContext>().ActiveFrameControl as BlameToolWindowControl;
            //blameToolControl.Init();

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
