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

using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc
{
    internal static class SccGlyphDisplayLogic
    {
        public static VsStateIcon GetStateIcon(
            AnkhGlyph glyph,
            int glyphOffset,
            bool preferVisualStudioNativeGlyphs,
            int glyphMonikerBaseIndex)
        {
            // IVsSccGlyphs2 appends our ImageMoniker list after Visual Studio's
            // built-in SCC glyphs. Once VS has requested that list, prefer it
            // for all visible Ankh states so each status gets the catalog icon
            // selected by SccGlyphMonikerLogic.
            if (glyphMonikerBaseIndex >= 0
                && glyph != AnkhGlyph.None
                && glyph != AnkhGlyph.Blank)
            {
                return (VsStateIcon)(glyphMonikerBaseIndex + (int)glyph);
            }

            if (preferVisualStudioNativeGlyphs)
            {
                VsStateIcon nativeIcon;
                if (TryGetNativeStateIcon(glyph, out nativeIcon))
                    return nativeIcon;
            }

            return GetCustomStateIcon(glyph, glyphOffset);
        }

        private static bool TryGetNativeStateIcon(
            AnkhGlyph glyph,
            out VsStateIcon icon)
        {
            // AnkhGlyph values 0-11 intentionally line up with Visual Studio's
            // built-in VsStateIcon values. Returning them directly lets Visual
            // Studio draw its own theme-aware source-control glyphs.
            if (glyph >= AnkhGlyph.None && glyph <= AnkhGlyph.Ignored)
            {
                icon = (VsStateIcon)glyph;
                return true;
            }

            // Visual Studio's legacy SCC glyph enum has no dedicated Added,
            // Conflict, or ChildChanged values. Use the closest visible native
            // state for modern Visual Studio rather than relying on the legacy
            // bitmap strip. The custom path below remains available as fallback.
            switch (glyph)
            {
                case AnkhGlyph.Added:
                case AnkhGlyph.ShouldBeAdded:
                case AnkhGlyph.ChildChanged:
                    icon = VsStateIcon.STATEICON_CHECKEDOUT;
                    return true;

                case AnkhGlyph.InConflict:
                    icon = VsStateIcon.STATEICON_ORPHANED;
                    return true;

                default:
                    icon = VsStateIcon.STATEICON_NOSTATEICON;
                    return false;
            }
        }

        private static VsStateIcon GetCustomStateIcon(
            AnkhGlyph glyph,
            int glyphOffset)
        {
            VsStateIcon icon = (VsStateIcon)glyph;

            if (icon == VsStateIcon.STATEICON_BLANK
                || icon == VsStateIcon.STATEICON_NOSTATEICON)
            {
                return icon;
            }

            return (VsStateIcon)((int)icon + glyphOffset);
        }
    }
}
