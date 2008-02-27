// $Id$
using System;
using NUnit.Framework;
using System.Xml;
using NSvn.Core;
using System.IO;
using System.Xml.Xsl;
using System.Xml.XPath;
using SharpSvn;


namespace Ankh.Tests
{
    /// <summary>
    /// Summary description for BlameResultTest.
    /// </summary>
    [TestFixture]
    public class BlameResultTest : NSvn.Core.Tests.TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp ();
        }

        [Test]
        public void TestSingleLine()
        {
            BlameResult br = new BlameResult();
            br.Start();
            DateTime now = DateTime.Now;
            /*br.Receive( 0, 42, "Arild", 
                now, "printf(\"Hello world\");" );*/
            br.End();

            Assert.AreEqual( 1, br.XmlDocument.SelectNodes( "/BlameResult/Blame" ).Count );

            XmlNode node = br.XmlDocument.SelectSingleNode( "/BlameResult/Blame" );
            Assert.AreEqual( "1", node.SelectSingleNode( "LineNumber" ).InnerText );
            Assert.AreEqual( "42", node.SelectSingleNode( "Revision" ).InnerText );
            Assert.AreEqual( "Arild", node.SelectSingleNode( "Author").InnerText );
            Assert.AreEqual( now.ToString( "s" ), node.SelectSingleNode( "Date").InnerText );
            Assert.AreEqual( "printf(\"Hello world\");", node.SelectSingleNode( "Line" ).InnerText );                        
        }

        [Test]
        public void TestRealWc()
        {
            this.ExtractWorkingCopy();
            this.ExtractRepos();

            string path = Path.Combine( this.WcPath, "Class1.cs" );

            BlameResult br = new BlameResult();
            br.Start();

            this.Client.Blame( path, new EventHandler<SvnBlameEventArgs>( br.Receive ) );

            br.End();

            string[] lines = this.RunCommand( "svn", "blame -v " + path ).Trim().Split( '\n' );

            Assert.AreEqual( lines.Length, br.XmlDocument.SelectNodes( "/BlameResult/Blame" ).Count );
            
        }

        [Test]
        public void TestTransform()
        {
            BlameResult br = new BlameResult();
            br.Start();
            DateTime now = DateTime.Now;
            /*br.Receive( 0, 42, "Arild", 
                now, "printf(\"Hello world\");" );*/
            br.End();

            XPathDocument doc = new XPathDocument( new StringReader( 
                @"<xsl:stylesheet version='1.0'
                    xmlns:xsl='http://www.w3.org/1999/XSL/Transform'> 
<xsl:output method='text'/>
<xsl:template match='/BlameResult/Blame'>
    <xsl:value-of select='Author'/> - <xsl:value-of select='Revision'/>
</xsl:template>
</xsl:stylesheet>" ) );
            
            XslTransform transform = new XslTransform();
            transform.Load( doc, null, null  );

            StringWriter writer = new StringWriter();
            br.Transform( transform, writer );

            Assert.IsTrue( writer.ToString().IndexOf( "Arild - 42" ) >= 0 );
        }
    }
}
