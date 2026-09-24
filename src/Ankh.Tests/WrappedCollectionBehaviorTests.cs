using System;
using System.Collections.Generic;
using System.Linq;
using Ankh.Collections;
using NUnit.Framework;

namespace Ankh.Tests
{
    [TestFixture]
    public class WrappedCollectionBehaviorTests
    {
        sealed class Row
        {
            public Row(string value) { Value = value; }
            public string Value { get; private set; }
        }

        sealed class Rows : WrapNotifyCollection<string, Row>
        {
            public Rows(INotifyCollection<string> source) : base(source) { }
            protected override Row GetWrapItem(string value) { return new Row(value); }
        }

        sealed class Names : KeyedNotifyCollection<string, string>
        {
            public Names() : base(StringComparer.OrdinalIgnoreCase) { }
            protected override string GetKeyForItem(string value) { return value; }
            public void Close() { Dispose(true); }
        }

        sealed class KeyedRows : KeyedWrapNotifyCollection<string, string, Row>
        {
            public KeyedRows(IKeyedNotifyCollection<string, string> source) : base(source) { }
            protected override Row GetWrapItem(string value) { return new Row(value); }
            protected override string GetKeyForItem(Row value) { return value.Value; }
        }

        [Test]
        public void ReplacementNotificationsIdentifyOldAndNewItemsForBothCollectionTypes()
        {
            foreach (INotifyCollection<string> source in new INotifyCollection<string>[] {
                new NotifyCollection<string>(), new Names() })
            {
                source.Add("old");
                CollectionChangedEventArgs<string> change = null;
                source.CollectionChanged += (sender, e) => change = e;
                source[0] = "new";
                Assert.That(change.Action, Is.EqualTo(CollectionChange.Replace));
                Assert.That(change.OldItems, Is.EqualTo(new[] { "old" }));
                Assert.That(change.NewItems, Is.EqualTo(new[] { "new" }));
                Assert.That(change.OldStartingIndex, Is.Zero);
                Assert.That(change.NewStartingIndex, Is.Zero);
                Assert.That(source[0], Is.EqualTo("new"));
            }
        }

        [Test]
        public void WrapperTracksInsertReplaceMoveRemoveAndResetWithCorrectValues()
        {
            var source = new NotifyCollection<string> { "alpha", "beta" };
            using (var rows = new Rows(source))
            {
                var actions = new List<CollectionChange>();
                rows.CollectionChanged += (sender, e) => actions.Add(e.Action);
                Assert.That(rows.Select(r => r.Value), Is.EqualTo(new[] { "alpha", "beta" }));
                source.Insert(1, "middle");
                Assert.That(rows.Select(r => r.Value), Is.EqualTo(new[] { "alpha", "middle", "beta" }));
                source[1] = "replacement";
                Assert.That(rows[1].Value, Is.EqualTo("replacement"));
                Row moved = rows[2];
                source.Move(2, 0);
                Assert.That(rows[0], Is.SameAs(moved));
                Assert.That(rows.Select(r => r.Value), Is.EqualTo(new[] { "beta", "alpha", "replacement" }));
                source.RemoveAt(1);
                Assert.That(rows.Select(r => r.Value), Is.EqualTo(new[] { "beta", "replacement" }));
                source.Clear();
                Assert.That(rows, Is.Empty);
                Assert.That(actions, Is.EqualTo(new[] { CollectionChange.Add, CollectionChange.Replace,
                    CollectionChange.Move, CollectionChange.Remove, CollectionChange.Reset }));
                Assert.That(rows.GetWrappedCollection(), Is.SameAs(source));
                Assert.That(((IWrapCollectionWithNotify<string, Row>)rows).GetWrapItem("new").Value, Is.EqualTo("new"));
            }
        }

        [Test]
        public void WrapperBatchPublishesFinalSnapshotAndDisposalUnsubscribes()
        {
            var source = new NotifyCollection<string>();
            var rows = new Rows(source);
            var snapshots = new List<string[]>();
            rows.CollectionChanged += (sender, e) => snapshots.Add(rows.Select(r => r.Value).ToArray());
            using (rows.BatchUpdate())
            {
                source.Add("one");
                source.Add("two");
                Assert.That(snapshots, Is.Empty);
            }
            Assert.That(snapshots.Count, Is.EqualTo(1));
            Assert.That(snapshots[0], Is.EqualTo(new[] { "one", "two" }));
            using (source.BatchUpdate())
            {
                source.Clear();
                source.Add("three");
                source.Add("four");
            }
            Assert.That(rows.Select(r => r.Value), Is.EqualTo(new[] { "three", "four" }));
            rows.Dispose();
            int before = snapshots.Count;
            source.Add("after disposal");
            Assert.That(snapshots.Count, Is.EqualTo(before));
            Assert.That(rows.Select(r => r.Value), Is.EqualTo(new[] { "three", "four" }));
        }

        [Test]
        public void NullWrapperSourceIsRejected()
        {
            Assert.Throws<ArgumentNullException>(() => new Rows(null));
        }

        [Test]
        public void KeyedWrapperTracksChangesAndPreservesIdentityAcrossBatchedReset()
        {
            var source = new Names { "alpha", "beta" };
            using (var rows = new KeyedRows(source))
            {
                Row alpha = rows["ALPHA"];
                Assert.That(alpha.Value, Is.EqualTo("alpha"));
                source.Insert(1, "middle");
                Assert.That(rows["MIDDLE"].Value, Is.EqualTo("middle"));
                source[1] = "replacement";
                Assert.That(rows.Contains("middle"), Is.False);
                Assert.That(rows["replacement"].Value, Is.EqualTo("replacement"));
                Row beta = rows["beta"];
                source.Move(2, 0);
                Assert.That(rows[0], Is.SameAs(beta));
                source.Remove("replacement");
                Assert.That(rows.Select(r => r.Value), Is.EqualTo(new[] { "beta", "alpha" }));
                using (source.BatchUpdate())
                {
                    source.Clear();
                    source.Add("ALPHA");
                    source.Add("gamma");
                }
                Assert.That(rows[0], Is.SameAs(alpha), "Stable keys must retain the UI wrapper across a reset.");
                Assert.That(rows["gamma"].Value, Is.EqualTo("gamma"));
                Assert.That(rows.Contains("beta"), Is.False);
                var exposed = rows.GetWrappedCollection();
                Assert.That(rows.GetWrappedCollection(), Is.SameAs(exposed));
                Assert.That(exposed, Is.EqualTo(source));
                Assert.Throws<NotSupportedException>(() => exposed.Add("forbidden"));
                source.Clear();
                Assert.That(rows, Is.Empty);
            }
        }

        [Test]
        public void SourceDisposalStopsKeyedWrapperNotifications()
        {
            var source = new Names { "one" };
            using (var rows = new KeyedRows(source))
            {
                int notifications = 0;
                rows.CollectionChanged += (sender, e) => notifications++;
                source.Close();
                source.Add("two");
                Assert.That(notifications, Is.Zero);
                Assert.That(rows.Select(r => r.Value), Is.EqualTo(new[] { "one" }));
            }
        }
    }
}
