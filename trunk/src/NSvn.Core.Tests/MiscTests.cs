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
            string url = Client.UrlFromPath( this.WcPath );

            Assertion.AssertEquals( "URL wrong", realUrl, url );
        }

        [Test]
        public void TestUrlFromFilePath()
        {
            string formPath = Path.Combine( this.WcPath, "Form.cs" );
            string realUrl = this.GetUrl( formPath );
            string url = Client.UrlFromPath( formPath );

            Assertion.AssertEquals( "URL wrong", realUrl, url );
            
        }

        [Test]
        [Ignore( "Dunno how this is supposed to work" )]
        public void TestUrlFromUnversionedPath()
        {
            string url = Client.UrlFromPath( @"C:\" );
            Assertion.AssertNull( "Url should be null for an unversioned path", url );
        }

        [Test]
        public void TestUuidFromUrl()
        {
            string realUuid = this.RunCommand( "svnlook", "uuid " + this.ReposPath ).Trim();

            string uuid = Client.UuidFromUrl( this.ReposUrl, new ClientContext() );
            Assertion.AssertEquals( "UUID wrong", realUuid, uuid );
        }

        [Test]
        [ExpectedException( typeof(WorkingCopyLockedException) )]
        public void TestCancel()
        {
            ClientContext ctx = new ClientContext();
            AuthenticationBaton baton = new AuthenticationBaton();
            ctx.AuthBaton = baton;
            ctx.AuthBaton.Add( AuthenticationProvider.GetUsernameProvider() );
            ctx.CancelCallback = new CancelCallback( this.CancelCallback );

            Client.Update( this.WcPath, Revision.Head, true, ctx );
            
            Assertion.Assert( "No cancellation callbacks", this.cancels > 0 );

            // no idea why this throws *WorkingCopyLockedException*...
            ctx.CancelCallback = new CancelCallback( this.ReallyCancelCallback );
            Client.Update( this.WcPath, Revision.Head, true, ctx );
        }

        private string GetUrl( string path )
        {
            string info = this.RunCommand( "svn", "info " + path );
            return Regex.Match( info, @"URL: (.*)", RegexOptions.IgnoreCase ).Groups[1].ToString().Trim();
        }

        private CancelOperation CancelCallback()
        {
            this.cancels++;
            return CancelOperation.DontCancel;
        }

        private CancelOperation ReallyCancelCallback()
        {
            this.cancels++;
            return CancelOperation.Cancel;
        }

        private int cancels = 0;

    }
}
