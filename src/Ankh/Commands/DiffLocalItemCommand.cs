// $Id$
using System;
using EnvDTE;
using NSvn;
using System.IO;
using System.Text;
using SHDocVw;

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
            string htmlFile = this.GetColoredHtmlFile( v.Diff );

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
     
        private string GetColoredHtmlFile( string diff )
        {
            string file = Path.GetTempFileName();
            using( StreamWriter writer = new StreamWriter( file ) )
            {
                this.WriteProlog( writer );
                using( StringReader reader = new StringReader( diff ) )
                {
                    string line;
                    while( (line=reader.ReadLine()) != null )
                    {
                        // get the css class for this line
                        string cssClass = this.GetClass( line );
                        writer.WriteLine( "<span class='{0}'>{1}</span>", cssClass, line );
                    }
                }
                this.WriteEpilog( writer );
            }
            return file;    
        }

        /// <summary>
        /// Retrieve the CSS class for a line, based on it's first chars.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string GetClass( string line )
        {
            switch( line[0] )
            {
                case '+':
                    return "plus";
                case '-':
                    return "minus";                            
                default:
                    return "default";
            }           
        }

        /// <summary>
        /// Write out the standard housekeeping stuff, and the inline stylesheet
        /// </summary>
        private void WriteProlog( StreamWriter writer )
        {
            writer.WriteLine( 
                @"<html>
   <head> 
      <title>Diff</title>
      <style type='text/css'>
       <!--
          .plus {  color: blue;  }
          .minus { color: red; }
          .default { color: green;}
       -->
      </style>
</head>
<body>
    <pre>" );
        }

        /// <summary>
        /// Standard housekeeping stuff to round off the html doc.
        /// </summary>
        /// <param name="writer"></param>
        private void WriteEpilog( StreamWriter writer )
        {
            writer.WriteLine( "</pre></body></html>" );
        }
    }
}



