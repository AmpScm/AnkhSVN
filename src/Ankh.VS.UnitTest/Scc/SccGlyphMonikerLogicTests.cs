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
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Scc
{
    [TestFixture]
    public class SccGlyphMonikerLogicTests
    {
        [Test]
        public void GetMoniker_MapsEveryAnkhStatusToVisualStudioCatalog()
        {
            AssertEmpty(AnkhGlyph.None);
            AssertMoniker(AnkhGlyph.MustLock, KnownMonikers.OverlayLock);
            AssertMoniker(AnkhGlyph.Modified, KnownMonikers.PendingChangeNode);
            AssertMoniker(AnkhGlyph.Deleted, KnownMonikers.PendingDeleteNode);
            AssertMoniker(AnkhGlyph.FileDirty, KnownMonikers.PendingChangeNode);
            AssertEmpty(AnkhGlyph.Blank);
            AssertMoniker(AnkhGlyph.Normal, KnownMonikers.CheckedInNode);
            AssertMoniker(AnkhGlyph.FileMissing, KnownMonikers.DocumentWarning);
            AssertMoniker(AnkhGlyph.CopiedOrMoved, KnownMonikers.PendingAddNode);
            AssertMoniker(AnkhGlyph.LockedNormal, KnownMonikers.Lock);
            AssertMoniker(AnkhGlyph.LockedModified, KnownMonikers.CheckedOutForEditNode);
            AssertMoniker(AnkhGlyph.Ignored, KnownMonikers.StatusExcluded);
            AssertMoniker(AnkhGlyph.Added, KnownMonikers.PendingAddNode);
            AssertMoniker(AnkhGlyph.ShouldBeAdded, KnownMonikers.CheckAdd);
            AssertMoniker(AnkhGlyph.InConflict, KnownMonikers.Conflict);
            AssertMoniker(AnkhGlyph.ChildChanged, KnownMonikers.PendingChangeNode);
        }

        [Test]
        public void CreateImageList_PreservesAnkhGlyphOrdering()
        {
            var list = SccGlyphMonikerLogic.CreateImageList();
            Assert.That(list.ImageCount, Is.EqualTo(16));

            var images = new ImageMoniker[list.ImageCount];
            list.GetImageMonikers(0, images.Length, images);

            for (int i = 0; i < images.Length; i++)
            {
                ImageMoniker expected =
                    SccGlyphMonikerLogic.GetMoniker((AnkhGlyph)i);

                Assert.That(images[i].Guid, Is.EqualTo(expected.Guid));
                Assert.That(images[i].Id, Is.EqualTo(expected.Id));
            }
        }

        private static void AssertEmpty(AnkhGlyph glyph)
        {
            ImageMoniker actual = SccGlyphMonikerLogic.GetMoniker(glyph);
            Assert.That(actual.Guid, Is.EqualTo(Guid.Empty));
            Assert.That(actual.Id, Is.EqualTo(0));
        }

        private static void AssertMoniker(
            AnkhGlyph glyph,
            ImageMoniker expected)
        {
            ImageMoniker actual = SccGlyphMonikerLogic.GetMoniker(glyph);
            Assert.That(actual.Guid, Is.EqualTo(expected.Guid));
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
        }
    }
}
