using System;
using System.Collections.Generic;
using System.Text;
using DotNetMock.Dynamic;
using EnvDTE;

namespace Ankh.Tests.MockObjects
{
	public class ProjectItemMock : DynamicMock<ProjectItem>
	{
		public ProjectItemMock(string name)
		{
			base.SetValue("Name", name);
		}

		public ProjectItemsMock SetChildren(params ProjectItemMock[] children)
		{
			return new ProjectItemsMock(this, children);
		}
	}
}
