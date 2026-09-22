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

using Ankh.UI.MergeWizard;
using NUnit.Framework;
using SharpSvn;

namespace AnkhSvn_UnitTestProject.MergeWizard
{
    [TestFixture]
    public class MergeSummaryLogicTests
    {
        [Test]
        public void Build_RejectsNullInput()
        {
            Assert.Throws<ArgumentNullException>(
                () => MergeSummaryLogic.Build(null));
        }

        [Test]
        public void Build_FormatsRangeMergeSummary()
        {
            MergeSummaryModel model = MergeSummaryLogic.Build(
                Input(
                    Ankh.UI.MergeWizard.MergeWizard.MergeType.RangeOfRevisions,
                    mergeRevisions: "2-4, 7"));

            Assert.That(model.MergeTarget, Is.EqualTo(@"C:\wc"));
            Assert.That(
                model.MergeSourceOne,
                Is.EqualTo("https://example.invalid/svn/trunk"));
            Assert.That(
                model.MergeSourceTwo,
                Is.EqualTo(MergeStrings.NotApplicableShort));
            Assert.That(model.Revisions, Is.EqualTo("2-4, 7"));
        }

        [Test]
        public void Build_UsesAllWhenNonTreeMergeHasNoRevisionList()
        {
            MergeSummaryModel model = MergeSummaryLogic.Build(
                Input(
                    Ankh.UI.MergeWizard.MergeWizard.MergeType.Reintegrate,
                    mergeRevisions: null));

            Assert.That(model.Revisions, Is.EqualTo(MergeStrings.All));
        }

        [Test]
        public void Build_FormatsTwoTreeSourcesAndRevisions()
        {
            MergeSummaryModel model = MergeSummaryLogic.Build(
                Input(
                    Ankh.UI.MergeWizard.MergeWizard.MergeType.TwoDifferentTrees,
                    hasSecondSource: true,
                    fromRevision: 10,
                    toRevision: 20));

            Assert.That(
                model.MergeSourceOne,
                Is.EqualTo("https://example.invalid/svn/one"));
            Assert.That(
                model.MergeSourceTwo,
                Is.EqualTo("https://example.invalid/svn/two"));
            Assert.That(model.Revisions, Is.EqualTo("10-20"));
        }

        [Test]
        public void Build_TwoTreeFallsBackToFirstSourceAndHead()
        {
            MergeSummaryModel model = MergeSummaryLogic.Build(
                Input(
                    Ankh.UI.MergeWizard.MergeWizard.MergeType.TwoDifferentTrees,
                    hasSecondSource: false,
                    fromRevision: -1,
                    toRevision: -1));

            Assert.That(
                model.MergeSourceTwo,
                Is.EqualTo("https://example.invalid/svn/one"));
            Assert.That(
                model.Revisions,
                Is.EqualTo(MergeStrings.HEAD + "-" + MergeStrings.HEAD));
        }

        [TestCase(MergeOptionsPage.ConflictResolutionOption.MARK, "ConflictHandlingMark")]
        [TestCase(MergeOptionsPage.ConflictResolutionOption.MINE, "ConflictHandlingMine")]
        [TestCase(MergeOptionsPage.ConflictResolutionOption.PROMPT, "ConflictHandlingPrompt")]
        [TestCase(MergeOptionsPage.ConflictResolutionOption.THEIRS, "ConflictHandlingTheirs")]
        [TestCase(MergeOptionsPage.ConflictResolutionOption.BASE, "ConflictHandlingBase")]
        public void Build_MapsConflictResolution(
            MergeOptionsPage.ConflictResolutionOption option,
            string expectedProperty)
        {
            MergeSummaryModel model = MergeSummaryLogic.Build(
                Input(
                    Ankh.UI.MergeWizard.MergeWizard.MergeType.RangeOfRevisions,
                    binary: option,
                    text: option));

            string expected = MergeString(expectedProperty);
            Assert.That(model.BinaryConflicts, Is.EqualTo(expected));
            Assert.That(model.TextConflicts, Is.EqualTo(expected));
        }

        [TestCase(SvnDepth.Children, "SvnDepthChildren")]
        [TestCase(SvnDepth.Empty, "SvnDepthEmpty")]
        [TestCase(SvnDepth.Files, "SvnDepthFiles")]
        [TestCase(SvnDepth.Infinity, "SvnDepthInfinity")]
        [TestCase(SvnDepth.Unknown, "SvnDepthUnknown")]
        public void Build_MapsDepth(SvnDepth depth, string expectedProperty)
        {
            MergeSummaryModel model = MergeSummaryLogic.Build(
                Input(
                    Ankh.UI.MergeWizard.MergeWizard.MergeType.RangeOfRevisions,
                    depth: depth));

            Assert.That(
                model.Depth,
                Is.EqualTo(MergeString(expectedProperty)));
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void Build_MapsBooleanOptions(
            bool ignoreAncestry,
            bool allowObstructions)
        {
            MergeSummaryModel model = MergeSummaryLogic.Build(
                Input(
                    Ankh.UI.MergeWizard.MergeWizard.MergeType.RangeOfRevisions,
                    ignoreAncestry: ignoreAncestry,
                    allowObstructions: allowObstructions));

            Assert.That(
                model.IgnoreAncestry,
                Is.EqualTo(ignoreAncestry ? MergeStrings.Yes : MergeStrings.No));
            Assert.That(
                model.AllowUnversionedObstructions,
                Is.EqualTo(allowObstructions ? MergeStrings.Yes : MergeStrings.No));
        }

        [Test]
        public void Build_UnknownEnumValuesProduceBlankLabels()
        {
            MergeSummaryModel model = MergeSummaryLogic.Build(
                Input(
                    Ankh.UI.MergeWizard.MergeWizard.MergeType.RangeOfRevisions,
                    binary:
                        (MergeOptionsPage.ConflictResolutionOption)Int32.MaxValue,
                    text:
                        (MergeOptionsPage.ConflictResolutionOption)Int32.MaxValue,
                    depth: (SvnDepth)Int32.MaxValue));

            Assert.That(model.BinaryConflicts, Is.Empty);
            Assert.That(model.TextConflicts, Is.Empty);
            Assert.That(model.Depth, Is.Empty);
        }

        static MergeSummaryInput Input(
            Ankh.UI.MergeWizard.MergeWizard.MergeType type,
            string mergeRevisions = "5-8",
            bool hasSecondSource = true,
            long fromRevision = 1,
            long toRevision = 2,
            MergeOptionsPage.ConflictResolutionOption binary =
                MergeOptionsPage.ConflictResolutionOption.PROMPT,
            MergeOptionsPage.ConflictResolutionOption text =
                MergeOptionsPage.ConflictResolutionOption.PROMPT,
            SvnDepth depth = SvnDepth.Infinity,
            bool ignoreAncestry = false,
            bool allowObstructions = false)
        {
            return new MergeSummaryInput(
                type,
                @"C:\wc",
                "https://example.invalid/svn/trunk",
                "https://example.invalid/svn/one",
                "https://example.invalid/svn/two",
                hasSecondSource,
                fromRevision,
                toRevision,
                mergeRevisions,
                binary,
                text,
                depth,
                ignoreAncestry,
                allowObstructions);
        }

        static string MergeString(string propertyName)
        {
            return typeof(MergeStrings)
                .GetProperty(
                    propertyName,
                    System.Reflection.BindingFlags.Static
                    | System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.Public)
                .GetValue(null, null)
                .ToString();
        }
    }
}
