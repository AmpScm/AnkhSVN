using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using DotNetMock.Dynamic;

namespace Ankh.Tests.MockObjects
{
	public class DTEMock :DynamicMock<DTE>
	{
		public DTEMock(string version)
		{
			SetValue("Version", version);
		}
	}
}
