// $Id$
using System;
using System.IO;
using NUnit.Framework;


namespace NSvn.Core.Tests
{
    /// <summary>
    /// Tests the Client::Cleanup method
    /// </summary>
    [TestFixture]
    public class CleanupTest : TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            this.ExtractWorkingCopy();
        }

        /// <summary>
        /// Test that Client::Cleanup removes the lock file
        /// </summary>
        [Test]
        public void TestRemoveLockFile()
        {
            ClientContext ctx = new ClientContext( new NotifyCallback( this.NotifyCallback ) );
            string lockPath = Path.Combine( this.WcPath, ".svn" );
            lockPath = Path.Combine( lockPath, "lock" );

            File.CreateText( lockPath ).Close();

            Client.Cleanup( this.WcPath, ctx );

            Assertion.Assert( "lock file still in place after running Client::Cleanup",
                !File.Exists( lockPath ) );
        }
		
    }
}
