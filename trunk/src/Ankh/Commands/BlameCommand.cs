using System;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;


using System.IO;
using System.Collections;
using System.Diagnostics;
using SharpSvn;
using AnkhSvn.Ids;
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
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (item.IsVersioned)
                    return;
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();


            SvnRevision revisionStart = SvnRevision.Zero;
            SvnRevision revisionEnd = SvnRevision.Head;


            bool first = true;
            PathSelectorResult result = null;
            PathSelectorInfo info = new PathSelectorInfo("Blame",
                e.Selection.GetSelectedSvnItems(true));

            info.CheckedFilter += delegate(SvnItem item) 
            {
                if (first)
                {
                    first = false;
                    return true;
                }
                return false;
            };
            info.VisibleFilter += delegate(SvnItem item) { return item.IsVersioned; };

            // is shift depressed?
            if ( !CommandBase.Shift )
            {
				
                info.RevisionStart = revisionStart;
                info.RevisionEnd = revisionEnd;
                info.EnableRecursive = false;
                info.Depth = SvnDepth.Empty;
                info.SingleSelection = true;

                // show the selector dialog
                result = context.UIShell.ShowPathSelector( info );
                if ( info == null )
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

            XslCompiledTransform transform = CommandBase.GetTransform( 
                context, BlameTransform );

            foreach( SvnItem item in result.Selection )
            {
                // do the blame thing
                BlameResult blameResult = new BlameResult();

                blameResult.Start();
                BlameRunner runner = new BlameRunner( item.FullPath,
                    revisionStart, revisionEnd, blameResult);

                e.GetService<IProgressRunner>().Run("Annotating", runner.Work);
                blameResult.End();
               
                // transform it to HTML
                StringWriter writer = new StringWriter();
                blameResult.Transform(transform, writer);

                // display the HTML with the filename as caption
                string filename = Path.GetFileName( item.FullPath );
                context.UIShell.DisplayHtml( filename, writer.ToString(), false );
            }
        }

        #endregion

        private class BlameRunner : IProgressWorker
        {
            public BlameRunner( string path, SvnRevision start, SvnRevision end, 
                BlameResult result )
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
