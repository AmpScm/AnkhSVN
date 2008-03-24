using System;
using System.Text;

using SharpSvn;

namespace Ankh
{
    public enum NodeStatusKind
    {
        None = SvnStatus.None,
        Normal = SvnStatus.Normal,
        Added = SvnStatus.Added,
        Deleted = SvnStatus.Deleted,
        Conflicted = SvnStatus.Conflicted,
        Unversioned = SvnStatus.NotVersioned,
        Modified = SvnStatus.Modified,
        Ignored = SvnStatus.Ignored,
        Replaced = SvnStatus.Replaced,
        External = SvnStatus.External,
        Incomplete = SvnStatus.Incomplete,
        Merged = SvnStatus.Merged,
        Missing = SvnStatus.Missing,
        Obstructed = SvnStatus.Obstructed,
        IndividualStatusesConflicting
    }

}
