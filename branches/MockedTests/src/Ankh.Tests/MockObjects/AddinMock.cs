using System;
using System.Collections.Generic;
using System.Text;
using DotNetMock.Dynamic;
using EnvDTE;
namespace Ankh.Tests.MockObjects
{
	public class AddinMock : DynamicMock<EnvDTE.AddIn>
	{
		public AddinMock()
		{
		}
	}
}
