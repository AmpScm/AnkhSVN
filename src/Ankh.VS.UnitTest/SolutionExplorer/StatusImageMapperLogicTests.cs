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
using Ankh.VS.SolutionExplorer;
using NUnit.Framework;
using SharpSvn;

namespace AnkhSvn_UnitTestProject.SolutionExplorer
{
    [TestFixture]
    public class StatusImageMapperLogicTests
    {
        [Test]
        public void GetGlyph_RejectsNullState()
        {
            Assert.Throws<ArgumentNullException>(
                () => StatusImageMapperLogic.GetGlyph(null));
        }

        [Test]
        public void GetGlyph_ConflictStatesTakeHighestPrecedence()
        {
            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(isConflicted: true, isReadOnlyMustLock: true)),
                Is.EqualTo(AnkhGlyph.InConflict));

            Assert.That(
                StatusImageMapperLogic.GetGlyph(Item(isObstructed: true)),
                Is.EqualTo(AnkhGlyph.InConflict));

            Assert.That(
                StatusImageMapperLogic.GetGlyph(Item(isTreeConflicted: true)),
                Is.EqualTo(AnkhGlyph.InConflict));
        }

        [Test]
        public void GetGlyph_MustLockPrecedesVersionedStatus()
        {
            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(
                        isReadOnlyMustLock: true,
                        combinedStatus: SvnStatus.Modified)),
                Is.EqualTo(AnkhGlyph.MustLock));
        }

        [Test]
        public void GetGlyph_ClassifiesUnversionedItems()
        {
            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(isVersioned: false, exists: false)),
                Is.EqualTo(AnkhGlyph.FileMissing));

            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(isVersioned: false, isIgnored: true)),
                Is.EqualTo(AnkhGlyph.Ignored));

            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(
                        isVersioned: false,
                        isVersionable: true,
                        inSolution: true)),
                Is.EqualTo(AnkhGlyph.ShouldBeAdded));

            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(
                        isVersioned: false,
                        isVersionable: true,
                        inSolution: true,
                        isSccExcluded: true)),
                Is.EqualTo(AnkhGlyph.Ignored));

            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(
                        isVersioned: false,
                        isVersionable: true,
                        inSolution: false)),
                Is.EqualTo(AnkhGlyph.None));

            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(
                        isVersioned: false,
                        isVersionable: false)),
                Is.EqualTo(AnkhGlyph.None));
        }

        [Test]
        public void GetGlyph_ClassifiesNormalVersionedItems()
        {
            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(
                        combinedStatus: SvnStatus.Normal,
                        isDocumentDirty: true,
                        isLocked: true)),
                Is.EqualTo(AnkhGlyph.FileDirty));

            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(
                        combinedStatus: SvnStatus.Normal,
                        isLocked: true)),
                Is.EqualTo(AnkhGlyph.LockedNormal));

            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(combinedStatus: SvnStatus.Normal)),
                Is.EqualTo(AnkhGlyph.Normal));
        }

        [TestCase(false, AnkhGlyph.Modified)]
        [TestCase(true, AnkhGlyph.LockedModified)]
        public void GetGlyph_ClassifiesModifiedItems(bool isLocked, AnkhGlyph expected)
        {
            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(
                        combinedStatus: SvnStatus.Modified,
                        isLocked: isLocked)),
                Is.EqualTo(expected));
        }

        [Test]
        public void GetGlyph_ClassifiesReplacedAndAddedItems()
        {
            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(combinedStatus: SvnStatus.Replaced)),
                Is.EqualTo(AnkhGlyph.CopiedOrMoved));

            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(
                        combinedStatus: SvnStatus.Added,
                        isCopied: false)),
                Is.EqualTo(AnkhGlyph.Added));

            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(
                        combinedStatus: SvnStatus.Added,
                        isCopied: true)),
                Is.EqualTo(AnkhGlyph.CopiedOrMoved));
        }

        [TestCase(false, AnkhGlyph.Deleted)]
        [TestCase(true, AnkhGlyph.InConflict)]
        public void GetGlyph_ClassifiesMissingItems(
            bool isCasingConflicted,
            AnkhGlyph expected)
        {
            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(
                        combinedStatus: SvnStatus.Missing,
                        isCasingConflicted: isCasingConflicted)),
                Is.EqualTo(expected));
        }

        [Test]
        public void GetGlyph_ClassifiesDeletedItems()
        {
            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(
                        combinedStatus: SvnStatus.Deleted,
                        exists: true,
                        inSolution: true)),
                Is.EqualTo(AnkhGlyph.ShouldBeAdded));

            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(
                        combinedStatus: SvnStatus.Deleted,
                        exists: true,
                        inSolution: true,
                        isSccExcluded: true)),
                Is.EqualTo(AnkhGlyph.Ignored));

            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(
                        combinedStatus: SvnStatus.Deleted,
                        exists: false,
                        inSolution: true)),
                Is.EqualTo(AnkhGlyph.Deleted));

            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(
                        combinedStatus: SvnStatus.Deleted,
                        exists: true,
                        inSolution: false)),
                Is.EqualTo(AnkhGlyph.Deleted));
        }

        [TestCase(SvnStatus.Conflicted)]
        [TestCase(SvnStatus.Obstructed)]
        [TestCase(SvnStatus.External)]
        [TestCase(SvnStatus.Incomplete)]
        public void GetGlyph_MapsConflictLikeStatusesToConflict(SvnStatus status)
        {
            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(combinedStatus: status)),
                Is.EqualTo(AnkhGlyph.InConflict));
        }

        [Test]
        public void GetGlyph_MapsIgnoredStatusToIgnored()
        {
            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(combinedStatus: SvnStatus.Ignored)),
                Is.EqualTo(AnkhGlyph.Ignored));
        }

        [Test]
        public void GetGlyph_MapsZeroAndUnknownStatusesToNone()
        {
            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(combinedStatus: SvnStatus.Zero)),
                Is.EqualTo(AnkhGlyph.None));

            Assert.That(
                StatusImageMapperLogic.GetGlyph(
                    Item(combinedStatus: (SvnStatus)Int32.MaxValue)),
                Is.EqualTo(AnkhGlyph.None));
        }

        static StatusImageInfo Item(
            bool isConflicted = false,
            bool isObstructed = false,
            bool isTreeConflicted = false,
            bool isReadOnlyMustLock = false,
            bool isVersioned = true,
            bool exists = true,
            bool isIgnored = false,
            bool isVersionable = true,
            bool inSolution = true,
            bool isSccExcluded = false,
            SvnStatus combinedStatus = SvnStatus.Normal,
            bool isDocumentDirty = false,
            bool isLocked = false,
            bool isCopied = false,
            bool isCasingConflicted = false)
        {
            return new StatusImageInfo(
                isConflicted,
                isObstructed,
                isTreeConflicted,
                isReadOnlyMustLock,
                isVersioned,
                exists,
                isIgnored,
                isVersionable,
                inSolution,
                isSccExcluded,
                combinedStatus,
                isDocumentDirty,
                isLocked,
                isCopied,
                isCasingConflicted);
        }
    }
}
