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
        [ExpectedException(typeof(WorkingCopyLockedException))]
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

    }
}
