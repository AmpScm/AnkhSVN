// Copyright 2026 The AnkhSVN Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;

using Ankh.Scc;
using SharpSvn;

namespace Ankh.VS.SolutionExplorer
{
    internal sealed class StatusImageInfo
    {
        public StatusImageInfo(
            bool isConflicted,
            bool isObstructed,
            bool isTreeConflicted,
            bool isReadOnlyMustLock,
            bool isVersioned,
            bool exists,
            bool isIgnored,
            bool isVersionable,
            bool inSolution,
            bool isSccExcluded,
            SvnStatus combinedStatus,
            bool isDocumentDirty,
            bool isLocked,
            bool isCopied,
            bool isCasingConflicted)
        {
            IsConflicted = isConflicted;
            IsObstructed = isObstructed;
            IsTreeConflicted = isTreeConflicted;
            IsReadOnlyMustLock = isReadOnlyMustLock;
            IsVersioned = isVersioned;
            Exists = exists;
            IsIgnored = isIgnored;
            IsVersionable = isVersionable;
            InSolution = inSolution;
            IsSccExcluded = isSccExcluded;
            CombinedStatus = combinedStatus;
            IsDocumentDirty = isDocumentDirty;
            IsLocked = isLocked;
            IsCopied = isCopied;
            IsCasingConflicted = isCasingConflicted;
        }

        public bool IsConflicted { get; private set; }
        public bool IsObstructed { get; private set; }
        public bool IsTreeConflicted { get; private set; }
        public bool IsReadOnlyMustLock { get; private set; }
        public bool IsVersioned { get; private set; }
        public bool Exists { get; private set; }
        public bool IsIgnored { get; private set; }
        public bool IsVersionable { get; private set; }
        public bool InSolution { get; private set; }
        public bool IsSccExcluded { get; private set; }
        public SvnStatus CombinedStatus { get; private set; }
        public bool IsDocumentDirty { get; private set; }
        public bool IsLocked { get; private set; }
        public bool IsCopied { get; private set; }
        public bool IsCasingConflicted { get; private set; }
    }

    internal static class StatusImageMapperLogic
    {
        public static AnkhGlyph GetGlyph(StatusImageInfo item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (item.IsConflicted || item.IsObstructed || item.IsTreeConflicted)
                return AnkhGlyph.InConflict;
            else if (item.IsReadOnlyMustLock)
                return AnkhGlyph.MustLock;
            else if (!item.IsVersioned)
            {
                if (!item.Exists)
                    return AnkhGlyph.FileMissing;
                else if (item.IsIgnored)
                    return AnkhGlyph.Ignored;
                else if (item.IsVersionable)
                {
                    if (item.InSolution)
                        return item.IsSccExcluded ? AnkhGlyph.Ignored : AnkhGlyph.ShouldBeAdded;
                    else
                        return AnkhGlyph.None;
                }
                else
                    return AnkhGlyph.None;
            }

            switch (item.CombinedStatus)
            {
                case SvnStatus.Normal:
                    if (item.IsDocumentDirty)
                        return AnkhGlyph.FileDirty;
                    else if (item.IsLocked)
                        return AnkhGlyph.LockedNormal;
                    else
                        return AnkhGlyph.Normal;

                case SvnStatus.Modified:
                    return item.IsLocked ? AnkhGlyph.LockedModified : AnkhGlyph.Modified;

                case SvnStatus.Replaced:
                    return AnkhGlyph.CopiedOrMoved;

                case SvnStatus.Added:
                    return item.IsCopied ? AnkhGlyph.CopiedOrMoved : AnkhGlyph.Added;

                case SvnStatus.Missing:
                    return item.IsCasingConflicted ? AnkhGlyph.InConflict : AnkhGlyph.Deleted;

                case SvnStatus.Deleted:
                    if (item.Exists && item.InSolution)
                        return item.IsSccExcluded ? AnkhGlyph.Ignored : AnkhGlyph.ShouldBeAdded;
                    return AnkhGlyph.Deleted;

                case SvnStatus.Conflicted:
                case SvnStatus.Obstructed:
                case SvnStatus.External:
                case SvnStatus.Incomplete:
                    return AnkhGlyph.InConflict;

                case SvnStatus.Ignored:
                    return AnkhGlyph.Ignored;

                case SvnStatus.Zero:
                default:
                    return AnkhGlyph.None;
            }
        }
    }
}
