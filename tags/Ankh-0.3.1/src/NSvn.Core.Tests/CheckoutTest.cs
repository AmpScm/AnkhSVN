// $Id$
using System;
using System.IO;
using NUnit.Framework;

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
            this.RecursiveDelete( this.newWc );
        }

        /// <summary>
        /// Test a standard checkout operation
        /// </summary>
        [Test]
        public void TestBasicCheckout()
        {
            ClientContext ctx = new ClientContext( new NotifyCallback( this.NotifyCallback ) );
            Client.Checkout( this.ReposUrl, this.newWc, Revision.Head, true, ctx );

            Assertion.Assert( "Checked out file not there", 
                File.Exists( Path.Combine( this.newWc, "Form.cs" ) ) );
            Assertion.Assert( "No .svn directory found", 
                Directory.Exists( Path.Combine( this.newWc, ".svn" ) ) );
            Assertion.AssertEquals( "Wrong status", "", this.RunCommand( "svn", "st " + 
                this.newWc ).Trim() );
        }

        private string newWc;

    }
}
