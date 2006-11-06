// $Id$
using System;
using System.IO;
using NUnit.Framework;
using Utils;
using TestUtils;
using NSvn.Common;

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
            this.RenameAdminDirs( this.wc2 );
        }

        public override void TearDown()
        {
            base.TearDown();
            PathUtils.RecursiveDelete( this.wc2 );
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
            this.Client.Update( this.WcPath, Revision.Head, Recurse.Full );

            Assert.IsTrue( File.Exists( filePath ), "File not restored after update" );
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

            this.Client.Update( this.WcPath, Revision.Head, Recurse.Full );

            string s;
            using( StreamReader r = new StreamReader( Path.Combine( this.WcPath, "Form.cs" ) ) )
                s = r.ReadToEnd();

            Assert.AreEqual( "Moo", s, "File not updated" );
        }

        [Test]
        public void TestUpdateMultipleFiles()
        {
            using( StreamWriter w = new StreamWriter( Path.Combine( this.wc2, "Form.cs" ) ) )
                w.Write( "Moo" );
            using( StreamWriter w = new StreamWriter( Path.Combine( this.wc2, "AssemblyInfo.cs" ) ) )
                w.Write( "Moo" );
            this.RunCommand( "svn", "ci -m \"\" " + this.wc2 );

            int[] revs = this.Client.Update( new string[]{ 
                                                             Path.Combine( this.WcPath, "Form.cs" ),
                                                             Path.Combine( this.WcPath, "AssemblyInfo.cs" )
                                                         }, Revision.Head, Recurse.None, true );
            Assert.AreEqual( 2, revs.Length );

            string s;
            using( StreamReader r = new StreamReader( Path.Combine( this.WcPath, "Form.cs" ) ) )
                s = r.ReadToEnd();
            Assert.AreEqual( "Moo", s, "File not updated" );

            using( StreamReader r = new StreamReader( Path.Combine( this.WcPath, "AssemblyInfo.cs" ) ) )
                s = r.ReadToEnd();
            Assert.AreEqual( "Moo", s, "File not updated" );
        }

        private string wc2;
    }
}
