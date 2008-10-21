using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;

namespace Ankh.Scc
{
	public interface ISvnLogChangedPathItem
	{
		SvnChangeAction Action { get; }
		string CopyFromPath { get; }
		long CopyFromRevision { get; }
		string Path { get; }
        long Revision { get; }

        SvnOrigin Origin { get; }
	}
}
