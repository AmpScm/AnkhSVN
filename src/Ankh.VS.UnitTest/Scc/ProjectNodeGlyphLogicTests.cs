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

using Ankh.Scc;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Scc
{
    [TestFixture]
    public class ProjectNodeGlyphLogicTests
    {
        [TestCase(AnkhGlyph.Normal)]
        [TestCase(AnkhGlyph.MustLock)]
        [TestCase(AnkhGlyph.Modified)]
        [TestCase(AnkhGlyph.Added)]
        [TestCase(AnkhGlyph.InConflict)]
        public void GetGlyph_PreservesExistingStatusGlyphs(AnkhGlyph glyph)
        {
            Assert.That(
                ProjectNodeGlyphLogic.GetGlyph(
                    glyph,
                    true,
                    false,
                    false,
                    true,
                    false),
                Is.EqualTo(glyph));
        }

        [Test]
        public void GetGlyph_ShowsShouldBeAddedForProjectNodeMissingFromProjectMap()
        {
            Assert.That(
                ProjectNodeGlyphLogic.GetGlyph(
                    AnkhGlyph.None,
                    true,
                    false,
                    false,
                    true,
                    false),
                Is.EqualTo(AnkhGlyph.ShouldBeAdded));
        }

        [Test]
        public void GetGlyph_RespectsSccExclusion()
        {
            Assert.That(
                ProjectNodeGlyphLogic.GetGlyph(
                    AnkhGlyph.None,
                    true,
                    false,
                    false,
                    true,
                    true),
                Is.EqualTo(AnkhGlyph.Ignored));
        }

        [TestCase(false, false, false, true)]
        [TestCase(true, true, false, true)]
        [TestCase(true, false, true, true)]
        [TestCase(true, false, false, false)]
        public void GetGlyph_DoesNotInventStatusForNonCandidates(
            bool exists,
            bool isVersioned,
            bool isIgnored,
            bool isVersionable)
        {
            Assert.That(
                ProjectNodeGlyphLogic.GetGlyph(
                    AnkhGlyph.None,
                    exists,
                    isVersioned,
                    isIgnored,
                    isVersionable,
                    false),
                Is.EqualTo(AnkhGlyph.None));
        }
    }
}
