using System;
using NUnit.Framework;
using NSvn.Common;
using System.Text;

/// <summary>
/// Tests the Client::RevPropGet	
/// </summary>

namespace NSvn.Core.Tests
{
    [TestFixture]	
    public class RevPropGetTest : TestBase
	
    {

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.ExtractWorkingCopy();
            this.ExtractRepos();
        }

        /// <summary>
        ///Attempts to Get Properties on a directory in the repository represented by url. 
        /// </summary>
        [Test]
        public void TestRevPropGetDir()
        {  
            ClientContext ctx = new ClientContext( );  
            int rev;

            int headRev = int.Parse( this.RunCommand( "svnlook", "youngest " + this.ReposPath ) );

            this.RunCommand( "svn", "ps --revprop -r HEAD cow moo " + this.ReposUrl );
 
            Property prop = Client.RevPropGet( "cow", this.ReposUrl, 
                Revision.Head, out rev, ctx );

            Assertion.AssertEquals( "Revision wrong", headRev, rev );
            Assertion.AssertEquals( "Wrong property value", "moo", prop.ToString());
        }
    }
}