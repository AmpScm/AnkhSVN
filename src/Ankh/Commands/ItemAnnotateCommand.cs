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

namespace Ankh.Commands
{
    /// <summary>
    /// Command to identify which users to blame for which lines.
    /// </summary>
    [Command(AnkhCommand.ItemAnnotate)]
    [Command(AnkhCommand.LogAnnotateRevision)]
    class ItemAnnotateCommand : CommandBase
    {
        XslCompiledTransform _transform;
        private const string BlameTransform = "blame.xsl";


        

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            switch (e.Command)
            {
                case AnkhCommand.ItemAnnotate:
                    foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                    {
                        if (item.IsVersioned && item.IsFile)
                            return;
                    }
                    break;
                case AnkhCommand.LogAnnotateRevision:
                    // Disabled for now, see TODO belows
                    e.Visible = e.Enabled = false;
                    return;

                    /*int count = 0;
                    foreach (ISvnLogChangedPathItem logItem in e.Selection.GetSelection<ISvnLogChangedPathItem>())
                    {
                        count++;
                        if (count > 1)
                            break;
                    }
                    if (count == 1)
                        return;
                    break;*/
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
            }
        }

        void BlameRevision(CommandEventArgs e)
        {
            IUIShell uiShell = e.GetService<IUIShell>();
            ISvnLogChangedPathItem item = null;
            foreach (ISvnLogChangedPathItem logItem in e.Selection.GetSelection<ISvnLogChangedPathItem>())
            {
                item = logItem;
                break;
            }
            if (item == null)
                return;

            SvnRevision revisionStart = SvnRevision.Zero;
            SvnRevision revisionEnd = item.Revision;

            BlameResult blameResult = new BlameResult();

            blameResult.Start();
            // TODO: we need the real filesystem path and/or url here
            BlameRunner runner = new BlameRunner(item.Path,
                revisionStart, revisionEnd, blameResult);
            
            e.GetService<IProgressRunner>().Run("Annotating", runner.Work);
            blameResult.End();

            // transform it to HTML
            StringWriter writer = new StringWriter();
            blameResult.Transform(GetTransform(e.Context), writer);

            // display the HTML with the filename as caption
            uiShell.DisplayHtml(string.Format("Revision {0}", item.Revision), writer.ToString(), false);
        }

        void BlameItem(CommandEventArgs e)
        {
            IAnkhPackage p = e.GetService<IAnkhPackage>();


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
            
            foreach (SvnItem item in result.Selection)
            {
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

                blameToolControl.LoadFile(firstItem.FullPath, tempFile);
                blameToolControl.AddLines(firstItem, blameResult);

                // TODO open multiple blame editors ?
                break;
            }
        }

        private class BlameRunner
        {
            public BlameRunner(SvnTarget target, SvnRevision start, SvnRevision end,
                BlameResult result)
            {
                this.target = target;
                this.start = start;
                this.end = end;
                this.result = result;
            }

            public void Work(ProgressWorkerArgs e)
            {
                SvnBlameArgs args = new SvnBlameArgs();
                args.Start = start;
                args.End = end;
                //args.IgnoreLineEndings
                //args.IgnoreMimeType
                //args.IgnoreSpacing
                //args.IncludeMergedRevisions

                e.Client.Blame(this.target, args, new EventHandler<SvnBlameEventArgs>(this.result.Receive));
            }

            private SvnTarget target;
            private SvnRevision start;
            private SvnRevision end;
            private BlameResult result;

            public void Work(object sender, ProgressWorkerArgs e)
            {
                Work(e);
            }
        }

        XslCompiledTransform GetTransform(IAnkhServiceProvider context)
        {
            return _transform ?? (_transform = CommandBase.GetTransform(context, BlameTransform));
        }
    }
}
