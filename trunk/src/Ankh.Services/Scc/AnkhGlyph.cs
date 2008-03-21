using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc
{
    /// <summary>
    /// This enum specified the usage of glyph icons by ankh
    /// </summary>
    /// <remarks>This enum contains 16 members which should map the VsStateIcon class if possible</remarks>
    public enum AnkhGlyph
    {
        /// <summary>
        /// Not supported / No Icon (STATEICON_NOSTATEICON /0x0)
        /// </summary>
        None,

        /// <summary>
        /// Path is versioned and not changed (STATEICON_CHECKEDIN /0x1)
        /// </summary>
        MustLock,

        /// <summary>
        /// Path is versioned and modified (STATEICON_CHECKEDOUT /0x2)
        /// </summary>
        Modified,

        /// <summary>
        /// File is missing (STATEICON_ORPHANED /0x3)
        /// </summary>
        Deleted,

        /// <summary>
        /// Item is versioned and not modified (STATEICON_EDITABLE /0x4)
        /// </summary>
        Normal,

        /// <summary>
        /// Blank Icon / No Icon (STATEICON_BLANK /0x5)
        /// </summary>
        /// <remarks>This icon is applied to all tree items which don't retrieve scc status</remarks>
        Blank,

        /// <summary>
        /// File is readonly (STATEICON_READONLY /0x6)
        /// </summary>
        FileDirty,

        /// <summary>
        /// (File is disabled) (STATEICON_DISABLED /0x7)
        /// </summary>
        FileMissing,

        /// <summary>
        /// Locked locally (STATEICON_CHECKEDOUTEXCLUSIVE /0x8)
        /// </summary>
        LockedNormal,

        /// <summary>
        /// File must be locked before editting (STATEICON_CHECKEDOUTSHAREDOTHER /0x9)
        /// </summary>
        LockedModified,

        /// <summary>
        /// File is marked as ignore (STATEICON_CHECKEDOUTEXCLUSIVEOTHER /0xA)
        /// </summary>
        Ignored,

        /// <summary>
        /// File is marked as added (STATEICON_EXCLUDEDFROMSCC /0xB)
        /// </summary>
        Added,

        /// <summary>
        /// File is not managed yet, but is on the pending list (Last+1 /0xC)
        /// </summary>
        /// <remarks>Currently generated internally by the Scc provider</remarks>
        ShouldBeAdded,

        /// <summary>
        /// File is added but has history (Last+2 /0xD)
        /// </summary>
        AddedWithHistory,

        /// <summary>
        /// File is replaced by an other file with the same name (Last+3 /0xE)
        /// </summary>
        Replaced,

        /// <summary>
        /// File is in conflict; must be resolved before continuing (Last+4 /0xF)
        /// </summary>
        InConflict
    }
}
