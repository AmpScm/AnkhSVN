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

            Assertion.AssertEquals( "URL wrong", realUrl, url );
        }

        [Test]
        public void TestUrlFromFilePath()
        {
            string formPath = Path.Combine( this.WcPath, "Form.cs" );
            string realUrl = this.GetUrl( formPath );
            string url = this.Client.UrlFromPath( formPath );

            Assertion.AssertEquals( "URL wrong", realUrl, url );
            
        }

        [Test]
        [Ignore( "Dunno how this is supposed to work" )]
        public void TestUrlFromUnversionedPath()
        {
            string url = this.Client.UrlFromPath( @"C:\" );
            Assertion.AssertNull( "Url should be null for an unversioned path", url );
        }

        [Test]
        public void TestUuidFromUrl()
        {
            string realUuid = this.RunCommand( "svnlook", "uuid " + this.ReposPath ).Trim();

            string uuid = this.Client.UuidFromUrl( this.ReposUrl );
            Assertion.AssertEquals( "UUID wrong", realUuid, uuid );
        }

        [Test]
        [ExpectedException(typeof(OperationCancelledException))]
        public void TestCancel()
        {
            this.Client.AuthBaton.Add( AuthenticationProvider.GetUsernameProvider() );
            this.Client.Cancel += new CancelDelegate(this.Cancel);

            this.Client.Update( this.WcPath, Revision.Head, true );
            
            Assertion.Assert( "No cancellation callbacks", this.cancels > 0 );

            this.Client.Cancel -= new CancelDelegate(this.Cancel);
            this.Client.Cancel += new CancelDelegate(this.ReallyCancel);
            this.Client.Update( this.WcPath, Revision.Head, true );
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool SetEnvironmentVariable( string name, string value );

        [Test]
        [System.Diagnostics.Conditional("ALT_ADMIN_DIR")]
        public void TestChangeAdminDirectoryName()
        {
#if ALT_ADMIN_DIR
            Client.AdminDirectoryName = "__SVN__";
            try
            {
                Assertion.AssertEquals( "Admin directory name should now be __SVN__",
                    "__SVN__", Client.AdminDirectoryName );

                string newwc = this.FindDirName( Path.Combine( Path.GetTempPath(), "moo" ) );
                this.Client.Checkout( this.ReposUrl, newwc, Revision.Head, true );

                Assertion.Assert( "Admin directory with new name not found", 
                    Directory.Exists( Path.Combine( newwc, "__SVN__" ) ) );
            }
            finally
            {
                Client.AdminDirectoryName = ".svn";
            }
#endif
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
