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
    public class BlameResult
    {
        public BlameResult()
        {
            this.xmlDocument = new XmlDocument();
            
        }

        public XmlDocument XmlDocument
        {
            get{ return this.xmlDocument; }
        }

        public void Transform( XslTransform transform, TextWriter writer )
        {
            transform.Transform( this.XmlDocument, new XsltArgumentList(), writer, null );
        }

        public void Start()
        {
            this.stream = new MemoryStream();
            this.writer = new XmlTextWriter( this.stream, System.Text.Encoding.UTF8 );
            ((XmlTextWriter)this.writer).Formatting = Formatting.Indented;
            ((XmlTextWriter)this.writer).Indentation =4;
            

            this.writer.WriteStartDocument();
            this.writer.WriteStartElement( "BlameResult" );
        }

        public void End()
        {
            Debug.Assert( this.stream != null );
            Debug.Assert( this.writer != null );

            this.writer.WriteEndElement();
            this.writer.WriteEndDocument();
            this.writer.Flush();

            this.stream.Seek( 0L, SeekOrigin.Begin );
            string s = System.Text.Encoding.UTF8.GetString( ((MemoryStream)this.stream).GetBuffer() );
            this.xmlDocument.Load( this.stream );
        }

        public void Receive( long linenumber, int revision, string author, DateTime date, 
            string line )
        {
            this.writer.WriteStartElement( "Blame" );

            this.writer.WriteStartElement( "LineNumber" );
            this.writer.WriteString( (linenumber + 1).ToString() );
            this.writer.WriteEndElement();

            this.writer.WriteStartElement( "Revision" );
            this.writer.WriteString( revision.ToString() );
            this.writer.WriteEndElement();

            this.writer.WriteStartElement( "Author" );
            this.writer.WriteString( author );
            this.writer.WriteEndElement();

            this.writer.WriteStartElement( "Date" );
            this.writer.WriteString( date.ToString( "s" ) );
            this.writer.WriteEndElement();

            this.writer.WriteStartElement( "Line" );
            this.writer.WriteCData( line.TrimEnd() );
            this.writer.WriteEndElement();

            this.writer.WriteEndElement();
        }

        

        private XmlDocument xmlDocument;
        private XmlWriter writer;
        private Stream stream;
    }
}
