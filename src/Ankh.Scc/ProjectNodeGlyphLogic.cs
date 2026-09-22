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

namespace Ankh.Scc
{
    internal static class ProjectNodeGlyphLogic
    {
        public static AnkhGlyph GetGlyph(
            AnkhGlyph currentGlyph,
            bool exists,
            bool isVersioned,
            bool isIgnored,
            bool isVersionable,
            bool isSccExcluded)
        {
            if (currentGlyph != AnkhGlyph.None)
                return currentGlyph;

            if (!exists || isVersioned || isIgnored || !isVersionable)
                return currentGlyph;

            return isSccExcluded
                ? AnkhGlyph.Ignored
                : AnkhGlyph.ShouldBeAdded;
        }
    }
}
