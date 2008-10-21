using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using SharpSvn.Implementation;

namespace Ankh.Scc
{
	public interface ISvnLogItem
	{
		DateTime CommitDate { get; }
		string Author { get; }
		string LogMessage { get; }
		long Revision { get; }
        SvnChangeItemCollection ChangedPaths { get; }

        Uri RepositoryRoot { get; }
	}
}
