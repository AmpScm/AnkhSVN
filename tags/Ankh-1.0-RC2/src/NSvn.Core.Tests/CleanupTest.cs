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
            base.SetUp();

            this.ExtractWorkingCopy();
        }

        /// <summary>
        /// Test that Client::Cleanup removes the lock file
        /// </summary>
        [Test]
        public void TestRemoveLockFile()
        {
            string lockPath = Path.Combine( this.WcPath, Client.AdminDirectoryName );
            lockPath = Path.Combine( lockPath, "lock" );

            File.CreateText( lockPath ).Close();

            this.Client.Cleanup( this.WcPath );

            Assert.IsTrue( !File.Exists( lockPath ),
                "lock file still in place after running Client::Cleanup" );
        }
		
    }
}
