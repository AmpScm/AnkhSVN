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
 
            PropertyDictionary mapping = Client.PropGet( "foo", path, Revision.Working, 
                false, new ClientContext() );
            Assertion.AssertEquals( "No entries returned", 1, mapping.Count );
           
            foreach( string key in mapping.Keys )
            {
                Assertion.AssertEquals( "No entry found", "foo", mapping[key].Name);
                Assertion.AssertEquals( "No entry found", "bar", mapping[key].GetString() );
            }            
        }
	}
}
