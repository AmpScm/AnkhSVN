// $Id$
using System;
using NUnit.Framework;
using System.IO;
using NSvn.Common;
using System.Text;

namespace NSvn.Core.Tests
{
	/// <summary>
	/// Tests Client::PropList
	/// </summary>
	[TestFixture]
	public class PropListTest : TestBase
	{
        [SetUp]
        public override void SetUp()
        {
            this.ExtractWorkingCopy();
        }

        [Test]
        public void TestBasic()
        {
            this.RunCommand( "svn", "ps foo bar " + this.WcPath );
            this.RunCommand( "svn", "ps kung foo " + this.WcPath );

            PropListItem[] items = Client.PropList( this.WcPath, Revision.Working,
                false, new ClientContext() );

            Assertion.AssertEquals( "Wrong number of proplist items", 1, items.Length );
            Assertion.AssertEquals( "Wrong number of properties", 2, 
                items[0].Properties.Count );
            Assertion.AssertEquals( "Wrong property", "bar", 
                items[0].Properties["foo"].ToString() );
            Assertion.AssertEquals( "Wrong property", "foo", 
                items[0].Properties["kung"].ToString() );
        }
	}
}
