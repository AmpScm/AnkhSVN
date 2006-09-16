using System;
using System.Collections.Generic;
using System.Text;
using DotNetMock.Dynamic;
using EnvDTE;

namespace Ankh.Tests.MockObjects
{
	public class ProjectItemsMock : DynamicMock<ProjectItems>
	{
		public ProjectItemsMock(ProjectItemMock parent, params ProjectItemMock[] children)
		{
			SetValue("Count", children.Length);
			SetValue("GetEnumerator", new WrappedEnumerator(children,
				delegate(object current) { return ((ProjectItemMock)current).Object; }));

		}
	}
}
