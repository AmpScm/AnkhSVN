using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using NUnit.Framework;
using NSvn.Core;
using System.Collections;
using System.IO;

namespace Ankh.Tests
{
	/// <summary>
	/// Tests the LogResult class.
	/// </summary>
	[TestFixture]
	public class LogResultTest : NSvn.Core.Tests.TestBase
	{
        [SetUp]
        public override void SetUp()
        {
            base.SetUp ();
            this.ExtractRepos();
            this.ExtractWorkingCopy();
            this.rs = new LogResult();
        }

        [Test]
        public void TestWc()
        {  
            string path = Path.Combine( this.WcPath, "Class1.cs" );

            this.rs.Start();            
            this.Client.Log( new string[]{ path }, Revision.FromNumber(1), Revision.Head,
                true, false, new LogMessageReceiver( this.LogCallback ) );                
            this.rs.End();

            Assert.AreEqual( this.messages.Count, this.rs.XmlDocument.SelectNodes( 
                "/LogResult/LogItem" ).Count );

            int i = 0;
            foreach( XmlNode node in this.rs.XmlDocument.SelectNodes( "/LogResult/LogItem" ) )
            {
                LogMessage msg = (LogMessage)this.messages[i];
                Assert.AreEqual( msg.Author, 
                    node.SelectSingleNode( "Author" ).InnerText );
                Assert.AreEqual( msg.Revision, 
                    int.Parse( node.SelectSingleNode( "Revision" ).InnerText ) );
                Assert.AreEqual( msg.Date.ToString( "s" ), 
                    node.SelectSingleNode( "Date" ).InnerText );
                Assert.AreEqual( msg.Message, 
                    node.SelectSingleNode( "Message" ).InnerText );
                

                foreach( XmlNode cpNode in node.SelectNodes( "ChangedPaths/ChangedPath" ) )
                {
                    string cpPath = cpNode.SelectSingleNode( "Path" ).InnerText;
                    ChangedPath cp = msg.ChangedPaths.Get( cpPath );
                    Assert.IsNotNull( cp );

                    Assert.AreEqual( cp.Action, Enum.Parse( typeof(ChangedPathAction), 
                        cpNode.SelectSingleNode( "Action" ).InnerText ));
                    if ( cp.CopyFromPath != null )
                    {
                        Assert.AreEqual( cp.CopyFromPath, cpNode.SelectSingleNode( "CopyFromPath" ) );
                        Assert.AreEqual( cp.CopyFromRevision, 
                            int.Parse( cpNode.SelectSingleNode( "CopyFromNode" ).InnerText ) );
                    }                    
                }

                i++;
            }
        }	
	
        private void LogCallback( LogMessage msg )
        {
            this.messages.Add( msg );
            this.rs.Receive( msg );
        }

        private ArrayList messages = new ArrayList();
        private LogResult rs;
	}
}
