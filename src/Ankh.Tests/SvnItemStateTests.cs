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
            Assert.That(item.IsModified, Is.False);
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
