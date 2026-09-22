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
    public class GlyphTipLogicTests
    {
        [Test]
        public void GetIndicators_ReturnsNoneForCleanItem()
        {
            Assert.That(
                GlyphTipLogic.GetIndicators(
                    false, false, false, false, true,
                    true, true, false, false),
                Is.EqualTo(GlyphTipIndicators.None));
        }

        [TestCase(false, true, false)]
        [TestCase(true, false, false)]
        [TestCase(true, true, true)]
        public void GetIndicators_ModifiedOnlyAppliesToSubitems(
            bool isSubItem,
            bool modified,
            bool expected)
        {
            GlyphTipIndicators result = GlyphTipLogic.GetIndicators(
                isSubItem, modified, false, false, true,
                true, true, false, false);

            Assert.That(
                (result & GlyphTipIndicators.Modified) != 0,
                Is.EqualTo(expected));
        }

        [Test]
        public void GetIndicators_CombinesConflictAndLock()
        {
            GlyphTipIndicators result = GlyphTipLogic.GetIndicators(
                false, false, true, false, true,
                true, true, false, true);

            Assert.That(
                result,
                Is.EqualTo(
                    GlyphTipIndicators.Conflict
                    | GlyphTipIndicators.Locked));
        }

        [TestCase(true, "FileObstructed")]
        [TestCase(false, "DirectoryObstructed")]
        public void GetIndicators_DistinguishesObstructedFileAndDirectory(
            bool isFile,
            string expected)
        {
            GlyphTipIndicators result = GlyphTipLogic.GetIndicators(
                false, false, false, true, isFile,
                true, true, false, false);

            Assert.That(result.ToString(), Is.EqualTo(expected));
        }

        [TestCase(false, true, false, true)]
        [TestCase(true, true, false, false)]
        [TestCase(false, false, false, false)]
        [TestCase(false, true, true, false)]
        public void GetIndicators_MissingRequiresVersionedUnscheduledItem(
            bool exists,
            bool versioned,
            bool deleteScheduled,
            bool expected)
        {
            GlyphTipIndicators result = GlyphTipLogic.GetIndicators(
                false, false, false, false, true,
                exists, versioned, deleteScheduled, false);

            Assert.That(
                (result & GlyphTipIndicators.DoesNotExist) != 0,
                Is.EqualTo(expected));
        }

        [Test]
        public void GetIndicators_CanCombineEveryIndicator()
        {
            GlyphTipIndicators result = GlyphTipLogic.GetIndicators(
                true, true, true, true, true,
                false, true, false, true);

            Assert.That(
                result,
                Is.EqualTo(
                    GlyphTipIndicators.Modified
                    | GlyphTipIndicators.Conflict
                    | GlyphTipIndicators.FileObstructed
                    | GlyphTipIndicators.DoesNotExist
                    | GlyphTipIndicators.Locked));
        }
    }
}
