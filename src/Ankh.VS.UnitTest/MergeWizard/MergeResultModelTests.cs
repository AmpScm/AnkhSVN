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

using System.Collections.Generic;
using Ankh.UI.MergeWizard;
using NUnit.Framework;
using SharpSvn;

namespace AnkhSvn_UnitTestProject.MergeWizard
{
    [TestFixture]
    public class MergeResultModelTests
    {
        [Test]
        public void Build_ClassifiesStructuralMergeActions()
        {
            MergeResultModel model = MergeResultModel.Build(
                new[]
                {
                    Notification(@"C:\wc\exists.txt", SvnNotifyAction.Exists, SvnNodeKind.File),
                    Notification(@"C:\wc\skip-dir", SvnNotifyAction.Skip, SvnNodeKind.Directory),
                    Notification(@"C:\wc\skip-file.txt", SvnNotifyAction.Skip, SvnNodeKind.File),
                    Notification(@"C:\wc\added.txt", SvnNotifyAction.UpdateAdd, SvnNodeKind.File),
                    Notification(@"C:\wc\deleted.txt", SvnNotifyAction.UpdateDelete, SvnNodeKind.File),
                    Notification(@"C:\wc\replaced.txt", SvnNotifyAction.UpdateReplace, SvnNodeKind.File)
                },
                EmptyResolutions());

            Assert.That(model.FileExisted, Is.EqualTo(1));
            Assert.That(model.FileSkippedDirectories, Is.EqualTo(1));
            Assert.That(model.FileSkippedFiles, Is.EqualTo(1));
            Assert.That(model.FileAdded, Is.EqualTo(2),
                "UpdateReplace historically contributes to the Added total.");
            Assert.That(model.FileDeleted, Is.EqualTo(1));
            Assert.That(model.Paths.Count, Is.EqualTo(6));

            CollectionAssert.AreEqual(
                new[] { MergeResultActionKind.Replaced },
                model.Paths[@"C:\wc\replaced.txt"].ContentActions);
        }

        [Test]
        public void Build_ClassifiesContentAndPropertyStatesIndependently()
        {
            MergeResultModel model = MergeResultModel.Build(
                new[]
                {
                    Notification(
                        @"C:\wc\changed.txt",
                        SvnNotifyAction.UpdateUpdate,
                        SvnNodeKind.File,
                        SvnNotifyState.Changed,
                        SvnNotifyState.Changed),
                    Notification(
                        @"C:\wc\conflicted.txt",
                        SvnNotifyAction.UpdateUpdate,
                        SvnNodeKind.File,
                        SvnNotifyState.Conflicted,
                        SvnNotifyState.Conflicted),
                    Notification(
                        @"C:\wc\merged.txt",
                        SvnNotifyAction.UpdateUpdate,
                        SvnNodeKind.File,
                        SvnNotifyState.Merged,
                        SvnNotifyState.Merged)
                },
                EmptyResolutions());

            Assert.That(model.FileUpdated, Is.EqualTo(1));
            Assert.That(model.FileConflicted, Is.EqualTo(1));
            Assert.That(model.FileMerged, Is.EqualTo(1));

            Assert.That(model.PropertyUpdated, Is.EqualTo(1));
            Assert.That(model.PropertyConflicted, Is.EqualTo(1));
            Assert.That(model.PropertyMerged, Is.EqualTo(1));

            CollectionAssert.AreEqual(
                new[] { MergeResultActionKind.Modified },
                model.Paths[@"C:\wc\changed.txt"].ContentActions);
            CollectionAssert.AreEqual(
                new[] { MergeResultActionKind.Modified },
                model.Paths[@"C:\wc\changed.txt"].PropertyActions);
        }

        [Test]
        public void Build_DeduplicatesRepeatedNotificationsForSamePathAndAction()
        {
            MergeNotificationInfo changed = Notification(
                @"C:\wc\same.txt",
                SvnNotifyAction.UpdateUpdate,
                SvnNodeKind.File,
                SvnNotifyState.Changed,
                SvnNotifyState.Changed);

            MergeResultModel model = MergeResultModel.Build(
                new[]
                {
                    changed,
                    changed,
                    Notification(
                        @"C:\wc\same.txt",
                        SvnNotifyAction.UpdateUpdate,
                        SvnNodeKind.File,
                        SvnNotifyState.Merged,
                        SvnNotifyState.Merged),
                    Notification(
                        @"C:\wc\same.txt",
                        SvnNotifyAction.UpdateUpdate,
                        SvnNodeKind.File,
                        SvnNotifyState.Merged,
                        SvnNotifyState.Merged)
                },
                EmptyResolutions());

            Assert.That(model.Paths.Count, Is.EqualTo(1));
            Assert.That(model.FileUpdated, Is.EqualTo(1));
            Assert.That(model.FileMerged, Is.EqualTo(1));
            Assert.That(model.PropertyUpdated, Is.EqualTo(1));
            Assert.That(model.PropertyMerged, Is.EqualTo(1));

            CollectionAssert.AreEqual(
                new[]
                {
                    MergeResultActionKind.Modified,
                    MergeResultActionKind.Merged
                },
                model.Paths[@"C:\wc\same.txt"].ContentActions);

            CollectionAssert.AreEqual(
                new[]
                {
                    MergeResultActionKind.Modified,
                    MergeResultActionKind.Merged
                },
                model.Paths[@"C:\wc\same.txt"].PropertyActions);
        }

        [Test]
        public void Build_IgnoresNonChangingStatesAndUnsupportedNotifications()
        {
            MergeResultModel model = MergeResultModel.Build(
                new[]
                {
                    Notification(
                        @"C:\wc\none.txt",
                        SvnNotifyAction.UpdateUpdate,
                        SvnNodeKind.File,
                        SvnNotifyState.None,
                        SvnNotifyState.None),
                    Notification(
                        @"C:\wc\unchanged.txt",
                        SvnNotifyAction.UpdateUpdate,
                        SvnNodeKind.File,
                        SvnNotifyState.Unchanged,
                        SvnNotifyState.Unchanged),
                    Notification(
                        @"C:\wc\unknown.txt",
                        SvnNotifyAction.UpdateUpdate,
                        SvnNodeKind.File,
                        SvnNotifyState.Unknown,
                        SvnNotifyState.Unknown),
                    Notification(
                        @"C:\wc\completed.txt",
                        SvnNotifyAction.UpdateCompleted,
                        SvnNodeKind.File)
                },
                EmptyResolutions());

            Assert.That(model.Paths, Is.Empty);
            Assert.That(model.FileUpdated, Is.Zero);
            Assert.That(model.FileConflicted, Is.Zero);
            Assert.That(model.FileMerged, Is.Zero);
            Assert.That(model.PropertyUpdated, Is.Zero);
            Assert.That(model.PropertyConflicted, Is.Zero);
            Assert.That(model.PropertyMerged, Is.Zero);
        }

        [Test]
        public void Build_CountsResolvedConflictsWithContentPrecedence()
        {
            Dictionary<string, List<SvnConflictType>> resolutions =
                new Dictionary<string, List<SvnConflictType>>
                {
                    {
                        @"C:\wc\content.txt",
                        new List<SvnConflictType> { SvnConflictType.Content }
                    },
                    {
                        @"C:\wc\property.txt",
                        new List<SvnConflictType> { SvnConflictType.Property }
                    },
                    {
                        @"C:\wc\both.txt",
                        new List<SvnConflictType>
                        {
                            SvnConflictType.Content,
                            SvnConflictType.Property
                        }
                    }
                };

            MergeResultModel model = MergeResultModel.Build(
                new MergeNotificationInfo[0],
                resolutions);

            Assert.That(model.FileResolved, Is.EqualTo(2));
            Assert.That(model.PropertyResolved, Is.EqualTo(1));
        }

        [Test]
        public void FormatActions_ProducesMergeDialogTextAndUnchangedFallback()
        {
            Assert.That(
                MergeResultModel.FormatActions(new MergeResultActionKind[0]),
                Is.EqualTo(MergeStrings.Unchanged));

            Assert.That(
                MergeResultModel.FormatActions(
                    new[]
                    {
                        MergeResultActionKind.Existed,
                        MergeResultActionKind.Skipped,
                        MergeResultActionKind.Added,
                        MergeResultActionKind.Deleted,
                        MergeResultActionKind.Replaced,
                        MergeResultActionKind.Modified,
                        MergeResultActionKind.Conflicted,
                        MergeResultActionKind.Merged
                    }),
                Is.EqualTo(
                    MergeStrings.Existed + ", " +
                    MergeStrings.Skipped + ", " +
                    MergeStrings.Added + ", " +
                    MergeStrings.Deleted + ", " +
                    MergeStrings.Replaced + ", " +
                    MergeStrings.Modified + ", " +
                    MergeStrings.Conflicted + ", " +
                    MergeStrings.Merged));
        }

        [Test]
        public void Build_AllowsMissingCollections()
        {
            MergeResultModel model = MergeResultModel.Build(null, null);

            Assert.That(model, Is.Not.Null);
            Assert.That(model.Paths, Is.Empty);
            Assert.That(model.FileResolved, Is.Zero);
            Assert.That(model.PropertyResolved, Is.Zero);
        }

        static MergeNotificationInfo Notification(
            string path,
            SvnNotifyAction action,
            SvnNodeKind nodeKind,
            SvnNotifyState contentState = SvnNotifyState.None,
            SvnNotifyState propertyState = SvnNotifyState.None)
        {
            return new MergeNotificationInfo(
                path,
                action,
                nodeKind,
                contentState,
                propertyState);
        }

        static Dictionary<string, List<SvnConflictType>> EmptyResolutions()
        {
            return new Dictionary<string, List<SvnConflictType>>();
        }
    }
}
