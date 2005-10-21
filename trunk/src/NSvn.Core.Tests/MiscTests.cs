using System;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.IO;

namespace NSvn.Core.Tests
{
    /// <summary>
    /// Contains tests for various Client functions that don't merit their own test fixture
    /// </summary>
    [TestFixture]
    public class MiscTests : TestBase
    {
        public override void SetUp()
        {
            base.SetUp();

            this.ExtractRepos();
            this.ExtractWorkingCopy();
        }

        /// <summary>
        /// Tests the Client::UrlFromPath function.
        /// </summary>
        [Test]
        public void TestUrlFromDirPath()
        {
            string info = this.RunCommand( "svn", "info " + this.WcPath );
            string realUrl = this.GetUrl( this.WcPath );
            string url = this.Client.UrlFromPath( this.WcPath );

            Assert.AreEqual( realUrl, url, "URL wrong" );
        }

        [Test]
        public void TestUrlFromFilePath()
        {
            string formPath = Path.Combine( this.WcPath, "Form.cs" );
            string realUrl = this.GetUrl( formPath );
            string url = this.Client.UrlFromPath( formPath );

            Assert.AreEqual( realUrl, url, "URL wrong" );
            
        }

        [Test]
        [Ignore( "Dunno how this is supposed to work" )]
        public void TestUrlFromUnversionedPath()
        {
            string url = this.Client.UrlFromPath( @"C:\" );
            Assert.IsNull( url, "Url should be null for an unversioned path" );
        }

        [Test]
        public void TestUuidFromUrl()
        {
            string realUuid = this.RunCommand( "svnlook", "uuid " + this.ReposPath ).Trim();

            string uuid = this.Client.UuidFromUrl( this.ReposUrl );
            Assert.AreEqual( realUuid, uuid, "UUID wrong" );
        }

        [Test]
        [ExpectedException(typeof(OperationCancelledException))]
        public void TestCancel()
        {
            this.Client.AuthBaton.Add( AuthenticationProvider.GetUsernameProvider() );
            this.Client.Cancel += new CancelDelegate(this.Cancel);

            this.Client.Update( this.WcPath, Revision.Head, true );
            
            Assert.IsTrue( this.cancels > 0, "No cancellation callbacks" );

            this.Client.Cancel -= new CancelDelegate(this.Cancel);
            this.Client.Cancel += new CancelDelegate(this.ReallyCancel);
            this.Client.Update( this.WcPath, Revision.Head, true );
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool SetEnvironmentVariable( string name, string value );

        [Test]
        public void TestChangeAdminDirectoryName()
        {
            string newAdminDir = "_svn";
            Client.AdminDirectoryName = newAdminDir;
            try
            {
                Assert.AreEqual( newAdminDir, Client.AdminDirectoryName,
                    "Admin directory name should now be " + newAdminDir );

                string newwc = this.FindDirName( Path.Combine( Path.GetTempPath(), "moo" ) );
                this.Client.Checkout( this.ReposUrl, newwc, Revision.Head, true );

                Assert.IsTrue( Directory.Exists( Path.Combine( newwc, newAdminDir ) ), 
                    "Admin directory with new name not found" );
            }
            finally
            {
                Client.AdminDirectoryName = ".svn";
                Assert.AreEqual(".svn", Client.AdminDirectoryName = ".svn", "Settings original admin dir failed");
            }
        }

        [Test]
        public void TestHasBinaryProp()
        {
            // first on a file
            Assert.IsFalse( this.Client.HasBinaryProp( Path.Combine( 
                this.WcPath, "Form.cs" ) ) );

            Assert.IsTrue( this.Client.HasBinaryProp( Path.Combine(
                this.WcPath, "App.ico" ) ) );

            // check what happens for a dir
            Assert.IsFalse( this.Client.HasBinaryProp( this.WcPath ) );

        }

        /// <summary>
        /// Test the Client::IsIgnored method.
        /// </summary>
        [Test]
        public void TestIsFileIgnored()
        {
            string ignored = this.CreateTextFile( "foo.bar" );
            this.RunCommand( "svn", "ps svn:ignore foo.bar " + this.WcPath );

            Assert.IsTrue( this.Client.IsIgnored( ignored ) );
            Assert.IsFalse( this.Client.IsIgnored( 
                Path.Combine( this.WcPath, "Form1.cs" ) ) );
        }

        [Test]
        public void TestIsDirectoryIgnored()
        {
            string ignored = Path.Combine( this.WcPath, "Foo" );
            Directory.CreateDirectory( ignored );
            this.RunCommand( "svn", "ps svn:ignore Foo " + this.WcPath );

            Assert.IsTrue( this.Client.IsIgnored( ignored ) );

        }

        private string GetUrl( string path )
        {
            string info = this.RunCommand( "svn", "info " + path );
            return Regex.Match( info, @"URL: (.*)", RegexOptions.IgnoreCase ).Groups[1].ToString().Trim();
        }

        private void Cancel( object sender, CancelEventArgs args )
        {
            this.cancels++;
            args.Cancel = false;
        }

        private void ReallyCancel( object sender, CancelEventArgs args )
        {
            this.cancels++;
            args.Cancel = true;
        }

        private int cancels = 0;

        public static void Main()
        {
            MiscTests t = new MiscTests();
            t.SetUp();
            t.TestChangeAdminDirectoryName();
        }

    }
}
