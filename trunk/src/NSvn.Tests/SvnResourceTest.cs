// $Id$
using System;
using System.IO;
using NUnit.Framework;

namespace NSvn.Tests
{
	/// <summary>
	/// Tests the SvnResource class
	/// </summary>
	[TestFixture]
	public class SvnResourceTest : TestBase
	{
        [SetUp]
        public override void SetUp()
        {
            this.ExtractWorkingCopy();
        }

        /// <summary>
        /// Tests the FromLocalPath factory method.
        /// </summary>
        [Test]
        public void TestFromLocalPath()
        {
            ILocalResource resDir = SvnResource.FromLocalPath( this.WcPath );
            Assertion.AssertEquals( "Wrong type resource", typeof(WorkingCopyDirectory), 
                resDir.GetType() );

            ILocalResource resFile = SvnResource.FromLocalPath( Path.Combine(
                this.WcPath, "Form.cs" ) );
            Assertion.AssertEquals( "Wrong type resource", typeof(WorkingCopyFile),
                resFile.GetType() );

            string mooFile = Path.Combine(this.WcPath,"Moo.moo");
            using( StreamWriter w = new StreamWriter( mooFile ) )
                w.WriteLine( "Moo" );

            ILocalResource unversionedFile = SvnResource.FromLocalPath( 
                mooFile );
            Assertion.AssertEquals( "Wrong type resource", typeof(UnversionedFile),
                unversionedFile.GetType() );

            ILocalResource totallyUnversionedFile = SvnResource.FromLocalPath(
                Path.GetTempFileName() );
            Assertion.AssertNull( "Expected a null resource from an unversioned path", 
                totallyUnversionedFile );
        }
	}
}
