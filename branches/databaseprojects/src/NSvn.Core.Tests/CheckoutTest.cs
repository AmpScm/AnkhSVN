// $Id$
using System;
using System.IO;
using NUnit.Framework;
using Utils;

namespace NSvn.Core.Tests
{
    /// <summary>
    /// Tests for the Client::Checkout method
    /// </summary>
    [TestFixture]
    public class CheckoutTest : TestBase
    {
        [SetUp]
        public override void SetUp() 
        {
            base.SetUp();
            this.ExtractRepos();
            this.newWc = this.FindDirName( Path.Combine( Path.GetTempPath(), "moo" ) );
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            PathUtils.RecursiveDelete( this.newWc );
        }

        /// <summary>
        /// Test a standard checkout operation
        /// </summary>
        [Test]
        public void TestBasicCheckout()
        {
            this.Client.Checkout( this.ReposUrl, this.newWc, Revision.Head, true );

            Assert.IsTrue( File.Exists( Path.Combine( this.newWc, "Form.cs" ) ),
                "Checked out file not there" );
            Assert.IsTrue( Directory.Exists( Path.Combine( this.newWc, Client.AdminDirectoryName ) ), 
                "No admin directory found" );
            Assert.AreEqual( "", this.RunCommand( "svn", "st \"" + this.newWc + "\"").Trim(),
                "Wrong status" );
        }

        [Test]
        public void TestProgressEvent()
        {
                this.Client.Progress += new ProgressDelegate(Client_Progress);
                this.Client.Checkout("http://ankhsvn.com/svn/test", this.newWc, Revision.Head, true);

                Assert.IsTrue(progressCalled, "Progress delegate not called");
        }

        void Client_Progress(object sender, ProgressEventArgs args)
        {
            progressCalled = true;
        }

        private bool progressCalled = false;
        private string newWc;

    }
}
