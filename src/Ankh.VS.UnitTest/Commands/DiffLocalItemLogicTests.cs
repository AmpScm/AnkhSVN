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

using Ankh;
using Ankh.Commands;
using NUnit.Framework;
using SharpSvn;

namespace AnkhSvn_UnitTestProject.Commands
{
    [TestFixture]
    public class DiffLocalItemLogicTests
    {
        [TestCase(AnkhCommand.ItemCompareBase, SvnRevisionType.Base, SvnRevisionType.Working)]
        [TestCase(AnkhCommand.ItemShowChanges, SvnRevisionType.Base, SvnRevisionType.Working)]
        [TestCase(AnkhCommand.DocumentShowChanges, SvnRevisionType.Base, SvnRevisionType.Working)]
        [TestCase(AnkhCommand.ItemCompareCommitted, SvnRevisionType.Committed, SvnRevisionType.Working)]
        [TestCase(AnkhCommand.ItemCompareLatest, SvnRevisionType.Head, SvnRevisionType.Working)]
        [TestCase(AnkhCommand.ItemComparePrevious, SvnRevisionType.Previous, SvnRevisionType.Working)]
        public void GetDefaultRevisionRange_MapsKnownCompareCommands(
            AnkhCommand command,
            SvnRevisionType expectedStart,
            SvnRevisionType expectedEnd)
        {
            SvnRevisionRange range = DiffLocalItemLogic.GetDefaultRevisionRange(command);

            Assert.That(range, Is.Not.Null);
            Assert.That(range.StartRevision.RevisionType, Is.EqualTo(expectedStart));
            Assert.That(range.EndRevision.RevisionType, Is.EqualTo(expectedEnd));
        }

        [TestCase(AnkhCommand.DiffLocalItem)]
        [TestCase(AnkhCommand.ItemCompareSpecific)]
        public void GetDefaultRevisionRange_LeavesSelectorCommandsUnresolved(AnkhCommand command)
        {
            Assert.That(DiffLocalItemLogic.GetDefaultRevisionRange(command), Is.Null);
        }

        [Test]
        public void ShouldInclude_RejectsUnversionedAndPlainAddedFiles()
        {
            Assert.That(
                DiffLocalItemLogic.ShouldInclude(
                    AnkhCommand.ItemCompareLatest,
                    Item(isVersioned: false)),
                Is.False);

            Assert.That(
                DiffLocalItemLogic.ShouldInclude(
                    AnkhCommand.ItemCompareLatest,
                    Item(localNodeStatus: SvnStatus.Added, isCopied: false)),
                Is.False);

            Assert.That(
                DiffLocalItemLogic.ShouldInclude(
                    AnkhCommand.ItemCompareLatest,
                    Item(localNodeStatus: SvnStatus.Added, isCopied: true)),
                Is.True,
                "Copied adds have repository history and remain diffable.");
        }

        [Test]
        public void ShouldInclude_BaseAndShowChangesRequireLocalChangesAndLocalDiff()
        {
            Assert.That(
                DiffLocalItemLogic.ShouldInclude(
                    AnkhCommand.ItemCompareBase,
                    Item(isModified: false, isDocumentDirty: false)),
                Is.False);

            Assert.That(
                DiffLocalItemLogic.ShouldInclude(
                    AnkhCommand.ItemCompareBase,
                    Item(isDocumentDirty: true, isLocalDiffAvailable: true)),
                Is.True);

            Assert.That(
                DiffLocalItemLogic.ShouldInclude(
                    AnkhCommand.ItemShowChanges,
                    Item(isModified: true, isLocalDiffAvailable: false)),
                Is.False);

            Assert.That(
                DiffLocalItemLogic.ShouldInclude(
                    AnkhCommand.ItemShowChanges,
                    Item(isModified: true, isLocalDiffAvailable: true)),
                Is.True);
        }

        [Test]
        public void ShouldInclude_DiffLocalRejectsDeletedOrMissingWorkingFiles()
        {
            Assert.That(
                DiffLocalItemLogic.ShouldInclude(
                    AnkhCommand.DiffLocalItem,
                    Item(isDeleteScheduled: true)),
                Is.False);

            Assert.That(
                DiffLocalItemLogic.ShouldInclude(
                    AnkhCommand.DiffLocalItem,
                    Item(exists: false)),
                Is.False);

            Assert.That(
                DiffLocalItemLogic.ShouldInclude(
                    AnkhCommand.DiffLocalItem,
                    Item()),
                Is.True);
        }

        [Test]
        public void RequiresDocumentSave_DetectsEitherWorkingRevision()
        {
            Assert.That(
                DiffLocalItemLogic.RequiresDocumentSave(
                    new SvnRevisionRange(SvnRevision.Base, SvnRevision.Working)),
                Is.True);

            Assert.That(
                DiffLocalItemLogic.RequiresDocumentSave(
                    new SvnRevisionRange(SvnRevision.Working, SvnRevision.Base)),
                Is.True);

            Assert.That(
                DiffLocalItemLogic.RequiresDocumentSave(
                    new SvnRevisionRange(SvnRevision.Base, SvnRevision.Head)),
                Is.False);

            Assert.That(DiffLocalItemLogic.RequiresDocumentSave(null), Is.False);
        }

        [Test]
        public void ShouldUseCopyOrigin_RequiresCopyOrReplaceAndRepositorySide()
        {
            SvnRevisionRange repositoryToWorking =
                new SvnRevisionRange(SvnRevision.Head, SvnRevision.Working);
            SvnRevisionRange workingToWorking =
                new SvnRevisionRange(SvnRevision.Working, SvnRevision.Working);

            Assert.That(
                DiffLocalItemLogic.ShouldUseCopyOrigin(true, false, repositoryToWorking),
                Is.True);
            Assert.That(
                DiffLocalItemLogic.ShouldUseCopyOrigin(false, true, repositoryToWorking),
                Is.True);
            Assert.That(
                DiffLocalItemLogic.ShouldUseCopyOrigin(false, false, repositoryToWorking),
                Is.False);
            Assert.That(
                DiffLocalItemLogic.ShouldUseCopyOrigin(true, false, workingToWorking),
                Is.False);
            Assert.That(
                DiffLocalItemLogic.ShouldUseCopyOrigin(true, false, null),
                Is.False);
        }

        static DiffLocalItemSelectionInfo Item(
            bool isVersioned = true,
            SvnStatus localNodeStatus = SvnStatus.Normal,
            bool isCopied = false,
            bool isModified = false,
            bool isDocumentDirty = false,
            bool isLocalDiffAvailable = true,
            bool isDeleteScheduled = false,
            bool exists = true)
        {
            return new DiffLocalItemSelectionInfo(
                isVersioned,
                localNodeStatus,
                isCopied,
                isModified,
                isDocumentDirty,
                isLocalDiffAvailable,
                isDeleteScheduled,
                exists);
        }
    }
}
