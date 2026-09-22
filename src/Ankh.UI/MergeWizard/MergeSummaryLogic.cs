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

using SharpSvn;

namespace Ankh.UI.MergeWizard
{
    internal sealed class MergeSummaryInput
    {
        public MergeSummaryInput(
            MergeWizard.MergeType mergeType,
            string mergeTarget,
            string defaultMergeSource,
            string twoTreeSourceOne,
            string twoTreeSourceTwo,
            bool hasSecondTwoTreeSource,
            long mergeFromRevision,
            long mergeToRevision,
            string mergeRevisions,
            MergeOptionsPage.ConflictResolutionOption binaryConflictResolution,
            MergeOptionsPage.ConflictResolutionOption textConflictResolution,
            SvnDepth depth,
            bool ignoreAncestry,
            bool allowUnversionedObstructions)
        {
            MergeType = mergeType;
            MergeTarget = mergeTarget;
            DefaultMergeSource = defaultMergeSource;
            TwoTreeSourceOne = twoTreeSourceOne;
            TwoTreeSourceTwo = twoTreeSourceTwo;
            HasSecondTwoTreeSource = hasSecondTwoTreeSource;
            MergeFromRevision = mergeFromRevision;
            MergeToRevision = mergeToRevision;
            MergeRevisions = mergeRevisions;
            BinaryConflictResolution = binaryConflictResolution;
            TextConflictResolution = textConflictResolution;
            Depth = depth;
            IgnoreAncestry = ignoreAncestry;
            AllowUnversionedObstructions = allowUnversionedObstructions;
        }

        public MergeWizard.MergeType MergeType { get; private set; }
        public string MergeTarget { get; private set; }
        public string DefaultMergeSource { get; private set; }
        public string TwoTreeSourceOne { get; private set; }
        public string TwoTreeSourceTwo { get; private set; }
        public bool HasSecondTwoTreeSource { get; private set; }
        public long MergeFromRevision { get; private set; }
        public long MergeToRevision { get; private set; }
        public string MergeRevisions { get; private set; }
        public MergeOptionsPage.ConflictResolutionOption BinaryConflictResolution { get; private set; }
        public MergeOptionsPage.ConflictResolutionOption TextConflictResolution { get; private set; }
        public SvnDepth Depth { get; private set; }
        public bool IgnoreAncestry { get; private set; }
        public bool AllowUnversionedObstructions { get; private set; }
    }

    internal sealed class MergeSummaryModel
    {
        public MergeSummaryModel(
            string mergeTarget,
            string mergeSourceOne,
            string mergeSourceTwo,
            string revisions,
            string binaryConflicts,
            string textConflicts,
            string depth,
            string ignoreAncestry,
            string allowUnversionedObstructions)
        {
            MergeTarget = mergeTarget;
            MergeSourceOne = mergeSourceOne;
            MergeSourceTwo = mergeSourceTwo;
            Revisions = revisions;
            BinaryConflicts = binaryConflicts;
            TextConflicts = textConflicts;
            Depth = depth;
            IgnoreAncestry = ignoreAncestry;
            AllowUnversionedObstructions = allowUnversionedObstructions;
        }

        public string MergeTarget { get; private set; }
        public string MergeSourceOne { get; private set; }
        public string MergeSourceTwo { get; private set; }
        public string Revisions { get; private set; }
        public string BinaryConflicts { get; private set; }
        public string TextConflicts { get; private set; }
        public string Depth { get; private set; }
        public string IgnoreAncestry { get; private set; }
        public string AllowUnversionedObstructions { get; private set; }
    }

    internal static class MergeSummaryLogic
    {
        public static MergeSummaryModel Build(MergeSummaryInput input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            bool twoTrees =
                input.MergeType == MergeWizard.MergeType.TwoDifferentTrees;

            string sourceOne =
                twoTrees ? input.TwoTreeSourceOne : input.DefaultMergeSource;

            string sourceTwo = twoTrees
                ? (input.HasSecondTwoTreeSource
                    ? input.TwoTreeSourceTwo
                    : input.TwoTreeSourceOne)
                : MergeStrings.NotApplicableShort;

            string revisions;
            if (twoTrees)
            {
                revisions =
                    RevisionText(input.MergeFromRevision)
                    + "-"
                    + RevisionText(input.MergeToRevision);
            }
            else
            {
                revisions = input.MergeRevisions ?? MergeStrings.All;
            }

            return new MergeSummaryModel(
                input.MergeTarget,
                sourceOne,
                sourceTwo,
                revisions,
                ConflictText(input.BinaryConflictResolution),
                ConflictText(input.TextConflictResolution),
                DepthText(input.Depth),
                input.IgnoreAncestry ? MergeStrings.Yes : MergeStrings.No,
                input.AllowUnversionedObstructions
                    ? MergeStrings.Yes
                    : MergeStrings.No);
        }

        static string RevisionText(long revision)
        {
            return revision != -1
                ? revision.ToString()
                : MergeStrings.HEAD;
        }

        static string ConflictText(
            MergeOptionsPage.ConflictResolutionOption option)
        {
            switch (option)
            {
                case MergeOptionsPage.ConflictResolutionOption.MARK:
                    return MergeStrings.ConflictHandlingMark;
                case MergeOptionsPage.ConflictResolutionOption.MINE:
                    return MergeStrings.ConflictHandlingMine;
                case MergeOptionsPage.ConflictResolutionOption.PROMPT:
                    return MergeStrings.ConflictHandlingPrompt;
                case MergeOptionsPage.ConflictResolutionOption.THEIRS:
                    return MergeStrings.ConflictHandlingTheirs;
                case MergeOptionsPage.ConflictResolutionOption.BASE:
                    return MergeStrings.ConflictHandlingBase;
                default:
                    return "";
            }
        }

        static string DepthText(SvnDepth depth)
        {
            switch (depth)
            {
                case SvnDepth.Children:
                    return MergeStrings.SvnDepthChildren;
                case SvnDepth.Empty:
                    return MergeStrings.SvnDepthEmpty;
                case SvnDepth.Files:
                    return MergeStrings.SvnDepthFiles;
                case SvnDepth.Infinity:
                    return MergeStrings.SvnDepthInfinity;
                case SvnDepth.Unknown:
                    return MergeStrings.SvnDepthUnknown;
                default:
                    return "";
            }
        }
    }
}
