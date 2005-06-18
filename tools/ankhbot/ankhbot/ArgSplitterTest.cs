using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace AnkhBot
{
	[TestFixture]
	class ArgSplitterTest
	{
		[Test]
		public void TestBasic()
		{
			ArgSplitter splitter = new ArgSplitter( "Hello world" );
			List<string> arr = new List<string>( splitter );
			Assert.AreEqual( "Hello", arr[0] );
			Assert.AreEqual( "world", arr[1] );
		}

		[Test]
		public void BasicQuotes()
		{
			ArgSplitter splitter = new ArgSplitter( "Hi \"World\"" );
			List<string> arr = new List<string>( splitter );
			Assert.AreEqual( "Hi", arr[0] );
			Assert.AreEqual( "World", arr[1] );
		}

		[Test]
		public void QuotesWithSpace()
		{
			ArgSplitter splitter = new ArgSplitter( "Hi \"wonderful world\" there" );
			List<string> arr = new List<string>( splitter );
			Assert.AreEqual( "Hi", arr[0] );
			Assert.AreEqual( "wonderful world", arr[1] );
			Assert.AreEqual( "there", arr[2] );

		}
	}
}
