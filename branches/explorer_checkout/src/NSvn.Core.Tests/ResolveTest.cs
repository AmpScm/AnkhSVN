// $Id$
using System;
using NUnit.Framework;
using System.IO;
using Utils;

namespace NSvn.Core.Tests
{
    /// <summary>
    /// Tests Client::Resolve
    /// </summary>
    [TestFixture]
    public class ResolveTest : TestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.path = Path.GetTempFileName();
            File.Delete ( this.path );
            Zip.ExtractZipResource( this.path, this.GetType(), "NSvn.Core.Tests.conflictwc.zip" );
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            this.RecursiveDelete( this.path );
        }

        /// <summary>
        ///Attempts to resolve a conflicted file. 
        /// </summary>
        [Test]
        public void TestResolveFile()
        {  
            string filePath = Path.Combine( this.path, "Form.cs" );

            ClientContext ctx = new ClientContext( new NotifyCallback ( this.NotifyCallback) );
            Client.Resolve( filePath, false, ctx );
 
            Assertion.AssertEquals(" Resolve didn't work!", 'M', this.GetSvnStatus( filePath ) );

        }
   
        /// <summary>
        ///Attempts to resolve a conflicted directory recursively. 
        /// </summary>
        [Test]
        public void TestResolveDirectory()
        {  
            ClientContext ctx = new ClientContext( new NotifyCallback ( this.NotifyCallback) );
            Client.Resolve( this.path, true, ctx );
 
            Assertion.AssertEquals(" Resolve didn't work! Directory still conflicted", 'M', 
                this.GetSvnStatus( this.path ) );
            Assertion.AssertEquals( "Resolve didn't work! File still conflicted", 'M', 
                this.GetSvnStatus( Path.Combine( this.path, "Form.cs" ) ) ); 

        }
    
        private string path;
    }

}
