// $Id$
using System;
using System.IO;
using NUnit.Framework;

namespace NSvn.Core.Tests
{
    /// <summary>
    /// Tests for the Client::Export method
    /// </summary>
    [TestFixture]
    public class ExportTest : TestBase
    {
        [SetUp]
        public override void SetUp() 
        {
            base.SetUp();
            this.ExtractRepos();
            this.ExtractWorkingCopy();
            this.newWc = this.FindDirName( Path.Combine( Path.GetTempPath(), "moo" ) );
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            Directory.Delete( this.newWc, true );
        }

        /// <summary>
        /// Test export operation from a repository
        /// </summary>
        [Test]
        public void TestExportRepos()
        {
            ClientContext ctx = new ClientContext();
            Client.Export( this.ReposUrl, this.newWc, Revision.Head, ctx );

            Assertion.Assert( "Exported file not there", 
                File.Exists( Path.Combine( this.newWc, "Form.cs" ) ) );
            Assertion.Assert( ".svn directory found", 
                !Directory.Exists( Path.Combine( this.newWc, ".svn" ) ) );
        }
        /// <summary>
        /// Test export operation from a working copy
        /// </summary>
        public void TestExportWc()
        { 
            ClientContext ctx = new ClientContext();
            
            Client.Export( this.WcPath, this.newWc, Revision.Head, ctx );

            Assertion.Assert( "Exported file not there", 
                File.Exists( Path.Combine( this.newWc, "Form.cs" ) ) );
            Assertion.Assert( ".svn directory found", 
                !Directory.Exists( Path.Combine( this.newWc, ".svn" ) ) );
        }

        private string newWc;

    }
}
