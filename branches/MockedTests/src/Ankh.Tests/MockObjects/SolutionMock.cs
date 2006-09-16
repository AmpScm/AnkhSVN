using System;
using System.Collections.Generic;
using System.Text;
using DotNetMock.Dynamic;

namespace Ankh.Tests.MockObjects
{
	class SolutionMock : DynamicMock<EnvDTE.Solution>
	{
		public SolutionMock()
		{
		}
	}
}
