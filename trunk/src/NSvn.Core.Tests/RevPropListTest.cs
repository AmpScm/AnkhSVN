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
            PropertyDictionary dict = Client.RevPropList( this.ReposUrl, Revision.Head, out rev, 
                new ClientContext() );

            Assertion.AssertEquals( "Revision wrong", headRev, rev );
            Assertion.AssertEquals( "Wrong property value", "bar", dict["foo"].ToString() );
            Assertion.AssertEquals( "Wrong property value", "foo", dict["kung"].ToString() );
        }
	}
}
