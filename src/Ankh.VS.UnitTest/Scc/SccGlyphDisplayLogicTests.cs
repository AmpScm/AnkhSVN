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
using Microsoft.VisualStudio.Shell.Interop;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Scc
{
    [TestFixture]
    public class SccGlyphDisplayLogicTests
    {
        [TestCase(AnkhGlyph.MustLock, 13)]
        [TestCase(AnkhGlyph.Modified, 14)]
        [TestCase(AnkhGlyph.Deleted, 15)]
        [TestCase(AnkhGlyph.FileDirty, 16)]
        [TestCase(AnkhGlyph.Normal, 18)]
        [TestCase(AnkhGlyph.FileMissing, 19)]
        [TestCase(AnkhGlyph.CopiedOrMoved, 20)]
        [TestCase(AnkhGlyph.LockedNormal, 21)]
        [TestCase(AnkhGlyph.LockedModified, 22)]
        [TestCase(AnkhGlyph.Ignored, 23)]
        [TestCase(AnkhGlyph.Added, 24)]
        [TestCase(AnkhGlyph.ShouldBeAdded, 25)]
        [TestCase(AnkhGlyph.InConflict, 26)]
        [TestCase(AnkhGlyph.ChildChanged, 27)]
        public void GetStateIcon_UsesMonikerListWhenAvailable(
            AnkhGlyph glyph,
            int expected)
        {
            Assert.That(
                (int)SccGlyphDisplayLogic.GetStateIcon(
                    glyph,
                    16,
                    true,
                    12),
                Is.EqualTo(expected));
        }

        [Test]
        public void GetStateIcon_MonikerListPreservesNoIconAndBlank()
        {
            Assert.That(
                SccGlyphDisplayLogic.GetStateIcon(
                    AnkhGlyph.None,
                    16,
                    true,
                    12),
                Is.EqualTo(VsStateIcon.STATEICON_NOSTATEICON));

            Assert.That(
                SccGlyphDisplayLogic.GetStateIcon(
                    AnkhGlyph.Blank,
                    16,
                    true,
                    12),
                Is.EqualTo(VsStateIcon.STATEICON_BLANK));
        }

        [TestCase(AnkhGlyph.None, VsStateIcon.STATEICON_NOSTATEICON)]
        [TestCase(AnkhGlyph.MustLock, VsStateIcon.STATEICON_CHECKEDIN)]
        [TestCase(AnkhGlyph.Modified, VsStateIcon.STATEICON_CHECKEDOUT)]
        [TestCase(AnkhGlyph.Deleted, VsStateIcon.STATEICON_ORPHANED)]
        [TestCase(AnkhGlyph.FileDirty, VsStateIcon.STATEICON_EDITABLE)]
        [TestCase(AnkhGlyph.Blank, VsStateIcon.STATEICON_BLANK)]
        [TestCase(AnkhGlyph.Normal, VsStateIcon.STATEICON_READONLY)]
        [TestCase(AnkhGlyph.FileMissing, VsStateIcon.STATEICON_DISABLED)]
        [TestCase(AnkhGlyph.CopiedOrMoved, VsStateIcon.STATEICON_CHECKEDOUTEXCLUSIVE)]
        [TestCase(AnkhGlyph.LockedNormal, VsStateIcon.STATEICON_CHECKEDOUTSHAREDOTHER)]
        [TestCase(AnkhGlyph.LockedModified, VsStateIcon.STATEICON_CHECKEDOUTEXCLUSIVEOTHER)]
        [TestCase(AnkhGlyph.Ignored, VsStateIcon.STATEICON_EXCLUDEDFROMSCC)]
        public void GetStateIcon_UsesVisualStudioNativeFallbackForAlignedStates(
            AnkhGlyph glyph,
            VsStateIcon expected)
        {
            Assert.That(
                SccGlyphDisplayLogic.GetStateIcon(
                    glyph,
                    16,
                    true,
                    -1),
                Is.EqualTo(expected));
        }

        [TestCase(AnkhGlyph.Added, VsStateIcon.STATEICON_CHECKEDOUT)]
        [TestCase(AnkhGlyph.ShouldBeAdded, VsStateIcon.STATEICON_CHECKEDOUT)]
        [TestCase(AnkhGlyph.ChildChanged, VsStateIcon.STATEICON_CHECKEDOUT)]
        [TestCase(AnkhGlyph.InConflict, VsStateIcon.STATEICON_ORPHANED)]
        public void GetStateIcon_UsesClosestNativeFallbackForExtendedStates(
            AnkhGlyph glyph,
            VsStateIcon expected)
        {
            Assert.That(
                SccGlyphDisplayLogic.GetStateIcon(
                    glyph,
                    16,
                    true,
                    -1),
                Is.EqualTo(expected));
        }

        [TestCase(AnkhGlyph.Modified, 18)]
        [TestCase(AnkhGlyph.Normal, 22)]
        [TestCase(AnkhGlyph.Added, 28)]
        [TestCase(AnkhGlyph.InConflict, 30)]
        public void GetStateIcon_PreservesLegacyCustomGlyphFallback(
            AnkhGlyph glyph,
            int expected)
        {
            Assert.That(
                (int)SccGlyphDisplayLogic.GetStateIcon(
                    glyph,
                    16,
                    false,
                    -1),
                Is.EqualTo(expected));
        }

        [Test]
        public void GetStateIcon_LegacyFallbackLeavesBlankStatesUnshifted()
        {
            Assert.That(
                SccGlyphDisplayLogic.GetStateIcon(
                    AnkhGlyph.None,
                    16,
                    false,
                    -1),
                Is.EqualTo(VsStateIcon.STATEICON_NOSTATEICON));

            Assert.That(
                SccGlyphDisplayLogic.GetStateIcon(
                    AnkhGlyph.Blank,
                    16,
                    false,
                    -1),
                Is.EqualTo(VsStateIcon.STATEICON_BLANK));
        }
    }
}
