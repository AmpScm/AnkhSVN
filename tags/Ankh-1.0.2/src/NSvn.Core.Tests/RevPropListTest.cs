// $Id$
using System;
using NUnit.Framework;
using NSvn.Common;


namespace NSvn.Core.Tests
{
    /// <summary>
    /// Tests Client::RevPropList
    /// </summary>
    public class RevPropListTest : TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.ExtractRepos();
        }

        /// <summary>
        /// Sets two properties on a repos and tries to retrieve them with Client::RevPropList
        /// </summary>
        [Test]
        public void TestBasic()
        {
            this.RunCommand( "svn", "ps --revprop -r HEAD foo bar " + this.ReposUrl );
            this.RunCommand( "svn", "ps --revprop -r HEAD kung foo " + this.ReposUrl );

            int headRev = int.Parse( this.RunCommand( "svnlook", "youngest " + this.ReposPath ) );

            int rev;
            PropertyDictionary dict = this.Client.RevPropList( this.ReposUrl, Revision.Head, 
                out rev );

            Assert.AreEqual( headRev, rev, "Revision wrong" );
            Assert.AreEqual( "bar", dict["foo"].ToString(), "Wrong property value" );
            Assert.AreEqual( "foo", dict["kung"].ToString(), "Wrong property value" );
        }
    }
}
