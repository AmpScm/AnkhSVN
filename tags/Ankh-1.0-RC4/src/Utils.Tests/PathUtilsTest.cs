using System;
using NUnit.Framework;
using Utils;

namespace Utils.Tests
{
    /// <summary>
    /// Tests for the Utils.PathUtils class
    /// </summary>
    [TestFixture]
    public class PathUtilsTest
    {
        /// <summary>
        /// Tests the GetParent method.
        /// </summary>
        [Test]
        public void TestGetParent()
        {
            string dir = @"C:\parent\foo";
            Assert.AreEqual( @"C:\parent", PathUtils.GetParent( dir ) );

            string file = @"\foo\bar\file.cs";
            Assert.AreEqual( @"\foo\bar", PathUtils.GetParent( file ) );
        }

        /// <summary>
        /// Tests the GetName method
        /// </summary>
        [Test]
        public void TestGetName()
        {
            string dir = @"C:\parent\foo";
            Assert.AreEqual( @"foo", PathUtils.GetName( dir ) );

            string file = @"\foo\bar\file.cs";
            Assert.AreEqual( @"file.cs", PathUtils.GetName( file ) );
        }
        
        /// <summary>
        /// Tests the StripTrailingSlash method.
        /// </summary>
        [Test]
        public void TestStripTrailingSlash()
        {
            string path = @"C:\parent\foo\";
            Assert.AreEqual( @"C:\parent\foo", PathUtils.StripTrailingSlash(path) );
            
            path = @"C:\parent\foo";
            Assert.AreEqual( @"C:\parent\foo", PathUtils.StripTrailingSlash(path) );
        }

        [Test]
        public void IsSubPathOf()
        {
            Assert.IsTrue(PathUtils.IsSubPathOf(@"C:\foo\bar", @"C:\foo"));
        }

        [Test]
        public void IsSubPathOfBarbara()
        {
            Assert.IsFalse( PathUtils.IsSubPathOf( @"C:\foo\barbara\foo.txt", @"C:\foo\bar" ) );
        }
    }
}
