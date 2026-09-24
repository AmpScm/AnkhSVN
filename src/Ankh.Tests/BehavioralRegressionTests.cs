using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Ankh;
using Ankh.Collections;
using Ankh.ContextServices;
using Ankh.UI.WizardFramework;
using NUnit.Framework;

namespace Ankh.Tests
{
    [TestFixture]
    public class BehavioralRegressionTests
    {
        sealed class KeyedStringCollection : KeyedNotifyCollection<string, string>
        {
            public KeyedStringCollection()
                : base(StringComparer.OrdinalIgnoreCase, 2)
            {
            }

            protected override string GetKeyForItem(string item)
            {
                return item;
            }
        }

        sealed class TrackingWizardPage : WizardPage
        {
            public int BeforeAddCount { get; private set; }
            public int AfterAddCount { get; private set; }
            public int BeforeRemoveCount { get; private set; }
            public int AfterRemoveCount { get; private set; }

            protected override void OnBeforeAdd(WizardPageCollection collection)
            {
                BeforeAddCount++;
                base.OnBeforeAdd(collection);
            }

            protected override void OnAfterAdd(WizardPageCollection collection)
            {
                AfterAddCount++;
                base.OnAfterAdd(collection);
            }

            protected override void OnBeforeRemove(WizardPageCollection collection)
            {
                BeforeRemoveCount++;
                base.OnBeforeRemove(collection);
            }

            protected override void OnAfterRemove(WizardPageCollection collection)
            {
                AfterRemoveCount++;
                base.OnAfterRemove(collection);
            }
        }

        [Test]
        public void CollectionChangedEventArgs_RejectInvalidActionAndNullPayloads()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() => new CollectionChangedEventArgs(CollectionChange.Add));
                Assert.Throws<ArgumentException>(() => new CollectionChangedEventArgs(CollectionChange.Replace, "x", 0));
                Assert.Throws<ArgumentNullException>(() => new CollectionChangedEventArgs(CollectionChange.Add, null, 0));
                Assert.Throws<ArgumentException>(() => new CollectionChangedEventArgs(CollectionChange.Add, "new", "old", 0));
                Assert.Throws<ArgumentNullException>(() => new CollectionChangedEventArgs(CollectionChange.Replace, null, "old", 0));
                Assert.Throws<ArgumentNullException>(() => new CollectionChangedEventArgs(CollectionChange.Replace, "new", null, 0));
                Assert.Throws<ArgumentNullException>(() => new CollectionChangedEventArgs(CollectionChange.Replace, (object[])null, new object[0], 0));
                Assert.Throws<ArgumentNullException>(() => new CollectionChangedEventArgs(CollectionChange.Replace, new object[0], (object[])null, 0));
                Assert.Throws<ArgumentException>(() => new CollectionChangedEventArgs(CollectionChange.Move, "x", -1, 0));
                Assert.Throws<ArgumentException>(() => new CollectionChangedEventArgs(CollectionChange.Move, "x", 0, -1));
                Assert.Throws<ArgumentNullException>(() => new CollectionChangedEventArgs(CollectionChange.Move, null, 0, 1));
                Assert.Throws<ArgumentNullException>(() => new CollectionChangedEventArgs(CollectionChange.Move, (object[])null, 0, 1));
            });
        }

        [Test]
        public void CollectionChangedEventArgs_ExposeIndicesAndPayloadsForEveryChangeType()
        {
            var add = new CollectionChangedEventArgs<string>(CollectionChange.Add, "new", 2);
            var remove = new CollectionChangedEventArgs<string>(CollectionChange.Remove, "old", 3);
            var replace = new CollectionChangedEventArgs<string>(CollectionChange.Replace, "new", "old", 4);
            var move = new CollectionChangedEventArgs<string>(CollectionChange.Move, "item", 5, 1);
            var reset = new CollectionChangedEventArgs<string>(CollectionChange.Reset);

            Assert.Multiple(() =>
            {
                Assert.That(add.NewItems, Is.EqualTo(new[] { "new" }));
                Assert.That(add.NewStartingIndex, Is.EqualTo(2));
                Assert.That(add.OldItems, Is.Null);

                Assert.That(remove.OldItems, Is.EqualTo(new[] { "old" }));
                Assert.That(remove.OldStartingIndex, Is.EqualTo(3));
                Assert.That(remove.NewItems, Is.Null);

                Assert.That(replace.NewItems, Is.EqualTo(new[] { "new" }));
                Assert.That(replace.OldItems, Is.EqualTo(new[] { "old" }));
                Assert.That(replace.NewStartingIndex, Is.EqualTo(4));
                Assert.That(replace.OldStartingIndex, Is.EqualTo(4));

                Assert.That(move.NewItems, Is.EqualTo(new[] { "item" }));
                Assert.That(move.OldItems, Is.EqualTo(new[] { "item" }));
                Assert.That(move.NewStartingIndex, Is.EqualTo(5));
                Assert.That(move.OldStartingIndex, Is.EqualTo(1));

                Assert.That(reset.NewStartingIndex, Is.EqualTo(-1));
                Assert.That(reset.OldStartingIndex, Is.EqualTo(-1));
            });
        }

        [Test]
        public void KeyedNotifyCollection_ExercisesLookupMutationBatchAndReentrancyContracts()
        {
            var collection = new KeyedStringCollection();
            var actions = new List<CollectionChange>();
            var properties = new List<string>();

            collection.CollectionChanged += (sender, e) => actions.Add(e.Action);
            collection.PropertyChanged += (sender, e) => properties.Add(e.PropertyName);

            collection.Add("one");
            collection.Add("two");
            collection.Add("three");

            Assert.That(collection.TryGetValue("TWO", out string two), Is.True);
            Assert.That(two, Is.EqualTo("two"));
            Assert.That(collection.TryGetValue("missing", out string missing), Is.False);
            Assert.That(missing, Is.Null);

            collection[1] = "TWO";
            collection.Move(2, 0);
            collection.Remove("one");

            using (collection.BatchUpdate())
            {
                collection.Add("four");
                collection.Add("five");
            }

            Assert.That(actions, Does.Contain(CollectionChange.Add));
            Assert.That(actions, Does.Contain(CollectionChange.Replace));
            Assert.That(actions, Does.Contain(CollectionChange.Move));
            Assert.That(actions, Does.Contain(CollectionChange.Remove));
            Assert.That(actions.Last(), Is.EqualTo(CollectionChange.Reset));
            Assert.That(properties, Does.Contain("Count"));
            Assert.That(properties, Does.Contain("Item[]"));

            EventHandler<CollectionChangedEventArgs<string>> reentrant = null;
            reentrant = (sender, e) => collection.Add("nested");
            collection.CollectionChanged += reentrant;
            Assert.Throws<InvalidOperationException>(() => collection.Add("outer"));
            collection.CollectionChanged -= reentrant;

            collection.Clear();
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void GuidUtils_CreateGuid_IsDeterministicVersionFiveAndRejectsInvalidInput()
        {
            var ns = new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8");
            byte[] name = System.Text.Encoding.UTF8.GetBytes("ankhsvn");

            Guid first = GuidUtils.CreateGuid(ns, name);
            Guid second = GuidUtils.CreateGuid(ns, name);
            Guid different = GuidUtils.CreateGuid(ns, System.Text.Encoding.UTF8.GetBytes("AnkhSVN"));

            Assert.Multiple(() =>
            {
                Assert.That(first, Is.EqualTo(second));
                Assert.That(first, Is.Not.EqualTo(different));
                Assert.That(first.ToByteArray()[7] >> 4, Is.EqualTo(5));
                Assert.Throws<ArgumentNullException>(() => GuidUtils.CreateGuid(Guid.Empty, name));
                Assert.Throws<ArgumentNullException>(() => GuidUtils.CreateGuid(ns, null));
            });
        }


        [Test]
        public void WizardPage_DetachedStateUsesSafeDefaultsAndExplicitOverrides()
        {
            using (var page = new TrackingWizardPage())
            using (var previous = new TrackingWizardPage())
            {
                Assert.Multiple(() =>
                {
                    Assert.That(page.Name, Is.EqualTo(typeof(TrackingWizardPage).FullName));
                    Assert.That(page.Context, Is.Null);
                    Assert.That(page.NextPage, Is.Null);
                    Assert.That(page.PreviousPage, Is.Null);
                    Assert.That(page.Description, Is.EqualTo(string.Empty));
                    Assert.That(page.MessageText, Is.EqualTo(string.Empty));
                    Assert.That(page.MessageType, Is.EqualTo(WizardMessage.MessageType.None));
                    Assert.That(page.Image, Is.Null);
                    Assert.That(page.IsPageComplete, Is.False);
                    Assert.That(page.CanFlipToNextPage, Is.False);
                });

                page.Name = "Custom";
                page.Description = "Description";
                page.Message = new WizardMessage("Warning", WizardMessage.MessageType.Warning);
                page.PreviousPage = previous;
                page.IsPageComplete = true;

                Assert.Multiple(() =>
                {
                    Assert.That(page.Name, Is.EqualTo("Custom"));
                    Assert.That(page.Description, Is.EqualTo("Description"));
                    Assert.That(page.Message.Message, Is.EqualTo("Warning"));
                    Assert.That(page.Message.Type, Is.EqualTo(WizardMessage.MessageType.Warning));
                    Assert.That(page.PreviousPage, Is.SameAs(previous));
                    Assert.That(page.IsPageComplete, Is.True);
                    Assert.That(page.CanFlipToNextPage, Is.False);
                });

                page.Message = null;

                Assert.Multiple(() =>
                {
                    Assert.That(page.MessageText, Is.EqualTo(string.Empty));
                    Assert.That(page.MessageType, Is.EqualTo(WizardMessage.MessageType.None));
                });
            }
        }

        [TestCase(WizardMessage.MessageType.None)]
        [TestCase(WizardMessage.MessageType.Information)]
        [TestCase(WizardMessage.MessageType.Warning)]
        [TestCase(WizardMessage.MessageType.Error)]
        public void WizardMessage_PreservesMessageAndType(WizardMessage.MessageType type)
        {
            var message = new WizardMessage("message", type);

            Assert.That(message.Message, Is.EqualTo("message"));
            Assert.That(message.Type, Is.EqualTo(type));
        }

        [Test]
        public void WizardPageCollection_Remove_CallsBalancedLifecycleHooksAndDetachesPage()
        {
            Exception failure = null;

            Thread thread = new Thread(() =>
            {
                try
                {
                    using (var wizard = new Wizard())
                    using (var page = new TrackingWizardPage())
                    {
                        wizard.Pages.Add(page);

                        Assert.That(page.BeforeAddCount, Is.EqualTo(1));
                        Assert.That(page.AfterAddCount, Is.EqualTo(1));
                        Assert.That(page.Wizard, Is.SameAs(wizard));

                        wizard.Pages.Remove(page);

                        Assert.That(page.BeforeRemoveCount, Is.EqualTo(1));
                        Assert.That(page.AfterRemoveCount, Is.EqualTo(1));
                        Assert.That(page.Wizard, Is.Null);
                    }
                }
                catch (Exception ex)
                {
                    failure = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (failure != null)
                Assert.Fail(failure.ToString());
        }
    }
}
