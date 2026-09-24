using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Ankh;
using Ankh.Collections;
using Ankh.Commands;
using Ankh.Scc;
using Microsoft.VisualStudio.OLE.Interop;
using NUnit.Framework;

namespace Ankh.Tests
{
    [TestFixture]
    public class CoreCollectionContractTests
    {
        [Test]
        public void NotifyCollection_ReportsAddReplaceMoveRemoveAndClear()
        {
            var collection = new NotifyCollection<string>();
            var changes = new List<CollectionChange>();
            var properties = new List<string>();

            collection.CollectionChanged += (sender, e) => changes.Add(e.Action);
            collection.PropertyChanged += (sender, e) => properties.Add(e.PropertyName);

            collection.Add("one");
            collection[0] = "two";
            collection.Add("three");
            collection.Move(1, 0);
            collection.RemoveAt(1);
            collection.Clear();

            Assert.That(changes, Is.EqualTo(new[]
            {
                CollectionChange.Add,
                CollectionChange.Replace,
                CollectionChange.Add,
                CollectionChange.Move,
                CollectionChange.Remove,
                CollectionChange.Reset
            }));
            Assert.That(properties, Does.Contain("Count"));
            Assert.That(properties, Does.Contain("Item[]"));
            Assert.That(collection.ToArray(), Is.Empty);
        }

        [Test]
        public void NotifyCollection_BatchUpdate_CoalescesMultipleChangesToReset()
        {
            var collection = new NotifyCollection<string>();
            var changes = new List<CollectionChange>();

            collection.CollectionChanged += (sender, e) => changes.Add(e.Action);

            using (collection.BatchUpdate())
            {
                collection.Add("one");
                collection.Add("two");
                collection.Remove("one");
            }

            Assert.That(changes, Is.EqualTo(new[] { CollectionChange.Reset }));
            Assert.That(collection.ToArray(), Is.EqualTo(new[] { "two" }));
        }

        [Test]
        public void NotifyCollection_BatchUpdate_PreservesSingleChange()
        {
            var collection = new NotifyCollection<string>();
            CollectionChangedEventArgs<string> observed = null;

            collection.CollectionChanged += (sender, e) => observed = e;

            using (collection.BatchUpdate())
                collection.Add("one");

            Assert.That(observed, Is.Not.Null);
            Assert.That(observed.Action, Is.EqualTo(CollectionChange.Add));
        }

        [Test]
        public void ReadOnlyNotifyCollection_ForwardsInnerNotifications()
        {
            var inner = new NotifyCollection<string>();
            var readOnly = new ReadOnlyNotifyCollection<string>(inner);
            int collectionEvents = 0;
            int propertyEvents = 0;

            EventHandler<CollectionChangedEventArgs<string>> collectionHandler =
                (sender, e) => collectionEvents++;
            PropertyChangedEventHandler propertyHandler =
                (sender, e) => propertyEvents++;

            readOnly.CollectionChanged += collectionHandler;
            readOnly.PropertyChanged += propertyHandler;

            inner.Add("one");

            Assert.That(collectionEvents, Is.EqualTo(1));
            Assert.That(propertyEvents, Is.EqualTo(2));
            Assert.That(readOnly.ToArray(), Is.EqualTo(new[] { "one" }));
            Assert.That(readOnly.BatchUpdate(), Is.Null);

            readOnly.CollectionChanged -= collectionHandler;
            readOnly.PropertyChanged -= propertyHandler;
        }

        [Test]
        public void HybridCollection_UniqueAddRange_UsesConfiguredComparer()
        {
            var collection = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            collection.AddRange(new[] { "one", "two" });
            collection.UniqueAddRange(new[] { "ONE", "three" });

            Assert.That(collection, Is.EqualTo(new[] { "one", "two", "three" }));
            Assert.Throws<ArgumentNullException>(() => collection.AddRange(null));
            Assert.Throws<ArgumentNullException>(() => collection.UniqueAddRange(null));
        }

        [Test]
        public void SvnItemsEventArgs_RejectsNullAndExposesReadOnlyList()
        {
            Assert.Throws<ArgumentNullException>(() => new SvnItemsEventArgs(null));

            var args = new SvnItemsEventArgs(new List<SvnItem>());

            Assert.That(args.ChangedItems, Is.Empty);
            Assert.Throws<NotSupportedException>(() => ((IList<SvnItem>)args.ChangedItems).Add(null));
        }


        [Test]
        public void KeyedWrapNotifyCollectionTracksSourceMutations()
        {
            var source = new TestInnerCollection();
            source.Add(new TestInner("A", "one"));
            source.Add(new TestInner("B", "two"));

            var context = new object();
            var wrapped = new TestWrappedCollection(source, context);
            var changes = new List<CollectionChange>();
            wrapped.CollectionChanged += (sender, e) => changes.Add(e.Action);

            source.Add(new TestInner("C", "three"));
            source[0] = new TestInner("A", "updated");
            source.Move(2, 0);
            source.RemoveAt(1);

            TestWrapped c;
            Assert.That(wrapped.TryGetValue("c", out c), Is.True);

            Assert.Multiple(() =>
            {
                CollectionAssert.AreEqual(
                    new[] { CollectionChange.Add, CollectionChange.Replace, CollectionChange.Move, CollectionChange.Remove },
                    changes);
                CollectionAssert.AreEqual(new[] { "C", "B" }, wrapped.Select(x => x.Key).ToArray());
                Assert.That(c.Value, Is.EqualTo("three"));
                Assert.That(wrapped.StoredContext, Is.SameAs(context));
                Assert.That(wrapped.GetWrappedCollection(), Is.Not.SameAs(source));
                Assert.That(wrapped.GetWrappedCollection().Count, Is.EqualTo(source.Count));
            });

            wrapped.Dispose();
        }

        [Test]
        public void KeyedWrapNotifyCollectionResetPreservesExistingWrappersByKey()
        {
            var source = new TestInnerCollection();
            source.Add(new TestInner("A", "one"));
            source.Add(new TestInner("B", "two"));

            var wrapped = new TestWrappedCollection(source, null);
            TestWrapped a = wrapped["A"];
            TestWrapped b = wrapped["B"];
            var changes = new List<CollectionChange>();
            wrapped.CollectionChanged += (sender, e) => changes.Add(e.Action);

            using (source.BatchUpdate())
            {
                source.Move(1, 0);
                source.Add(new TestInner("C", "three"));
            }

            Assert.Multiple(() =>
            {
                CollectionAssert.AreEqual(new[] { CollectionChange.Reset }, changes);
                CollectionAssert.AreEqual(new[] { "B", "A", "C" }, wrapped.Select(x => x.Key).ToArray());
                Assert.That(wrapped[0], Is.SameAs(b));
                Assert.That(wrapped[1], Is.SameAs(a));
                Assert.That(wrapped[2].Value, Is.EqualTo("three"));
            });

            wrapped.Dispose();
        }

        [TestCase(true, false, true)]
        [TestCase(false, true, false)]
        [TestCase(false, false, true)]
        public void CommandEventArgs_ShouldPromptHonorsExplicitPromptPolicy(
            bool promptUser,
            bool dontPrompt,
            bool expected)
        {
            var args = new CommandEventArgs(
                default(AnkhCommand),
                null,
                "argument",
                promptUser,
                dontPrompt);

            Assert.That(args.Argument, Is.EqualTo("argument"));
            Assert.That(args.PromptUser, Is.EqualTo(promptUser));
            Assert.That(args.DontPrompt, Is.EqualTo(dontPrompt));
            Assert.That(args.ShouldPrompt(false), Is.EqualTo(expected));

            args.Result = "result";
            Assert.That(args.Result, Is.EqualTo("result"));
        }

        [Test]
        public void CommandAttributesPreserveRoutingMetadataAndEnforceRangeRules()
        {
            var attribute = new CommandAttribute(AnkhCommand.ListViewSort0, AnkhCommandContext.Global)
            {
                LastCommand = (AnkhCommand)((int)AnkhCommand.ListViewSort0 + 2),
                CommandTarget = CommandTarget.SelectedPathsRecursive,
                HideWhenDisabled = false,
                ArgumentDefinition = "d|p *"
            };

            MethodInfo getAllCommands = typeof(CommandAttribute).GetMethod(
                "GetAllCommands",
                BindingFlags.Instance | BindingFlags.NonPublic);

            var commands = ((IEnumerable<AnkhCommand>)getAllCommands.Invoke(attribute, null)).ToArray();

            Assert.Multiple(() =>
            {
                Assert.That(attribute.Command, Is.EqualTo(AnkhCommand.ListViewSort0));
                Assert.That(attribute.Context, Is.EqualTo(AnkhCommandContext.Global));
                Assert.That(attribute.CommandTarget, Is.EqualTo(CommandTarget.SelectedPathsRecursive));
                Assert.That(attribute.HideWhenDisabled, Is.False);
                Assert.That(attribute.ArgumentDefinition, Is.EqualTo("d|p *"));
                Assert.That(commands, Is.EqualTo(new[]
                {
                    AnkhCommand.ListViewSort0,
                    (AnkhCommand)((int)AnkhCommand.ListViewSort0 + 1),
                    (AnkhCommand)((int)AnkhCommand.ListViewSort0 + 2)
                }));
            });

            var single = new CommandAttribute(AnkhCommand.Refresh);
            var singleCommands = ((IEnumerable<AnkhCommand>)getAllCommands.Invoke(single, null)).ToArray();
            Assert.That(singleCommands, Is.EqualTo(new[] { AnkhCommand.Refresh }));

            var reversed = new CommandAttribute(AnkhCommand.ListViewSort0)
            {
                LastCommand = (AnkhCommand)((int)AnkhCommand.ListViewSort0 - 1)
            };
            Assert.Throws<InvalidOperationException>(() =>
                ((IEnumerable<AnkhCommand>)getAllCommands.Invoke(reversed, null)).ToArray());

            var tooWide = new CommandAttribute(AnkhCommand.ListViewSort0)
            {
                LastCommand = (AnkhCommand)((int)AnkhCommand.ListViewSort0 + 257)
            };
            Assert.Throws<InvalidOperationException>(() =>
                ((IEnumerable<AnkhCommand>)getAllCommands.Invoke(tooWide, null)).ToArray());
        }

        [Test]
        public void CommandAttributesExposeSvnAvailabilityAndProtectAlwaysAvailableState()
        {
            var svn = new SvnCommandAttribute(AnkhCommand.Refresh);
            var scc = new SccCommandAttribute(AnkhCommand.Refresh, AnkhCommandContext.Global);
            var always = new CommandAttribute(AnkhCommand.Refresh);

            Assert.Multiple(() =>
            {
                Assert.That(svn.Availability, Is.EqualTo(CommandAvailability.SvnActive));
                Assert.That(scc.Availability, Is.EqualTo(CommandAvailability.SvnActive));
                Assert.That(scc.Context, Is.EqualTo(AnkhCommandContext.Global));
                Assert.That(always.HideWhenDisabled, Is.True);
                Assert.That(always.AlwaysAvailable, Is.False);
            });

            always.AlwaysAvailable = true;

            Assert.That(always.AlwaysAvailable, Is.True);
            Assert.Throws<InvalidOperationException>(() => always.AlwaysAvailable = false);
        }

        [Test]
        public void CommandMapItemDispatchesHandlerBeforeExecuteEventAndSharesArguments()
        {
            var order = new List<string>();
            var map = new TestCommandMapItem(AnkhCommand.Refresh);
            var handler = new RecordingCommandHandler(order);
            map.ICommand = handler;

            var args = new CommandEventArgs(AnkhCommand.Refresh, null);
            map.Execute += delegate(object sender, CommandEventArgs e)
            {
                order.Add("event");
                Assert.That(sender, Is.SameAs(map));
                Assert.That(e, Is.SameAs(args));
                Assert.That(e.Result, Is.EqualTo("handler"));
                e.Result = "event";
            };

            Assert.That(map.IsHandled, Is.True);

            map.RaiseExecute(args);

            Assert.Multiple(() =>
            {
                CollectionAssert.AreEqual(new[] { "handler", "event" }, order);
                Assert.That(handler.ExecuteCount, Is.EqualTo(1));
                Assert.That(args.Result, Is.EqualTo("event"));
                Assert.That(map.Command, Is.EqualTo(AnkhCommand.Refresh));
            });
        }

        [Test]
        public void CommandMapItemDispatchesUpdateHandlerBeforeUpdateEvent()
        {
            var order = new List<string>();
            var map = new TestCommandMapItem(AnkhCommand.Refresh);
            var handler = new RecordingCommandHandler(order);
            map.ICommand = handler;

            var args = new CommandUpdateEventArgs(AnkhCommand.Refresh, null);
            map.Update += delegate(object sender, CommandUpdateEventArgs e)
            {
                order.Add("event");
                Assert.That(sender, Is.SameAs(map));
                Assert.That(e, Is.SameAs(args));
                Assert.That(e.Enabled, Is.False);
                e.Visible = false;
            };

            map.RaiseUpdate(args);

            Assert.Multiple(() =>
            {
                CollectionAssert.AreEqual(new[] { "handler", "event" }, order);
                Assert.That(handler.UpdateCount, Is.EqualTo(1));
                Assert.That(args.Enabled, Is.False);
                Assert.That(args.Visible, Is.False);
            });
        }

        [Test]
        public void CommandMapItemHandledStateTracksExecutableSources()
        {
            var map = new TestCommandMapItem(AnkhCommand.Refresh);

            Assert.That(map.IsHandled, Is.False);

            EventHandler<CommandUpdateEventArgs> update = delegate { };
            map.Update += update;
            Assert.That(map.IsHandled, Is.False,
                "Update-only observers do not provide an executable command.");

            EventHandler<CommandEventArgs> execute = delegate { };
            map.Execute += execute;
            Assert.That(map.IsHandled, Is.True);

            map.Execute -= execute;
            Assert.That(map.IsHandled, Is.False);

            map.ICommand = new RecordingCommandHandler(new List<string>());
            Assert.That(map.IsHandled, Is.True);

            map.ICommand = null;
            map.Update -= update;
            Assert.That(map.IsHandled, Is.False);
        }

        [Test]
        public void CommandUpdateEventArgs_DefaultStateIsEnabledAndVisible()
        {
            var args = new CommandUpdateEventArgs(default(AnkhCommand), null, TextQueryType.Name);
            OLECMDF flags = 0;

            args.UpdateFlags(ref flags);

            Assert.Multiple(() =>
            {
                Assert.That(flags, Is.EqualTo(OLECMDF.OLECMDF_ENABLED));
                Assert.That(args.Enabled, Is.True);
                Assert.That(args.Visible, Is.True);
                Assert.That(args.Checked, Is.False);
                Assert.That(args.Ninched, Is.False);
                Assert.That(args.HideOnContextMenu, Is.False);
                Assert.That(args.DynamicMenuEnd, Is.False);
                Assert.That(args.Text, Is.Null);
                Assert.That(args.TextQueryType, Is.EqualTo(TextQueryType.Name));
            });
        }

        [Test]
        public void CommandUpdateEventArgs_MapsStateToOleCommandFlags()
        {
            var args = new CommandUpdateEventArgs(default(AnkhCommand), null, TextQueryType.Status)
            {
                Enabled = true,
                Visible = false,
                Checked = true,
                Ninched = true,
                HideOnContextMenu = true,
                DynamicMenuEnd = true,
                Text = "status"
            };

            OLECMDF flags = 0;
            args.UpdateFlags(ref flags);

            Assert.That((flags & OLECMDF.OLECMDF_ENABLED) != 0, Is.True);
            Assert.That((flags & OLECMDF.OLECMDF_LATCHED) != 0, Is.True);
            Assert.That((flags & OLECMDF.OLECMDF_NINCHED) != 0, Is.True);
            Assert.That((flags & OLECMDF.OLECMDF_INVISIBLE) != 0, Is.True);
            Assert.That((flags & OLECMDF.OLECMDF_DEFHIDEONCTXTMENU) != 0, Is.True);
            Assert.That(args.TextQueryType, Is.EqualTo(TextQueryType.Status));
            Assert.That(args.DynamicMenuEnd, Is.True);
            Assert.That(args.Text, Is.EqualTo("status"));
        }
        sealed class TestInner
        {
            public TestInner(string key, string value)
            {
                Key = key;
                Value = value;
            }

            public string Key { get; private set; }
            public string Value { get; private set; }
        }

        sealed class TestWrapped
        {
            readonly TestInner _inner;

            public TestWrapped(TestInner inner)
            {
                _inner = inner;
            }

            public string Key { get { return _inner.Key; } }
            public string Value { get { return _inner.Value; } }
        }

        sealed class TestInnerCollection : KeyedNotifyCollection<string, TestInner>
        {
            public TestInnerCollection()
                : base(StringComparer.OrdinalIgnoreCase)
            {
            }

            protected override string GetKeyForItem(TestInner item)
            {
                return item.Key;
            }
        }

        sealed class TestWrappedCollection : KeyedWrapNotifyCollection<string, TestInner, TestWrapped>
        {
            public TestWrappedCollection(IKeyedNotifyCollection<string, TestInner> source, object context)
                : base(source, context)
            {
            }

            public object StoredContext
            {
                get { return Context; }
            }

            protected override string GetKeyForItem(TestWrapped item)
            {
                return item.Key;
            }

            protected override string GetKeyForItem(TestInner inner)
            {
                return inner.Key;
            }

            protected override TestWrapped GetWrapItem(TestInner inner)
            {
                return new TestWrapped(inner);
            }
        }

        sealed class TestCommandMapItem : CommandMapItem
        {
            public TestCommandMapItem(AnkhCommand command)
                : base(command)
            {
            }

            public void RaiseExecute(CommandEventArgs e)
            {
                OnExecute(e);
            }

            public void RaiseUpdate(CommandUpdateEventArgs e)
            {
                OnUpdate(e);
            }
        }

        sealed class RecordingCommandHandler : ICommandHandler
        {
            readonly IList<string> _order;

            public RecordingCommandHandler(IList<string> order)
            {
                _order = order;
            }

            public int ExecuteCount { get; private set; }
            public int UpdateCount { get; private set; }

            public void OnExecute(CommandEventArgs e)
            {
                ExecuteCount++;
                _order.Add("handler");
                e.Result = "handler";
            }

            public void OnUpdate(CommandUpdateEventArgs e)
            {
                UpdateCount++;
                _order.Add("handler");
                e.Enabled = false;
            }
        }

    }
}
