using System;
using NUnit.Framework;
using System.IO;

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
            this.ExtractZipFile( this.path, "NSvn.Core.Tests.conflictwc.zip" );
        }

        /// <summary>
        ///Attempts to resolve conflict. 
        /// </summary>
        [Test]
        public void TestResolve()
        {  
            string filePath = Path.Combine( this.path, "Form.cs" );

            ClientContext ctx = new ClientContext( new NotifyCallback ( this.NotifyCallback) );
            Client.Resolve( filePath, false, ctx );
 
            Assertion.AssertEquals(" Resolve didn't work!", 'M', this.GetSvnStatus( filePath ) );

        }   
    
        private string path;
    }

}
