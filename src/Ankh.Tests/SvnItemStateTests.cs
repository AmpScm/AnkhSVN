using System;
using System.Reflection;
using System.Runtime.Serialization;
using Ankh.Scc;
using NUnit.Framework;
using Rhino.Mocks;
using SharpSvn;

namespace Ankh.Tests
{
    [TestFixture]
    public class SvnItemStateTests
    {
        static SvnStatusData Status(
            SvnStatus node,
            SvnStatus text = SvnStatus.Normal,
            SvnStatus property = SvnStatus.None,
            SvnNodeKind nodeKind = SvnNodeKind.File,
            bool copied = false,
            bool moved = false,
            bool conflicted = false,
            bool localFileExists = true,
            bool locked = false)
        {
            var status = (SvnStatusData)FormatterServices.GetUninitializedObject(typeof(SvnStatusData));

            SetField(status, "_localNodeStatus", node);
            SetField(status, "_localTextStatus", text);
            SetField(status, "_localPropertyStatus", property);
            SetField(status, "_nodeKind", nodeKind);
            SetField(status, "_localCopied", copied);
            SetField(status, "_movedHere", moved);
            SetField(status, "_conflicted", conflicted);
            SetField(status, "_localFileExists", localFileExists);
            SetField(status, "_localLocked", locked);

            return status;
        }

        static void SetField<T>(SvnStatusData status, string name, T value)
        {
            typeof(SvnStatusData)
                .GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(status, value);
        }

        static SvnItem Item(SvnStatusData status)
        {
            ISvnStatusCache cache = MockRepository.GenerateStub<ISvnStatusCache>();
            return new SvnItem(cache, @"C:\wc\item.txt", status);
        }

        [Test]
        public void NormalFileIsVersionedAndClean()
        {
            SvnItem item = Item(Status(SvnStatus.Normal));

            Assert.That(item.IsVersioned, Is.True);
            Assert.That(item.Exists, Is.True);
            Assert.That(item.IsFile, Is.True);
            Assert.That(item.IsModified, Is.False);
            Assert.That(item.IsPropertyModified, Is.False);
        }

        [Test]
        public void AddedCopyAndMoveStatesArePreserved()
        {
            SvnItem item = Item(Status(
                SvnStatus.Added,
                text: SvnStatus.Modified,
                copied: true,
                moved: true));

            Assert.That(item.IsVersioned, Is.True);
            Assert.That(item.IsAdded, Is.True);
            Assert.That(item.IsMoved, Is.True);
            Assert.That(item.HasCopyableHistory, Is.True);
            Assert.That(item.IsModified, Is.True);
        }

        [Test]
        public void ReplacedCopyRetainsCopyHistory()
        {
            SvnItem item = Item(Status(
                SvnStatus.Replaced,
                text: SvnStatus.Normal,
                copied: true));

            Assert.That(item.IsVersioned, Is.True);
            Assert.That(item.IsReplaced, Is.True);
            Assert.That(item.HasCopyableHistory, Is.True);
            Assert.That(item.IsModified, Is.True);
        }

        [Test]
        public void ContentConflictIsRecorded()
        {
            SvnItem item = Item(Status(
                SvnStatus.Conflicted,
                text: SvnStatus.Conflicted,
                conflicted: true));

            Assert.That(item.IsVersioned, Is.True);
            Assert.That(item.IsConflicted, Is.True);
        }

        [Test]
        public void MissingVersionedFileDoesNotExistOnDisk()
        {
            SvnItem item = Item(Status(
                SvnStatus.Missing,
                localFileExists: false));

            Assert.That(item.IsVersioned, Is.True);
            Assert.That(item.Exists, Is.False);
        }

        [Test]
        public void DeletedFileTracksScheduledDeleteAndDiskAbsence()
        {
            SvnItem item = Item(Status(
                SvnStatus.Deleted,
                localFileExists: false));

            Assert.That(item.IsVersioned, Is.True);
            Assert.That(item.IsDeleteScheduled, Is.True);
            Assert.That(item.Exists, Is.False);
        }

        [Test]
        public void PropertyModificationAndLocalLockAreRecorded()
        {
            SvnItem item = Item(Status(
                SvnStatus.Normal,
                property: SvnStatus.Modified,
                locked: true));

            Assert.That(item.IsPropertyModified, Is.True);
            Assert.That(item.IsLocked, Is.True);
        }

        [Test]
        public void PropertyConflictIsRecordedAsModifiedPropertyState()
        {
            SvnItem item = Item(Status(
                SvnStatus.Normal,
                property: SvnStatus.Conflicted,
                conflicted: true));

            Assert.That(item.IsPropertyModified, Is.True);
            Assert.That(item.IsConflicted, Is.True);
        }

        [Test]
        public void IncompleteItemRemainsVersioned()
        {
            SvnItem item = Item(Status(SvnStatus.Incomplete));

            Assert.That(item.IsVersioned, Is.True);
        }


        [TestCase(SvnStatus.None, false, false, false)]
        [TestCase(SvnStatus.NotVersioned, false, true, false)]
        [TestCase(SvnStatus.Ignored, false, true, true)]
        [TestCase(SvnStatus.Obstructed, false, true, false)]
        public void UnmanagedStatusesPreserveExpectedVersioningAndIgnoreState(
            SvnStatus status,
            bool versioned,
            bool exists,
            bool ignored)
        {
            SvnItem item = Item(Status(status));

            Assert.Multiple(() =>
            {
                Assert.That(item.IsVersioned, Is.EqualTo(versioned));
                Assert.That(item.Exists, Is.EqualTo(exists));
                Assert.That(item.IsIgnored, Is.EqualTo(ignored));
            });
        }

        [TestCase(false, false)]
        [TestCase(true, true)]
        public void DeletedStatusReflectsWhetherTheScheduledDeleteStillExists(bool localFileExists, bool exists)
        {
            SvnItem item = Item(Status(
                SvnStatus.Deleted,
                localFileExists: localFileExists));

            Assert.Multiple(() =>
            {
                Assert.That(item.IsVersioned, Is.True);
                Assert.That(item.IsDeleteScheduled, Is.True);
                Assert.That(item.Exists, Is.EqualTo(exists));
            });
        }

        [TestCase(SvnStatus.Added, false, false)]
        [TestCase(SvnStatus.Added, true, false)]
        [TestCase(SvnStatus.Added, true, true)]
        [TestCase(SvnStatus.Replaced, false, false)]
        [TestCase(SvnStatus.Replaced, true, false)]
        [TestCase(SvnStatus.Replaced, true, true)]
        public void AddedAndReplacedStatesCoverCopyAndMoveCombinations(
            SvnStatus nodeStatus,
            bool copied,
            bool moved)
        {
            SvnItem item = Item(Status(
                nodeStatus,
                text: SvnStatus.Normal,
                copied: copied,
                moved: moved));

            Assert.Multiple(() =>
            {
                Assert.That(item.IsAdded, Is.EqualTo(nodeStatus == SvnStatus.Added));
                Assert.That(item.IsReplaced, Is.EqualTo(nodeStatus == SvnStatus.Replaced));
                Assert.That(item.HasCopyableHistory, Is.EqualTo(copied));
                Assert.That(item.IsMoved, Is.EqualTo(copied && moved));
                Assert.That(item.IsVersioned, Is.True);
            });
        }

        [TestCase(SvnStatus.Modified, SvnStatus.Modified, true, false)]
        [TestCase(SvnStatus.Modified, SvnStatus.Conflicted, false, true)]
        [TestCase(SvnStatus.Modified, SvnStatus.Normal, false, false)]
        [TestCase(SvnStatus.Conflicted, SvnStatus.Modified, true, false)]
        [TestCase(SvnStatus.Conflicted, SvnStatus.Conflicted, false, true)]
        [TestCase(SvnStatus.Conflicted, SvnStatus.Normal, false, false)]
        public void ModifiedAndConflictedNodeStatesHonorTextStatus(
            SvnStatus node,
            SvnStatus text,
            bool modified,
            bool contentConflicted)
        {
            SvnItem item = Item(Status(
                node,
                text: text,
                conflicted: contentConflicted));

            Assert.Multiple(() =>
            {
                Assert.That(item.IsModified, Is.EqualTo(modified));
                Assert.That(item.IsConflicted, Is.EqualTo(contentConflicted));
                Assert.That(item.IsVersioned, Is.True);
            });
        }

        [TestCase(SvnStatus.None, false)]
        [TestCase(SvnStatus.Normal, false)]
        [TestCase(SvnStatus.Modified, true)]
        [TestCase(SvnStatus.Conflicted, true)]
        public void PropertyStatusesCoverCleanModifiedAndConflictPaths(
            SvnStatus propertyStatus,
            bool propertyModified)
        {
            SvnItem item = Item(Status(
                SvnStatus.Normal,
                property: propertyStatus,
                conflicted: propertyStatus == SvnStatus.Conflicted));

            Assert.That(item.IsPropertyModified, Is.EqualTo(propertyModified));
        }

        [Test]
        public void NoSccConstructorsCoverVersionableAndMissingStates()
        {
            ISvnStatusCache cache = MockRepository.GenerateStub<ISvnStatusCache>();

            var notVersioned = new SvnItem(
                cache, @"C:\wc\new.txt", NoSccStatus.NotVersioned, SvnNodeKind.File);
            var notVersionable = new SvnItem(
                cache, @"C:\wc\special", NoSccStatus.NotVersionable, SvnNodeKind.Unknown);
            var missing = new SvnItem(
                cache, @"C:\wc\gone.txt", NoSccStatus.NotExisting, SvnNodeKind.Unknown);

            Assert.Multiple(() =>
            {
                Assert.That(notVersioned.IsVersioned, Is.False);
                Assert.That(notVersioned.Exists, Is.True);
                Assert.That(notVersionable.IsVersioned, Is.False);
                Assert.That(missing.IsVersioned, Is.False);
            });
        }

        [Test]
        public void PathValidationRejectsWindowsInvalidCharactersOnlyWhenExtraChecksRequested()
        {
            Assert.Multiple(() =>
            {
                Assert.That(SvnItem.IsValidPath(@"C:\wc\good.txt", true), Is.True);
                Assert.That(SvnItem.IsValidPath(@"C:\wc\bad?.txt", true), Is.False);
                Assert.That(SvnItem.IsValidPath(@"C:\wc\bad*.txt", true), Is.False);
                Assert.That(SvnItem.IsValidPath(@"C:\wc\bad|.txt", true), Is.False);
                Assert.That(SvnItem.IsValidPath(@"C:\wc\bad<.txt", true), Is.False);
                Assert.That(SvnItem.IsValidPath(@"C:\wc\bad>.txt", true), Is.False);
                Assert.That(SvnItem.IsValidPath("C:\\wc\\bad" + (char)1 + ".txt", true), Is.False);
                Assert.That(SvnItem.IsValidPath(@"C:\wc\bad?.txt", false), Is.True);
            });
        }

        [Test]
        public void IsBelowRootRequiresAPathBoundaryAndHandlesExactRoot()
        {
            Assert.Multiple(() =>
            {
                Assert.That(SvnItem.IsBelowRoot(@"C:\wc", @"C:\wc"), Is.True);
                Assert.That(SvnItem.IsBelowRoot(@"C:\wc\file.txt", @"C:\wc"), Is.True);
                Assert.That(SvnItem.IsBelowRoot(@"C:\WC\file.txt", @"c:\wc"), Is.True);
                Assert.That(SvnItem.IsBelowRoot(@"C:\wc2\file.txt", @"C:\wc"), Is.False);
                Assert.That(SvnItem.IsBelowRoot(@"D:\wc\file.txt", @"C:\wc"), Is.False);
                Assert.Throws<ArgumentNullException>(() => SvnItem.IsBelowRoot(null, @"C:\wc"));
                Assert.Throws<ArgumentNullException>(() => SvnItem.IsBelowRoot(@"C:\wc", null));
            });
        }

        [Test]
        public void SubPathCoversShortEqualChildAndPrefixCases()
        {
            Assert.Multiple(() =>
            {
                Assert.That(SvnItem.SubPath(@"C:\wc", @"C:\working"), Is.Empty);
                Assert.That(SvnItem.SubPath(@"C:\wc", @"C:\wc"), Is.EqualTo("."));
                Assert.That(SvnItem.SubPath(@"C:\wc", @"C:\wc", true), Is.EqualTo(@"C:\wc"));
                Assert.That(SvnItem.SubPath(@"C:\wc\dir\file.txt", @"C:\wc"), Is.EqualTo(@"dir\file.txt"));
                Assert.That(SvnItem.SubPath(@"C:\wc2", @"C:\wc"), Is.EqualTo("2"));
                Assert.Throws<ArgumentNullException>(() => SvnItem.SubPath(null, @"C:\wc"));
                Assert.Throws<ArgumentNullException>(() => SvnItem.SubPath(@"C:\wc", null));
            });
        }

        [Test]
        public void StaticCollectionAndEqualityHelpersHandleNullsAndCase()
        {
            SvnItem one = Item(Status(SvnStatus.Normal));
            SvnItem samePathDifferentCase = new SvnItem(
                MockRepository.GenerateStub<ISvnStatusCache>(),
                @"c:\WC\ITEM.TXT",
                Status(SvnStatus.Normal));
            SvnItem other = new SvnItem(
                MockRepository.GenerateStub<ISvnStatusCache>(),
                @"C:\wc\other.txt",
                Status(SvnStatus.Normal));

            Assert.Multiple(() =>
            {
                CollectionAssert.AreEqual(
                    new[] { @"C:\wc\item.txt", @"C:\wc\other.txt" },
                    SvnItem.GetPaths(new[] { one, null, other }));
                Assert.That(one == samePathDifferentCase, Is.True);
                Assert.That(one != other, Is.True);
                Assert.That((SvnItem)null == null, Is.True);
                Assert.That(one == null, Is.False);
                Assert.Throws<ArgumentNullException>(() => SvnItem.GetPaths(null));
            });
        }

        [Test]
        public void ExternalStatusProvidesDirectoryShapeWithoutTreatingItAsFile()
        {
            SvnItem item = Item(Status(
                SvnStatus.External,
                nodeKind: SvnNodeKind.Directory));

            Assert.That(item.Exists, Is.True);
            Assert.That(item.IsDirectory, Is.True);
            Assert.That(item.IsFile, Is.False);
        }
    }
}
