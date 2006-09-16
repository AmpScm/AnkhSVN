using System;
using System.Collections.Generic;
using System.Text;
using DotNetMock.Dynamic;
using EnvDTE;

namespace Ankh.Tests.MockObjects
{
	public class UIHierarchyItemMock : DynamicMock<UIHierarchyItem>
	{
		public UIHierarchyItemMock(string name)
		{
			SetValue("Name", name);
		}

		public UIHierarchyItemsMock SetChildren(params UIHierarchyItemMock[] children)
		{
			return new UIHierarchyItemsMock(this, children);
		}
	}
}