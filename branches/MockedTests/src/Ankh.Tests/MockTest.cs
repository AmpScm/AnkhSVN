using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using EnvDTE;
using Ankh.Tests.MockObjects;

namespace Ankh.Tests
{
	[TestFixture]
	public class MockTest
	{
		[SetUp]
		public void SetUp()
		{
			dte = new DTEMock("8.0");
			context = new AnkhContext(dte.Object, null, null);
		}

		[Test]
		public void SomeTest()
		{
			
		}

		private IContext context;
		private DTEMock dte;
	}
}
