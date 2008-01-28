// $Id$
using System;
using System.Xml;
using NUnit.Framework;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;


namespace NSvn.Core.Tests
{
    /// <summary>
    /// Tests Client::Log
    /// </summary>
    [TestFixture]
    public class LogTest : TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.ExtractRepos();
			this.logMessages.Clear();
        }

        [Test]
        public void TestLog()
        {
            ClientLogMessage[] clientLogs = this.ClientLog( this.ReposUrl );

            //the client prints them in a reverse order by default
            Array.Reverse( clientLogs );

            this.Client.Log( new string[]{ this.ReposUrl },
                Revision.FromNumber(1), Revision.Head, false, false, 
                new LogMessageReceiver(this.LogCallback) );

            Assert.AreEqual( clientLogs.Length, this.logMessages.Count, 
				"Number of log entries differs" );
            for( int i = 0; i < this.logMessages.Count; i++ )
                clientLogs[i].CheckMatch( (LogMessage)this.logMessages[i] );                        
        }

		[Test]
		public void TestLogNonAsciiChars()
		{
			this.RunCommand( "svn", "propset svn:log --revprop -r 1  \"Æ e i a æ å, sjø\" " + 
				this.ReposUrl );
			this.Client.Log( new string[]{ this.ReposUrl }, 
				Revision.FromNumber(1), Revision.FromNumber(1), false, false, 
				new LogMessageReceiver(this.LogCallback) );
			Assert.AreEqual( 1, this.logMessages.Count );
			Assert.AreEqual( "Æ e i a æ å, sjø", ((LogMessage)this.logMessages[0]).Message );
		}

        private void LogCallback( LogMessage logMessage )
        {
            this.logMessages.Add( logMessage );
        }

        private ClientLogMessage[] ClientLog( string path )
        {
            string output = this.RunCommand( "svn", "log --xml " + path );
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.LoadXml( output );

            ArrayList list = new ArrayList();

            foreach( XmlNode node in doc.SelectNodes( "/log/logentry" ) )
                list.Add( new ClientLogMessage( node ) );

            return (ClientLogMessage[])list.ToArray( typeof(ClientLogMessage) );
        }

        /// <summary>
        /// Represents a log message gotten from the command line client
        /// </summary>
        private class ClientLogMessage
        {
            public ClientLogMessage( XmlNode node )
            {
                this.date = this.ParseTime(node[ "date" ].InnerText);
                this.revision = int.Parse( node.Attributes["revision"].InnerText );

                // need to get rid of some redundant whitespace
                // (probably introduced somewhere in the XML process)
                string msg = node["msg"].InnerText;
                string[] lines = Regex.Split(msg, Environment.NewLine );
                for( int i = 0; i < lines.Length; i++ )
                    lines[i] = lines[i].Trim();
                
                this.message = string.Join( Environment.NewLine, lines );

                this.author = node["author"].InnerText;
            }

            /// <summary>
            /// Does it match the LogMessage object
            /// </summary>
            /// <param name="msg"></param>
            public void CheckMatch( LogMessage msg )
            {
                Assert.AreEqual( this.author, msg.Author, "Author differs" );
                Assert.AreEqual( this.message, msg.Message, "Message differs" );
                Assert.AreEqual( this.revision, msg.Revision, "Revision differs" );
                Assert.AreEqual( this.date, msg.Date, "Date differs" );
            }

            private DateTime ParseTime( string dateString )
            {
                return DateTime.ParseExact( dateString,  
                    @"yyyy-MM-dd\THH:mm:ss.ffffff\Z", 
                    CultureInfo.CurrentCulture );
            }

            private DateTime date;
            private string author;
            private string message;
            private int revision;


        }

        private ArrayList logMessages = new ArrayList();
    }
}
