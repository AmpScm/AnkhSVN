using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc
{
	public interface ISvnLogItem
	{
		DateTime CommitDate { get; }
		string Author { get; }
		string LogMessage { get; }
		long Revision { get; }
	}
}
