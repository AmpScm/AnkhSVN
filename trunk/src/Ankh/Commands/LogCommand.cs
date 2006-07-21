// $Id$
using System;
using NSvn.Core;
using System.Diagnostics;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Collections;

namespace Ankh.Commands
{
    [VSNetCommand("Log", Text = "Log...", Tooltip = "Runs Log on the selected item",
         Bitmap = ResourceBitmaps.Default),
    VSNetItemControl( "Ankh", Position = 1 )]  
	public class LogCommand : CommandBase
	{
        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            if ( context.SolutionExplorer.GetSelectionResources( false, 
                new ResourceFilterCallback( SvnItem.VersionedFilter ) ).Count > 0 )
                return Enabled;
            else
                return Disabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            IList resources = context.SolutionExplorer.GetSelectionResources(
                false, new ResourceFilterCallback( SvnItem.VersionedFilter ) );

            this.info = new LogDialogInfo( resources, resources );
            this.info.RevisionStart = Revision.FromNumber( 0 );
            this.info.RevisionEnd = Revision.Head;

            // is Shift down?
            if ( !CommandBase.Shift )
            {
                this.info = context.UIShell.ShowLogDialog( info );
                if ( this.info == null )
                    return;
            }

            this.result = null;

            context.UIShell.RunWithProgressDialog( new SimpleProgressWorker( 
                new SimpleProgressWorkerCallback( this.ProgressCallback ) ), "Retrieving log" );

            Debug.Assert( this.result != null );

            // transform it to HTML and display it
            XslTransform transform = CommandBase.GetTransform( context, "log.xsl" );
            StringWriter writer = new StringWriter();
            this.result.Transform( transform, writer );

            // display the HTML with the filename as caption
            context.UIShell.DisplayHtml( "Log", writer.ToString(), false );

        }

        private void ProgressCallback( IContext context )
        {
            this.result = new LogResult();
            this.result.Start();

            string[] paths = SvnItem.GetPaths( info.CheckedItems );
            context.Client.Log( paths, info.RevisionStart, info.RevisionEnd, true, 
                info.StopOnCopy, new LogMessageReceiver(result.Receive) );

            this.result.End();
        }


        private LogDialogInfo info;
        private LogResult result;
      
	}
}
