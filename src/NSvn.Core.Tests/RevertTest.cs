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
            ClientContext ctx = new ClientContext( new NotifyCallback( this.NotifyCallback ));

            string oldContents ;
            string newContents;
            this.ModifyFile( out oldContents, out newContents, filePath, filePath, false );


            Assertion.AssertEquals( "File not reverted", oldContents, newContents );

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

            Assertion.AssertEquals( "File not reverted", oldContents, newContents );

        }

        private void ModifyFile(out string oldContents, out string newContents, string filePath, 
            string revertPath, bool recursive )
        {            

            using ( StreamReader reader = new StreamReader( filePath ))
                oldContents = reader.ReadToEnd();
            using ( StreamWriter writer = new StreamWriter (filePath ))
                writer.WriteLine( "mooooooo" );

            ClientContext ctx = new ClientContext( new NotifyCallback( this.NotifyCallback ));
            Client.Revert( new string[]{revertPath}, recursive, ctx );

            using ( StreamReader reader = new StreamReader( filePath ))
                newContents = reader.ReadToEnd();
        } 
		    
    }
}
