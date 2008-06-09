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

namespace Ankh.Commands
{
    /// <summary>
    /// Command to identify which users to blame for which lines.
    /// </summary>
    [Command(AnkhCommand.Blame)]
    public class BlameCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (!e.State.SccProviderActive)
            {
                e.Visible = e.Enabled = false;
                return;
            }
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (item.IsVersioned && item.IsFile)
                    return;
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IUIShell uiShell = e.GetService<IUIShell>();

            SvnRevision revisionStart = SvnRevision.Zero;
            SvnRevision revisionEnd = SvnRevision.Head;

            SvnItem firstItem = null;
            PathSelectorResult result = null;
            PathSelectorInfo info = new PathSelectorInfo("Blame",
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

                revisionStart = info.RevisionStart;
                revisionEnd = info.RevisionEnd;
            }
            else
            {
                result = info.DefaultResult;
            }

            if (!result.Succeeded)
                return;

            XslCompiledTransform transform = CommandBase.GetTransform(e.Context, BlameTransform);

            foreach (SvnItem item in result.Selection)
            {
                // do the blame thing
                BlameResult blameResult = new BlameResult();

                blameResult.Start();
                BlameRunner runner = new BlameRunner(item.FullPath,
                    revisionStart, revisionEnd, blameResult);

                e.GetService<IProgressRunner>().Run("Annotating", runner.Work);
                blameResult.End();

                // transform it to HTML
                StringWriter writer = new StringWriter();
                blameResult.Transform(transform, writer);

                // display the HTML with the filename as caption
                string filename = Path.GetFileName(item.FullPath);
                uiShell.DisplayHtml(filename, writer.ToString(), false);
            }
        }

        #endregion

        private class BlameRunner
        {
            public BlameRunner(string path, SvnRevision start, SvnRevision end,
                BlameResult result)
            {
                this.path = path;
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

                e.Client.Blame(this.path, args, new EventHandler<SvnBlameEventArgs>(this.result.Receive));
            }

            private string path;
            private SvnRevision start;
            private SvnRevision end;
            private BlameResult result;

            public void Work(object sender, ProgressWorkerArgs e)
            {
                Work(e);
            }
        }



        private const string BlameTransform = "blame.xsl";


    }
}
