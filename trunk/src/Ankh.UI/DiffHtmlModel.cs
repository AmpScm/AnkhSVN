using System;
using System.Drawing;
using System.IO;

namespace Ankh.UI
{
    /// <summary>
    /// Represents a diff and its conversion to a colored HTML representation.
    /// </summary>
    public class DiffHtmlModel
    {
        public DiffHtmlModel( string diff )
        {
            this.diff = diff;
        }

        public DiffHtmlModel() : this( "" )
        {
        }

        public string Diff
        {
            get{ return this.diff; }
            set{ this.diff = value; }
        }

        /// <summary>
        /// The color to use for added lines.
        /// </summary>
        public Color AddedLine
        {
            get{ return this.addedLine; }
            set{ this.addedLine = value; }
        }

        /// <summary>
        /// The color to use for removed lines.
        /// </summary>
        public Color RemovedLine
        {
            get{ return this.removedLine; }
            set{ this.removedLine = value; }
        }

        /// <summary>
        /// The color to use for normal lines.
        /// </summary>
        public Color NormalLine
        {
            get{ return this.normalLine; }
            set{ this.normalLine = value; }
        }

        /// <summary>
        /// The color to use for @@ lines.
        /// </summary>
        public Color AtLine
        {
            get{ return this.atLine; }
            set{ this.AtLine = value; }
        }

        public string GetHtml()
        {
            using( StringWriter writer = new StringWriter() )
            {
                this.WriteProlog( writer );
                string escapedDiff = this.diff.Replace( "<", "&lt;" ).
                    Replace( ">", "&gt;" );

                using( StringReader reader = new StringReader( escapedDiff ) )
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

                return writer.ToString();
            }            
        }

        /// <summary>
        /// Retrieve the CSS class for a line, based on it's first chars.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string GetClass( string line )
        {
            if ( line.Length == 0 )
                return "default";

            switch( line[0] )
            {
                case '+':
                    return "plus";
                case '-':
                    return "minus";   
                case '@':
                    return "at";
                default:
                    return "default";
            }           
        }

        /// <summary>
        /// Write out the standard housekeeping stuff, and the inline stylesheet
        /// </summary>
        private void WriteProlog( TextWriter writer )
        {
            
            writer.WriteLine( 
                @"<html>
   <head> 
      <title>Diff</title>
      <style type='text/css'>
       <!--
          .plus {{  color: {0};  }}
          .minus {{ color: {1}; }}
          .at {{ color: {2}; }}
          .default {{ color: {3};}}
       -->
      </style>
</head>
<body>
    <pre>", this.Convert( this.AddedLine ),
                this.Convert( this.RemovedLine ),
                this.Convert( this.AtLine ),
                this.Convert( this.NormalLine ) );
        }

        /// <summary>
        /// Standard housekeeping stuff to round off the html doc.
        /// </summary>
        /// <param name="writer"></param>
        private void WriteEpilog( TextWriter writer )
        {
            writer.WriteLine( "</pre></body></html>" );
        }

        private string Convert( Color color )
        {
            return string.Format( "#{0:x2}{1:x2}{2:x2}", color.R, color.G,
                color.B );
        }

        private string diff;
        private Color removedLine = Color.Red;
        private Color addedLine = Color.Blue;
        private Color atLine = Color.Brown;
        private Color normalLine = Color.Green;
    }
}
