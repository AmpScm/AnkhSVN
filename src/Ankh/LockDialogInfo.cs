using System;
using System.Collections;
using System.Collections.Generic;

namespace Ankh
{
	/// <summary>
	/// Summary description for LockDialogInfo.
	/// </summary>
	public class LockDialogInfo : PathSelectorInfo
	{
		public LockDialogInfo(ICollection<SvnItem> items, Predicate<SvnItem> checkedFilter) :
			base("", items, checkedFilter)
		{
			this.message = "";
			this.stealLocks = false;
		}

		public string Message
		{
			get { return this.message; }
			set { this.message = value; }
		}

		public bool StealLocks
		{
			get { return this.stealLocks; }
			set { this.stealLocks = value; }
		}

		private string message;
		private bool stealLocks;
	}
}
