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

namespace Ankh.Scc
{
    [Flags]
    internal enum GlyphTipIndicators
    {
        None = 0,
        Modified = 1,
        Conflict = 2,
        FileObstructed = 4,
        DirectoryObstructed = 8,
        DoesNotExist = 16,
        Locked = 32
    }

    internal static class GlyphTipLogic
    {
        public static GlyphTipIndicators GetIndicators(
            bool isSubItem,
            bool isModified,
            bool isConflicted,
            bool isObstructed,
            bool isFile,
            bool exists,
            bool isVersioned,
            bool isDeleteScheduled,
            bool isLocked)
        {
            GlyphTipIndicators result = GlyphTipIndicators.None;

            if (isSubItem && isModified)
                result |= GlyphTipIndicators.Modified;

            if (isConflicted)
                result |= GlyphTipIndicators.Conflict;

            if (isObstructed)
            {
                result |= isFile
                    ? GlyphTipIndicators.FileObstructed
                    : GlyphTipIndicators.DirectoryObstructed;
            }

            if (!exists && isVersioned && !isDeleteScheduled)
                result |= GlyphTipIndicators.DoesNotExist;

            if (isLocked)
                result |= GlyphTipIndicators.Locked;

            return result;
        }
    }
}
