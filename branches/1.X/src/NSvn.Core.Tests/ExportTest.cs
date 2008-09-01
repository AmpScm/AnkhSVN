// $Id$
using System;
using System.IO;
using NUnit.Framework;

using NSvn.Common;

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
            this.Client.Export( this.ReposUrl, this.newWc, Revision.Head, false );

            Assert.IsTrue( File.Exists( Path.Combine( this.newWc, "Form.cs" ) ), 
                "Exported file not there" );
            Assert.IsTrue( !Directory.Exists( Path.Combine( this.newWc, Client.AdminDirectoryName ) ), 
                ".svn directory found" );
        }
        /// <summary>
        /// Test export operation from a working copy
        /// </summary>
        [Test]
        public void TestExportWc()
        {   
            this.Client.Export( this.WcPath, this.newWc, Revision.Head, false );

            Assert.IsTrue( File.Exists( Path.Combine( this.newWc, "Form.cs" ) ), 
                "Exported file not there" );
            Assert.IsTrue( !Directory.Exists( Path.Combine( this.newWc, Client.AdminDirectoryName ) ), 
                ".svn directory found" );
        }

        [Test]
        public void TestExportNonRecursive()
        {
            this.Client.Export( this.WcPath, this.newWc, Revision.Unspecified, Revision.Head,
                false, false, Recurse.None, null );
            Assert.AreEqual( 0, Directory.GetDirectories( this.newWc ).Length );
        }       

        private string newWc;

    }
}
