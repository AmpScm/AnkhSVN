using System;
using NUnit.Framework;

namespace NSvn.Core.Tests
{
	/// <summary>
	/// Tests for the Revision class
	/// </summary>
	[TestFixture]
	public class RevisionTest 
    {
        [Test]
        public void TestToString()
        {
            Revision revision = Revision.Base;
            Assertion.AssertEquals( "Base", revision.ToString() );

            revision = Revision.Committed;
            Assertion.AssertEquals( "Committed", revision.ToString() );

            revision = Revision.Head;
            Assertion.AssertEquals( "Head", revision.ToString() );

            revision = Revision.Previous;
            Assertion.AssertEquals( "Previous", revision.ToString() );

            revision = Revision.Unspecified;
            Assertion.AssertEquals( "Unspecified", revision.ToString() );

            revision = Revision.Working;
            Assertion.AssertEquals( "Working", revision.ToString() );

            DateTime t = DateTime.Now;
            revision = Revision.FromDate( t );
            Assertion.AssertEquals( t.ToString(), revision.ToString() );

            revision = Revision.FromNumber( 42 );
            Assertion.AssertEquals( "42", revision.ToString() );
        }
		
	}
}
