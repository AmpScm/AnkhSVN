// $Id$
using System;
using NUnit.Framework;
using System.IO;
using Utils;
using TestUtils;

using NSvn.Common;

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
            this.path = this.GetTempFile();
            Zip.ExtractZipResource( this.path, this.GetType(), "NSvn.Core.Tests.conflictwc.zip" );
            this.RenameAdminDirs( this.path );
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            PathUtils.RecursiveDelete( this.path );
        }

        /// <summary>
        ///Attempts to resolve a conflicted file. 
        /// </summary>
        [Test]
        public void TestResolveFile()
        {  
            string filePath = Path.Combine( this.path, "Form.cs" );

            this.Client.Resolved( filePath, Recurse.None );
 
            Assert.AreEqual('M', this.GetSvnStatus( filePath ), "Resolve didn't work!" );

        }
   
        /// <summary>
        ///Attempts to resolve a conflicted directory recursively. 
        /// </summary>
        [Test]
        public void TestResolveDirectory()
        {  
            this.Client.Resolved( this.path, Recurse.Full );
 
            Assert.AreEqual( 'M', this.GetSvnStatus( this.path ),
                " Resolve didn't work! Directory still conflicted" );
            Assert.AreEqual( 'M', this.GetSvnStatus( Path.Combine( this.path, "Form.cs" ) ),
                "Resolve didn't work! File still conflicted" ); 

        }
    
        private string path;
    }

}
