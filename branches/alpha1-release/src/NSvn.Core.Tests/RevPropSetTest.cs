// $Id$
using System;
using NUnit.Framework;
using NSvn.Common;
using System.Text;

/// <summary>
/// Tests the Client::RevPropSet	
/// </summary>

namespace NSvn.Core.Tests
{
    [TestFixture]	
    public class RevPropSetTest : TestBase
	
    {

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.ExtractWorkingCopy();
            this.ExtractRepos();
        }

        /// <summary>
        ///Attempts to Set Properties on a file in the repository represented by url. 
        /// </summary>
        [Test]
        public void TestRevSetPropDir()
        {  
            ClientContext ctx = new ClientContext( );  
            byte[] propval = Encoding.UTF8.GetBytes ( "moo" );
            int rev;

            Client.RevPropSet( new Property( "cow", propval ), this.ReposUrl, 
                Revision.Head, out rev, ctx );
            Assertion.AssertEquals( "Couldn't set prop on selected Repos!", 
            	"moo", this.RunCommand( "svn", "propget cow --revprop -r head " + this.ReposUrl).Trim() );
        }
    }
}
