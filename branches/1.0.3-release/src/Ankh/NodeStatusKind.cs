using System;
using System.Text;
using NSvn.Core;

namespace Ankh
{
    public enum NodeStatusKind
    {
        None = StatusKind.None,
        Normal = StatusKind.Normal,
        Added = StatusKind.Added,
        Deleted = StatusKind.Deleted,
        Conflicted = StatusKind.Conflicted,
        Unversioned = StatusKind.Unversioned,
        Modified = StatusKind.Modified,
        Ignored = StatusKind.Ignored,
        Replaced = StatusKind.Replaced,
        IndividualStatusesConflicting
    }

}
