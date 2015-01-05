using System;
using System.Collections.Generic;

namespace Ankh.Scc.Git
{
    /// <summary>
    /// This flag enum specifies the full UI state of a <see cref="SvnItem"/> in all live updating windows
    /// </summary>
    /// <remarks>
    /// It is a superset of the <see cref="PendingChangeKind"/> and the <see cref="AnkhGlyph"/>. 
    /// Those two values can be 100% calculated from just this state
    /// </remarks>
    [Flags]
    public enum GitItemState
    {
        #region State Flags

        /// <summary>
        /// None; not calculated
        /// </summary>
        None = 0,

        /// <summary>
        /// File is on disk
        /// </summary>
        Exists = 0x00000001,

        /// <summary>
        /// File is in the current open solution
        /// </summary>
        InSolution = 0x00000002,

        /// <summary>
        /// Item is versioned or below a versioned directory
        /// </summary>
        Versionable = 0x00000020,

        /// <summary>
        /// Gets a boolean indicating whether the item is a file
        /// </summary>
        /// <remarks>Contains the on disk status.. If obstructed this does not match</remarks>
        IsDiskFile = 0x00000040,

        /// <summary>
        /// Gets a boolean indicating whether the item is a directory
        /// </summary>
        IsDiskFolder = 0x00000080,

        /// <summary>
        /// The node is read only on disk
        /// </summary>
        ReadOnly = 0x00000100,

        /// <summary>
        /// The file is marked as dirty by the editor that has the file open
        /// </summary>
        DocumentDirty = 0x00000200,

        /// <summary>
        /// 
        /// </summary>
        IsWCRoot = 0x00000400,

        /// <summary>
        /// Somehow modified in subversion
        /// </summary>
        SvnDirty = 0x00000800,

        IsAdministrativeArea = 0x00001000,
        #endregion


        Versioned = 0x00010000,
        Modified = 0x00020000,
        Added = 0x00040000,
        Deleted = 0x00080000,
        GitDirty = 0x00100000,
        Ignored = 0x00200000,
        Conflicted = 0x00400000

    }
}
