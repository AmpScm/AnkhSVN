using System;
using NUnit.Framework;
using NSvn.Core;
using System.IO;
using System.Collections;

namespace NSvn.Core.Tests
{
	/// <summary>
	/// Tests NSvn::Core::Client::Add
	/// </summary>
	[TestFixture]
	public class AddTest : TestBase
	{
        [SetUp]
        public void SetUp()
        {
            this.ExtractWorkingCopy();

            this.notifications = new ArrayList();
        }

        /// <summary>
        /// Attempts to add a file, checking that the file actually was added
        /// </summary>
        [Test]
        public void TestBasic()
        {
            string testFile = Path.Combine( this.WcPath, "testfile.txt" );
            using ( StreamWriter writer = File.CreateText( testFile ) )
                writer.Write( "Hello world" );

            ClientContext ctx = new ClientContext( new NotifyCallback( this.NotifyCallback ) );
            Client.Add( testFile, false, ctx );    
       
            Assertion.Assert( "No notification callbacks received", this.notifications.Count > 0 );

            Assertion.AssertEquals( "svn st does not report the file as added", 
                'A', this.GetSvnStatus( testFile ) );
                        
        }

        public void NotifyCallback( Notification notification )
        {
            this.notifications.Add( notification );
        }

        private ArrayList notifications;
		
	}
}
