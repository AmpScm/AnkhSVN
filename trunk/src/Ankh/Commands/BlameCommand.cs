using System;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;


using System.IO;
using System.Collections;
using System.Diagnostics;
using SharpSvn;
using AnkhSvn.Ids;

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
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems())
            {
                if (item.IsVersioned)
                    return;
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            IList resources = new ArrayList();/* context.Selection.GetSelectionResources(true,
                 new ResourceFilterCallback(SvnItem.VersionedFilter));*/

            if ( resources.Count == 0 )
            {
                return;
            }

            SvnRevision revisionStart = SvnRevision.Zero;
            SvnRevision revisionEnd = SvnRevision.Head;

            // is shift depressed?
            if ( !CommandBase.Shift )
            {
                PathSelectorInfo info = new PathSelectorInfo( "Blame", resources, new SvnItem[] {(SvnItem)resources[0]} );
                info.RevisionStart = revisionStart;
                info.RevisionEnd = revisionEnd;
                info.EnableRecursive = false;
                info.Depth = SvnDepth.Empty;
                info.SingleSelection = true;

                // show the selector dialog
                info = context.UIShell.ShowPathSelector( info );
                if ( info == null )
                    return;

                revisionStart = info.RevisionStart;
                revisionEnd = info.RevisionEnd;
                resources = info.CheckedItems;
            }
            else
            {
                resources = new SvnItem[] { (SvnItem)resources[0] };
            }

            XslCompiledTransform transform = CommandBase.GetTransform( 
                context, BlameTransform );

            foreach( SvnItem item in resources )
            {
                // do the blame thing
                BlameResult result = new BlameResult();

                result.Start();
                BlameRunner runner = new BlameRunner( item.FullPath, 
                    revisionStart, revisionEnd, result );
                context.UIShell.RunWithProgressDialog( runner, "Figuring out who to blame" );
                result.End();
               
                // transform it to HTML
                StringWriter writer = new StringWriter();
                result.Transform( transform, writer );

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

            public void Work(AnkhWorkerArgs e)
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
        }

        

        private const string BlameTransform = "blame.xsl";


    }
}
