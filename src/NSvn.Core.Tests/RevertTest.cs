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
            using ( StreamReader reader = new StreamReader( filePath ))
                oldContents = reader.ReadToEnd();
            using ( StreamWriter writer = new StreamWriter (filePath ))
                writer.WriteLine( "mooooooo" );

            Client.Revert( filePath, false, ctx);

            string newContents;
            using ( StreamReader reader = new StreamReader( filePath ))
                newContents = reader.ReadToEnd();

            Assertion.AssertEquals( "File not reverted", oldContents, newContents );

        }   
		    
	}
}
