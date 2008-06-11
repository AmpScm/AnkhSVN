using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc
{
    public enum PendingChangeKind
    {
        /// <summary>
        /// Nothing modified
        /// </summary>
        None,
        /// <summary>
        /// The file is new and should be added to Scc
        /// </summary>
        New,
        /// <summary>
        /// The item is modified
        /// </summary>
        Modified,
        /// <summary>
        /// A property on the item is modified
        /// </summary>
        PropertyModified,
        /// <summary>
        /// The item is replaced
        /// </summary>
        Replaced,
        /// <summary>
        /// The item was copied from another location
        /// </summary>
        Copied,
        /// <summary>
        /// The item was added to scc
        /// </summary>
        Added,
        /// <summary>
        /// The item was deleted
        /// </summary>
        Deleted,
        /// <summary>
        /// The item was deleted on disk, but not in scc
        /// </summary>
        Missing,
        /// <summary>
        /// The item is in conflict. The conflict must be resolved before continuing
        /// </summary>
        Conflicted,
        /// <summary>
        /// The properties of the node are in conflict
        /// </summary>
        PropertyConflicted,
        /// <summary>
        /// The item is obstructed by an item of the wrong kind
        /// </summary>
        Obstructed,
        /// <summary>
        /// The item is unmodified but open and dirty in an editor
        /// </summary>
        EditorDirty,

        /// <summary>
        /// The item is in incomplete state
        /// </summary>
        Incomplete,
    }
}
