// $Id$
using System;
using NSvn.Core;
using System.Diagnostics;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Collections;
using SharpSvn;
using System.Collections.ObjectModel;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to show the change log for the selected item.
    /// </summary>
    [VSNetCommand("Log",
         Text = "&Log...",
         Tooltip = "Show the change log for the selected item.",
         Bitmap = ResourceBitmaps.Log),
         VSNetItemControl( VSNetControlAttribute.AnkhSubMenu, Position = 6 )]
	public class LogCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            if ( context.Selection.GetSelectionResources( false, 
                new ResourceFilterCallback( SvnItem.VersionedFilter ) ).Count > 0 )
                return Enabled;
            else
                return Disabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            IList resources = context.Selection.GetSelectionResources(
                false, new ResourceFilterCallback( SvnItem.VersionedFilter ) );

            this.info = new LogDialogInfo( resources, resources );
            this.info.RevisionStart = SvnRevision.Zero;
            this.info.RevisionEnd = SvnRevision.Head;

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

        #endregion

        private void ProgressCallback(IContext context)
        {
            this.result = new LogResult();
            this.result.Start();

            string[] paths = SvnItem.GetPaths(info.CheckedItems);

            SvnLogArgs args = new SvnLogArgs();
            args.IncludeMergedRevisions = !info.StopOnCopy;
            args.Log += new EventHandler<SvnLogEventArgs>(result.Receive);

            Collection<SvnLogEventArgs> logItems;
            context.Client.GetLog(paths[0], args, out logItems);
            //context.Client.Log( paths, info.RevisionEnd, info.RevisionStart, true, 
            //    info.StopOnCopy, new LogMessageReceiver(result.Receive) );

            this.result.End();
        }


        private LogDialogInfo info;
        private LogResult result;
    }
}
