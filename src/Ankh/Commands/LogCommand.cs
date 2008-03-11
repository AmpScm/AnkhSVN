// $Id$
using System;

using System.Diagnostics;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Collections;
using SharpSvn;
using System.Collections.ObjectModel;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to show the change log for the selected item.
    /// </summary>
    [Command(AnkhCommand.Log)]
	public class LogCommand : CommandBase
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
            XslCompiledTransform transform = CommandBase.GetTransform( context, "log.xsl" );
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
            args.StrictNodeHistory = info.StopOnCopy;
            args.Log += new EventHandler<SvnLogEventArgs>(result.Receive);

            Collection<SvnLogEventArgs> logItems;
            context.Client.GetLog(paths[0], args, out logItems);

            this.result.End();
        }


        private LogDialogInfo info;
        private LogResult result;
    }
}
