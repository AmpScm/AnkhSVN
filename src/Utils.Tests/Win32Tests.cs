using System;
using System.Text;
using NUnit.Framework;
using Utils;

namespace Utils.Tests
{
	/// <summary>
	/// Summary description for Win32Tests.
	/// </summary>
	[TestFixture]
	public class Win32Tests
	{
        [Test]
        public void TestPathRelativePathTo()
        {
            string path1 = @"C:\foo";
            string path2 = @"C:\foo\bar\Foo.cs";

            string result = Utils.Win32.Win32.PathRelativePathTo( 
                path1, Utils.Win32.FileAttribute.Directory, 
                path2, Utils.Win32.FileAttribute.Normal );

            Assert.AreEqual( @".\bar\Foo.cs", result );
        }

        [Test]
        public void TestPathRelativePathToFail()
        {
            string path1 = @"D:\foo";
            string path2 = @"C:\blah";
            string result = Utils.Win32.Win32.PathRelativePathTo(
                path1, Utils.Win32.FileAttribute.Directory, 
                path2, Utils.Win32.FileAttribute.Normal );
            Assert.IsNull( result );
        }
	}
}
