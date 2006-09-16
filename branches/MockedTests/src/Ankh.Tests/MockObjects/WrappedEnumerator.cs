using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;

namespace Ankh.Tests.MockObjects
{
	public delegate object CurrentItemHandler(object currentItem);
	public class WrappedEnumerator : IEnumerator
	{
		IEnumerator baseEnumerator;
		CurrentItemHandler handler;
		public WrappedEnumerator(IEnumerable baseEnumerable, CurrentItemHandler handler)
			: this(baseEnumerable.GetEnumerator(), handler)
		{
		}
		public WrappedEnumerator(IEnumerator baseEnumerator, CurrentItemHandler handler)
		{
			this.baseEnumerator = baseEnumerator;
			this.handler = handler;
		}
		public object Current
		{
			get { return handler.Invoke(baseEnumerator.Current); }
		}

		public bool MoveNext()
		{
			return baseEnumerator.MoveNext();
		}

		public void Reset()
		{
			baseEnumerator.Reset();
		}
	}
}
