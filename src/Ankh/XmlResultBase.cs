// $Id$
using System;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.IO;
using System.Diagnostics;

namespace Ankh
{
    /// <summary>
    /// Summary description for XmlResultBase.
    /// </summary>
    public abstract class XmlResultBase
    {
        public XmlResultBase( string rootTag )
        {
            this.xmlDocument = new XmlDocument();
            this.rootTag = rootTag;
        }

        public XmlDocument XmlDocument
        {
            get{ return this.xmlDocument; }
        }

        protected XmlWriter Writer
        {
            get{ return this.writer; }
        }

        public void Transform( XslTransform transform, TextWriter writer )
        {
            transform.Transform( this.XmlDocument, new XsltArgumentList(), writer );
        }

        public void Start()
        {
            this.stream = new MemoryStream();
            this.writer = new XmlTextWriter( this.stream, System.Text.Encoding.UTF8 );
            ((XmlTextWriter)this.writer).Formatting = Formatting.Indented;
            ((XmlTextWriter)this.writer).Indentation = 4;
            

            this.writer.WriteStartDocument();
            this.writer.WriteStartElement( this.rootTag );
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

        private XmlDocument xmlDocument;
        private XmlWriter writer;
        private Stream stream;

        private string rootTag;
    }
}
