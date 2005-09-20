// $Id$
using System;
using NUnit.Framework;
using System.IO;
using NSvn.Common;

namespace NSvn.Core.Tests
{
    /// <summary>
    /// Summary description for PropGetTest.
    /// </summary>
    [TestFixture]
    public class PropGetTest : TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.ExtractWorkingCopy();
        }

        [Test]
        public void TestPropGetOnFile()
        {
            string path = Path.Combine( this.WcPath, "Form.cs" );
            this.RunCommand( "svn", "ps foo bar " + path );
 
            PropertyDictionary mapping = this.Client.PropGet( "foo", path, Revision.Working, 
                false );
            Assert.AreEqual( 1, mapping.Count, "No entries returned" );
           
            foreach( string key in mapping.Keys )
            {
                Assert.AreEqual( "foo", mapping[key].Name, "No entry found" );
                Assert.AreEqual( "bar", mapping[key].GetString(), "No entry found" );
            }            
        }
    }
}
