// $Id$
using System;
using EnvDTE;
using NSvn;
using System.IO;
using System.Text;
using SHDocVw;
using Ankh.UI;

namespace Ankh.Commands
{
    /// <summary>
    /// Summary description for DiffLocalItem.
    /// </summary>
    [VSNetCommand( "DiffLocalItem", Text="Diff", Tooltip="Diff de dah" ),
    VSNetControl( "Item", Position=2 ),
    VSNetControl( "Project", Position = 2 ),
    VSNetControl( "Solution", Position = 2 ) ]
    internal class DiffLocalItem : CommandBase
    {
		
        public override EnvDTE.vsCommandStatus QueryStatus(Ankh.AnkhContext context)
        {
            // only allow diff if all selected items are modified
            ModifiedVisitor v = new ModifiedVisitor();
            context.SolutionExplorer.VisitSelectedItems( v, true );
            if ( v.Modified )
                return vsCommandStatus.vsCommandStatusEnabled |
                    vsCommandStatus.vsCommandStatusSupported;
            else
                return vsCommandStatus.vsCommandStatusUnsupported;
        }

        public override void Execute(Ankh.AnkhContext context)
        {
            // get the diff itself
            DiffVisitor v = new DiffVisitor();
            context.SolutionExplorer.VisitSelectedItems( v, true );

            // convert it to HTML and store in a temp file
            DiffHtmlModel model = new DiffHtmlModel( v.Diff );
            string html = model.GetHtml();
            string htmlFile = Path.GetTempFileName();
            using( StreamWriter w = new StreamWriter( htmlFile ) )
                w.Write( html );

            // the Start Page window is a web browser
            Window browserWindow = context.DTE.Windows.Item( 
                Constants.vsWindowKindWebBrowser );
            WebBrowser browser = (WebBrowser)browserWindow.Object;

            // have it show the html
            object url = "file://" + htmlFile;
            object nullObject = null;
            browser.Navigate2( ref url, ref nullObject, ref nullObject,
                ref nullObject, ref nullObject );
            browserWindow.Activate();            
        } 
    }
}



