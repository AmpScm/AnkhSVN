// $Id$
using System;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Diagnostics;

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

        public void Receive( long linenumber, int revision, string author, DateTime date, 
            string line )
        {
            this.Writer.WriteStartElement( "Blame" );

            this.Writer.WriteStartElement( "LineNumber" );
            this.Writer.WriteString( (linenumber + 1).ToString() );
            this.Writer.WriteEndElement();

            this.Writer.WriteStartElement( "Revision" );
            this.Writer.WriteString( revision.ToString() );
            this.Writer.WriteEndElement();

            this.Writer.WriteStartElement( "Author" );
            this.Writer.WriteString( author );
            this.Writer.WriteEndElement();

            this.Writer.WriteStartElement( "Date" );
            this.Writer.WriteString( date.ToString( "s" ) );
            this.Writer.WriteEndElement();

            this.Writer.WriteStartElement( "Line" );
            this.Writer.WriteCData( line.TrimEnd() );
            this.Writer.WriteEndElement();

            this.Writer.WriteEndElement();
        }
    }
}
