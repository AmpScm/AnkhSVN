using System;
using System.Text;
using System.IO;
using NUnit.Framework;

namespace Utils.Tests
{
    [TestFixture]
    public class HashUtilsTest
    {
        [SetUp]
        public void SetUp()
        {
            this.file = Path.GetTempFileName();
            using ( StreamWriter writer = new StreamWriter( file ) )
            {
                writer.WriteLine( "Hi world" );
            }

        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void GetMd5Hash()
        {
            byte[] hash = HashUtils.GetMd5HashForFile( this.file );
            Assert.IsNotNull( hash );
            Assert.AreEqual( 16, hash.Length );
        }

        [Test]
        public void HashesAreEqual()
        {
            byte[] hash1 = HashUtils.GetMd5HashForFile( this.file );
            byte[] hash2 = HashUtils.GetMd5HashForFile( this.file );

            Assert.IsTrue( HashUtils.HashesAreEqual( hash1, hash2 ) );

        }

        [Test]
        public void HashesAreNotEqual()
        {
            byte[] hash1 = HashUtils.GetMd5HashForFile( this.file );
            byte[] hash2 = new byte[ 16 ];

            Assert.IsFalse( HashUtils.HashesAreEqual( hash1, hash2 ) );
        }

        [Test]
        public void HashesWithDifferentLengthAreNotEqual()
        {
            byte[] hash1 = HashUtils.GetMd5HashForFile( this.file );
            byte[] hash2 = new byte[ 14 ];
            Array.Copy( hash1, hash2, 14 );

            Assert.IsFalse( HashUtils.HashesAreEqual( hash1, hash2 ) );
        }

        [Test]
        public void FileMatchesMd5Hash()
        {
            byte[] hash1 = HashUtils.GetMd5HashForFile( this.file );
            Assert.IsTrue( HashUtils.FileMatchesMd5Hash( hash1, this.file ) );
        }

        [Test]
        public void FileDoesNotMatchMd5Hash()
        {
            byte[] hash1 = HashUtils.GetMd5HashForFile( this.file );
            string file2 = Path.GetTempFileName();
            using ( StreamWriter writer = new StreamWriter( file2 ) )
            {
                writer.WriteLine( "Blah" );
            }

            Assert.IsFalse( HashUtils.FileMatchesMd5Hash( hash1, file2 ) );
        }

        [Test]
        public void GetSh1Hash()
        {
            byte[] hash = HashUtils.GetSha1HashForFile( this.file );
            Assert.IsNotNull( hash );
            Assert.IsTrue( hash.Length > 0 );
        }

        [Test]
        public void FileMatchesSha1Hash()
        {
            byte[] hash1 = HashUtils.GetSha1HashForFile( this.file );
            Assert.IsTrue( HashUtils.FileMatchesSha1Hash( hash1, this.file ) );
        }

        [Test]
        public void FileDoesNotMatchSha1Hash()
        {
            byte[] hash1 = HashUtils.GetSha1HashForFile( this.file );
            string file2 = Path.GetTempFileName();
            using ( StreamWriter writer = new StreamWriter( file2 ) )
            {
                writer.WriteLine( "Blah" );
            }

            Assert.IsFalse( HashUtils.FileMatchesMd5Hash( hash1, file2 ) );
        }


        private string file;
    }
}
