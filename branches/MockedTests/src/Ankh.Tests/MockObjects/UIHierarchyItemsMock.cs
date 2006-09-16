using System;
using System.Collections.Generic;
using System.Text;
using DotNetMock.Dynamic;
using EnvDTE;

namespace Ankh.Tests.MockObjects
{
	public class UIHierarchyItemsMock : DynamicMock<UIHierarchyItems>
	{
		public UIHierarchyItemsMock(UIHierarchyItemMock parent, params UIHierarchyItemMock[] children)
		{
			SetValue("Count", children == null ? 0 : children.Length);
			SetValue("GetEnumerator", new WrappedEnumerator(children.GetEnumerator(), delegate(object current)
			{
				return ((UIHierarchyItemMock)current).Object;
			}));
			SetValue("Parent", parent.Object);
			Handle("Item", delegate(object[] args) { return children[(int)args[0]]; });
			// Not set: DTE, Expanded

			parent.SetValue("Collection", Object);

		}
	}
}
