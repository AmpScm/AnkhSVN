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
            string realUrl = Regex.Match( info, @"Url: (.*)" ).Groups[1].ToString().Trim();

            string url = Client.UrlFromPath( this.WcPath );

            Assertion.AssertEquals( "URL wrong", realUrl, url );
        }

        [Test]
        public void TestUrlFromFilePath()
        {
            string formPath = Path.Combine( this.WcPath, "Form.cs" );
            string info = this.RunCommand( "svn", "info " + formPath );
            string realUrl = Regex.Match( info, @"Url: (.*)" ).Groups[1].ToString().Trim();

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

    }
}
