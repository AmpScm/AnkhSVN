using System;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using NSvn.Core;
using System.IO;
using System.Collections;
using System.Diagnostics;

namespace Ankh.Commands
{
	/// <summary>
	/// A command class to support the svn blame command.
	/// </summary>
    [VSNetCommand("Blame", Text = "Blame...", Tooltip = "Runs Blame on the selected item",
         Bitmap = ResourceBitmaps.Default),
    VSNetControl( "Item.Ankh", Position = 1 ),
    VSNetProjectNodeControl( "Ankh", Position = 1 ),
    VSNetControl( "Solution.Ankh", Position = 1)]    
	public class BlameCommand : CommandBase
	{
        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            if ( context.SolutionExplorer.GetSelectionResources( true, 
                new ResourceFilterCallback( CommandBase.VersionedSingleFileFilter) ).Count == 1 )
                return Enabled;
            else
                return Disabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            IList resources = context.SolutionExplorer.GetSelectionResources( true, 
                new ResourceFilterCallback( CommandBase.VersionedFilter) );

            Revision revisionStart = Revision.FromNumber(0);
            Revision revisionEnd = Revision.Head;

            // is shift depressed?
            if ( !CommandBase.Shift )
            {
                PathSelectorInfo info = new PathSelectorInfo( "Blame", resources, resources );
                info.RevisionStart = revisionStart;
                info.RevisionEnd = revisionEnd;
                info.EnableRecursive = false;
                info.Recursive = false;

                // show the selector dialog
                info = context.UIShell.ShowPathSelector( info );
                if ( info == null )
                    return;

                revisionStart = info.RevisionStart;
                revisionEnd = info.RevisionEnd;
                resources = info.CheckedItems;
            }

            XslTransform transform = CommandBase.GetTransform( 
                context, BlameTransform );

            foreach( SvnItem item in resources )
            {
                // do the blame thing
                BlameResult result = new BlameResult();

                result.Start();
                BlameRunner runner = new BlameRunner( item.Path, 
                    Revision.FromNumber(0), Revision.Head, result );
                context.UIShell.RunWithProgressDialog( runner, "Figuring out who to blame" );
                result.End();
               
                // transform it to HTML
                StringWriter writer = new StringWriter();
                result.Transform( transform, writer );

                // display the HTML with the filename as caption
                string filename = Path.GetFileName( item.Path );
                context.UIShell.DisplayHtml( filename, writer.ToString(), false );
            }
        }

        

        private class BlameRunner : IProgressWorker
        {
            public BlameRunner( string path, Revision start, Revision end, 
                BlameResult result )
            {
                this.path = path; 
                this.start = start;
                this.end = end;
                this.result = result;
            }

            public void Work(IContext context)
            {
                context.Client.Blame( this.path, this.start, this.end,
                    new BlameReceiver( this.result.Receive ) );
            }

            private string path;
            private Revision start;
            private Revision end;
            private BlameResult result;
        }

        

        private const string BlameTransform = "blame.xsl";


	}
}
