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
    [VSNetCommand( "DiffLocalItem", Text="Diff", 
         Tooltip="Diff against local text base.", 
         Bitmap = ResourceBitmaps.Diff),
    VSNetControl( "Item", Position=2 ),
    VSNetControl( "Project", Position = 2 ),
    VSNetControl( "Solution", Position = 2 ),
    VSNetControl( "Folder", Position = 2 )]
    internal class DiffLocalItem : CommandBase
    {
		
        public override EnvDTE.vsCommandStatus QueryStatus(Ankh.AnkhContext context)
        {
            // always allow diff - worst case you get an empty diff            
            return vsCommandStatus.vsCommandStatusEnabled |
                vsCommandStatus.vsCommandStatusSupported;
            
        }

        public override void Execute(Ankh.AnkhContext context, string parameters)
        {
            try
            {
                context.StartOperation( "Diffing..." );

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
            finally
            {
                context.EndOperation();
            }

        } 
    }
}



