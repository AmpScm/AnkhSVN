using System;
using NUnit.Framework;
using System.IO;
using System.Collections;
using System.Xml.Serialization;

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
            Assert.AreEqual( "Base", revision.ToString() );

            revision = Revision.Committed;
            Assert.AreEqual( "Committed", revision.ToString() );

            revision = Revision.Head;
            Assert.AreEqual( "Head", revision.ToString() );

            revision = Revision.Previous;
            Assert.AreEqual( "Previous", revision.ToString() );

            revision = Revision.Unspecified;
            Assert.AreEqual( "Unspecified", revision.ToString() );

            revision = Revision.Working;
            Assert.AreEqual( "Working", revision.ToString() );

            DateTime t = DateTime.Now;
            revision = Revision.FromDate( t );
            Assert.AreEqual( t.ToString(), revision.ToString() );

            revision = Revision.FromNumber( 42 );
            Assert.AreEqual( "42", revision.ToString() );
        }

        /// <summary>
        /// Test the parse method.
        /// </summary>
        [Test]
        public void TestParse()
        {
            this.DoTestParse( "working", Revision.Working );
            this.DoTestParse( "unspecified", Revision.Unspecified );
            this.DoTestParse( "head", Revision.Head );
            this.DoTestParse( "committed", Revision.Committed );
            this.DoTestParse( "base", Revision.Base );
            this.DoTestParse( "previous", Revision.Previous );

            Assert.AreEqual( "42", Revision.Parse("42").ToString() );

            DateTime t = DateTime.Now;
            Assert.AreEqual( t.ToString(), Revision.Parse(t.ToString()).ToString() );

            // this should throw
            try
            {
                DateTime.Parse( "Foo" );
                Assert.Fail( "Foo is not a valid revision" );
            }
            catch( FormatException )
            {}
        }

        [Test]
        public void TestRevisionTypes()
        {
            Revision r = Revision.FromDate( DateTime.Now );
            Assert.AreEqual( RevisionType.Date, r.Type );

            r = Revision.FromNumber( 42 );
            Assert.AreEqual( RevisionType.Number, r.Type );

            Assert.AreEqual( RevisionType.Base, Revision.Base.Type );
            Assert.AreEqual( RevisionType.Commmitted, Revision.Committed.Type );
            Assert.AreEqual( RevisionType.Head, Revision.Head.Type );
            Assert.AreEqual( RevisionType.Previous, Revision.Previous.Type );
            Assert.AreEqual( RevisionType.Unspecified, Revision.Unspecified.Type );
            Assert.AreEqual( RevisionType.Working, Revision.Working.Type );
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestGetDateFromOtherTypeOfRevision()
        {
            DateTime dt = Revision.Head.Date;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestGetNumberFromOtherTypeOfRevision()
        {
            int number = Revision.Head.Number;
        }

        private void DoTestParse( string s, Revision rev )
        {
            Assert.AreEqual( Revision.Parse(s), rev );
        }
	}
}
