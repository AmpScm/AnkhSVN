using System;
using NUnit.Framework;
using System.IO;

namespace NSvn.Core.Tests
{
    ///<summary>
    ///Test Client::Switc
    ///</summary>
    [TestFixture]
    public class SwitchTest : TestBase
    {
       [SetUp]
        public override void SetUp() 
        {
           base.SetUp();
           this.path = Path.GetTempPath();
           base.ExtractRepos();
           base.ExtractWorkingCopy();
        }

        /// <summary>
        /// Try to switch wc to repos/doc
        /// </summary>
        [Test]
        public void TestSwitchUrl()
        {
            string workingPath = Path.Combine( this.path, this.WcPath );
            string switchUrl = Path.Combine( this.ReposUrl, "doc" );
            string checkFile = Path.Combine( this.WcPath, "text_r5.txt" );
            ClientContext ctx = new ClientContext( new NotifyCallback ( this.NotifyCallback ) );

            Client.Switch( workingPath, switchUrl, Revision.Head, true, ctx );
            Assertion.Assert( " Didn't switch to repos/doc", File.Exists( checkFile ) );

        }
        private string path;
    }
}