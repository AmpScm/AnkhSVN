// $Id$
using System;
using NUnit.Framework;
using System.IO;

namespace NSvn.Core.Tests
{
    /// <summary>
    /// Tests Client::Revert 
    /// </summary>
	
    [TestFixture]
    public class RevertTest : TestBase
    {

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.ExtractWorkingCopy();
        }

        /// <summary>
        ///Attempts to revert single file. 
        /// </summary>
        [Test]
        public void TestRevertFile()
        {
            string filePath = Path.Combine( this.WcPath, "Form.cs" );

            string oldContents ;
            string newContents;
            this.ModifyFile( out oldContents, out newContents, filePath, filePath, false );


            Assert.AreEqual( oldContents, newContents, "File not reverted" );

        }  
 
        /// <summary>
        ///Attempts to revert the whole working copy 
        /// </summary>
        [Test]
        public void TestRevertDirectory()
        {
            string oldContents;
            string newContents;
            this.ModifyFile( out oldContents, out newContents, Path.Combine( this.WcPath, "Form.cs" ), 
                this.WcPath, true );

            Assert.AreEqual( oldContents, newContents, "File not reverted" );

        }

        private void ModifyFile(out string oldContents, out string newContents, string filePath, 
            string revertPath, bool recursive )
        {            

            using ( StreamReader reader = new StreamReader( filePath ))
                oldContents = reader.ReadToEnd();
            using ( StreamWriter writer = new StreamWriter (filePath ))
                writer.WriteLine( "mooooooo" );

            this.Client.Revert( new string[]{revertPath}, recursive );

            using ( StreamReader reader = new StreamReader( filePath ))
                newContents = reader.ReadToEnd();
        } 
		    
    }
}
