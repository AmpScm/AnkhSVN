// $Id$
using System.IO;
using Ankh.UI;
using EnvDTE;
using SHDocVw;

namespace Ankh.Commands
{
    /// <summary>
    /// Summary description for DiffLocalItem.
    /// </summary>
    [VSNetCommand( "DiffLocalItem", Text="Diff", 
         Tooltip="Diff against local text base.", 
         Bitmap = ResourceBitmaps.Diff),
    VSNetControl( "Item", Position=2 ),
    VSNetProjectNodeControl( "", Position = 2 ),
    VSNetControl( "Solution", Position = 2 ),
    VSNetControl( "Folder", Position = 2 )]
    internal class DiffLocalItem : LocalDiffCommandBase
    {
        public override void Execute(Ankh.AnkhContext context, string parameters)
        {
            try
            {
                this.SaveAllDirtyDocuments( context );

                context.StartOperation( "Diffing" );

                string diff = this.GetDiff( context );
                if (diff != null)
                {
                    // convert it to HTML and store in a temp file
                    DiffHtmlModel model = new DiffHtmlModel( diff);
                    string html = model.GetHtml();
                    string htmlFile = Path.GetTempFileName();
                    using( StreamWriter w = new StreamWriter( htmlFile, false, System.Text.Encoding.Default ) )
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
            finally
            {
                context.EndOperation();
            }

        } 
    }
}



