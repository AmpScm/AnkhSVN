using System;
using Ankh.Scc;
using NUnit.Framework;
using SharpSvn;

namespace Ankh.Tests
{
    [TestFixture]
    public class PendingChangeStatusTests
    {
        [TestCase(SvnStatus.Normal, SvnStatus.Normal, SvnStatus.Normal, true, PendingChangeKind.TreeConflict)]
        [TestCase(SvnStatus.Conflicted, SvnStatus.Normal, SvnStatus.Normal, false, PendingChangeKind.Conflicted)]
        [TestCase(SvnStatus.Normal, SvnStatus.Normal, SvnStatus.Conflicted, false, PendingChangeKind.Conflicted)]
        [TestCase(SvnStatus.NotVersioned, SvnStatus.None, SvnStatus.None, false, PendingChangeKind.None)]
        [TestCase(SvnStatus.Modified, SvnStatus.Modified, SvnStatus.Normal, false, PendingChangeKind.Modified)]
        [TestCase(SvnStatus.Modified, SvnStatus.Normal, SvnStatus.Modified, false, PendingChangeKind.PropertyModified)]
        [TestCase(SvnStatus.Replaced, SvnStatus.Normal, SvnStatus.Normal, false, PendingChangeKind.Replaced)]
        [TestCase(SvnStatus.Added, SvnStatus.Normal, SvnStatus.Normal, false, PendingChangeKind.Added)]
        [TestCase(SvnStatus.Deleted, SvnStatus.Normal, SvnStatus.Normal, false, PendingChangeKind.Deleted)]
        [TestCase(SvnStatus.Missing, SvnStatus.Normal, SvnStatus.Normal, false, PendingChangeKind.Missing)]
        [TestCase(SvnStatus.Obstructed, SvnStatus.Normal, SvnStatus.Normal, false, PendingChangeKind.Obstructed)]
        [TestCase(SvnStatus.Incomplete, SvnStatus.Normal, SvnStatus.Normal, false, PendingChangeKind.Incomplete)]
        [TestCase(SvnStatus.External, SvnStatus.Normal, SvnStatus.Normal, false, PendingChangeKind.None)]
        [TestCase(SvnStatus.Normal, SvnStatus.Normal, SvnStatus.Modified, false, PendingChangeKind.PropertyModified)]
        [TestCase(SvnStatus.Normal, SvnStatus.Normal, SvnStatus.Normal, false, PendingChangeKind.None)]
        [TestCase(SvnStatus.Ignored, SvnStatus.None, SvnStatus.None, false, PendingChangeKind.None)]
        public void CombineStatusMapsCommonStates(
            SvnStatus nodeStatus,
            SvnStatus textStatus,
            SvnStatus propertyStatus,
            bool treeConflict,
            PendingChangeKind expected)
        {
            Assert.That(
                PendingChange.CombineStatus(nodeStatus, textStatus, propertyStatus, treeConflict, null),
                Is.EqualTo(expected));
        }

        [Test]
        public void CombineStatusRejectsInvalidNodeStatus()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                PendingChange.CombineStatus(
                    SvnStatus.Zero,
                    SvnStatus.Normal,
                    SvnStatus.Normal,
                    false,
                    null));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                PendingChange.CombineStatus(
                    SvnStatus.Merged,
                    SvnStatus.Normal,
                    SvnStatus.Normal,
                    false,
                    null));
        }

        [Test]
        public void CombineStatusRejectsInvalidPropertyStatus()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                PendingChange.CombineStatus(
                    SvnStatus.Normal,
                    SvnStatus.Normal,
                    SvnStatus.Zero,
                    false,
                    null));
        }
    }
}
