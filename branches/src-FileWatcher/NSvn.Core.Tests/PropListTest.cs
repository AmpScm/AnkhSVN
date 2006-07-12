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
            base.SetUp();

            this.ExtractWorkingCopy();
        }

        [Test]
        public void TestBasic()
        {
            this.RunCommand( "svn", "ps foo bar " + this.WcPath );
            this.RunCommand( "svn", "ps kung foo " + this.WcPath );

            PropListItem[] items = this.Client.PropList( this.WcPath, Revision.Working,
                Recurse.None );

            Assert.AreEqual( 1, items.Length, 
                "Wrong number of proplist items" );
            Assert.AreEqual( 2, items[0].Properties.Count,
                "Wrong number of properties" );
            Assert.AreEqual( "bar", items[0].Properties["foo"].ToString(),
                "Wrong property" );
            Assert.AreEqual( "foo", items[0].Properties["kung"].ToString(),
                "Wrong property" );
        }
    }
}
