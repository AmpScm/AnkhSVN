// $Id$
using System;
using System.IO;
using NUnit.Framework;
using Utils;

namespace NSvn.Core.Tests
{
	/// <summary>
	/// Tests Client::Update
	/// </summary>
    [TestFixture]
    public class UpdateTest : TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.ExtractRepos();
            this.ExtractWorkingCopy();
            this.wc2 = this.FindDirName( Path.Combine( TestBase.BASEPATH, TestBase.WC_NAME ) );
            Zip.ExtractZipResource( this.wc2, this.GetType(), this.WC_FILE );
        }

        public override void TearDown()
        {
            base.TearDown();
            this.RecursiveDelete( this.wc2 );
        }

        /// <summary>
        /// Deletes a file, then calls update on the working copy to restore it 
        /// from the text-base
        /// </summary>
        [Test]
        public void TestDeletedFile()
        {
            string filePath = Path.Combine( this.WcPath, "Form.cs" ); 
            File.Delete( filePath );
            ClientContext ctx = new ClientContext( new NotifyCallback( this.NotifyCallback ) );
            Client.Update( this.WcPath, Revision.Head, true, ctx );

            Assertion.Assert( "File not restored after update", File.Exists( filePath ) );
        }

        /// <summary>
        /// Changes a file in a secondary working copy and commits. Updates the 
        /// primary wc and compares
        /// </summary>
        [Test]
        public void TestChangedFile()
        {
            using( StreamWriter w = new StreamWriter( Path.Combine( this.wc2, "Form.cs" ) ) )
                w.Write( "Moo" );
            this.RunCommand( "svn", "ci -m \"\" " + this.wc2 );

            ClientContext ctx = new ClientContext( new NotifyCallback( this.NotifyCallback ) );
            Client.Update( this.WcPath, Revision.Head, true, ctx );

            string s;
            using( StreamReader r = new StreamReader( Path.Combine( this.WcPath, "Form.cs" ) ) )
                s = r.ReadToEnd();

            Assertion.AssertEquals( "File not updated", "Moo", s );
        }

        private string wc2;
	}
}
