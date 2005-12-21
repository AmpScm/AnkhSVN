// $Id$
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
            string switchUrl = Path.Combine( this.ReposUrl, "doc" );
            string checkFile = Path.Combine( this.WcPath, "text_r5.txt" );

            this.Client.Switch( this.WcPath, switchUrl, Revision.Head, true );
            Assert.IsTrue( File.Exists( checkFile ), "Didn't switch to repos/doc" );

        }
        private string path;
    }
}