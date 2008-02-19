// $Id$
using System;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Diagnostics;
using SharpSvn;

namespace Ankh
{
    /// <summary>
    /// Summary description for BlameResult.
    /// </summary>
    public class BlameResult : XmlResultBase
    {
        public BlameResult() : base( "BlameResult" )
        {          
            
        }    

        public void Receive( object sender, SvnBlameEventArgs e)
        {
            this.Writer.WriteStartElement( "Blame" );

            this.Writer.WriteStartElement( "LineNumber" );
            this.Writer.WriteString( (e.LineNumber + 1).ToString() );
            this.Writer.WriteEndElement();

            this.Writer.WriteStartElement( "Revision" );
            this.Writer.WriteString( e.Revision.ToString() );
            this.Writer.WriteEndElement();

            this.Writer.WriteStartElement( "Author" );
            this.Writer.WriteString( e.Author );
            this.Writer.WriteEndElement();

            this.Writer.WriteStartElement( "Date" );
            this.Writer.WriteString( e.Time.ToLocalTime().ToString( "s" ) );
            this.Writer.WriteEndElement();

            this.Writer.WriteStartElement( "Line" );
            this.Writer.WriteCData( e.Line.TrimEnd() );
            this.Writer.WriteEndElement();

            this.Writer.WriteEndElement();
        }
    }
}
