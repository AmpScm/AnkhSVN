using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Ankh;
using Ankh.UI;
using Ankh.UI.VSSelectionControls;
using NUnit.Framework;

namespace Ankh.Tests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class SmartListViewBehaviorTests
    {
        [Test]
        public void SmartColumnComparisonCoversNullPlainSmartCustomAndReversePaths()
        {
            using (var view = new TestSmartListView())
            {
                var column = new SmartColumn(view, "Name", "&Name", 120);
                view.Columns.Add(column);
                view.AllColumns.Add(column);

                var alpha = new ListViewItem("alpha");
                var beta = new ListViewItem("Beta");
                var comparer = (ISmartValueComparer)column;

                Assert.Multiple(() =>
                {
                    Assert.That(comparer.Compare(null, beta, false), Is.LessThan(0));
                    Assert.That(comparer.Compare(alpha, null, false), Is.GreaterThan(0));
                    Assert.That(comparer.Compare(alpha, beta, false), Is.LessThan(0));
                });

                var smartA = new SmartListViewItem(view);
                var smartB = new SmartListViewItem(view);
                smartA.SetValues("zeta");
                smartB.SetValues("eta");

                Assert.That(comparer.Compare(smartA, smartB, false), Is.GreaterThan(0));

                column.ReverseSort = true;
                Assert.That(comparer.Compare(smartA, smartB, true), Is.LessThan(0));
                Assert.That(comparer.Compare(smartA, smartB, false), Is.GreaterThan(0));

                column.Sorter = new ConstantComparer(-7);
                Assert.That(comparer.Compare(alpha, beta, false), Is.EqualTo(-7));
                Assert.That(comparer.Compare(alpha, beta, true), Is.EqualTo(7));

                column.MenuText = null;
                column.Sortable = false;
                column.Hideable = false;
                column.Moveable = false;
                column.Dragable = false;
                column.Groupable = true;

                Assert.Multiple(() =>
                {
                    Assert.That(column.MenuText, Is.Empty);
                    Assert.That(column.Sortable, Is.False);
                    Assert.That(column.Hideable, Is.False);
                    Assert.That(column.Moveable, Is.False);
                    Assert.That(column.Dragable, Is.False);
                    Assert.That(column.Groupable, Is.True);
                    Assert.That(column.AllColumnsIndex, Is.Zero);
                });
            }
        }

        [Test]
        public void SmartListViewItemSupportsHiddenColumnsAndExactArrayBoundary()
        {
            using (var view = new TestSmartListView())
            {
                var visible = new SmartColumn(view, "Visible", 100, "visible");
                var hidden = new SmartColumn(view, "Hidden", 100, "hidden");

                view.Columns.Add(visible);
                view.AllColumns.Add(visible);
                view.AllColumns.Add(hidden);

                var item = new SmartListViewItem(view);
                view.Items.Add(item);

                item.SetValues("one");
                item.SetValue(1, "two");

                Assert.Multiple(() =>
                {
                    Assert.That(item.GetValue(0), Is.EqualTo("one"));
                    Assert.That(item.GetValue(1), Is.EqualTo("two"));
                    Assert.That(item.SubItems[0].Text, Is.EqualTo("one"));
                    Assert.Throws<ArgumentOutOfRangeException>(() => item.SetValue(-1, "bad"));
                    Assert.Throws<ArgumentOutOfRangeException>(() => item.SetValue(2, "bad"));
                    Assert.Throws<ArgumentOutOfRangeException>(() => item.GetValue(-1));
                    Assert.Throws<ArgumentOutOfRangeException>(() => item.GetValue(2));
                });
            }
        }

        [Test]
        public void SmartListViewItemWithoutAllColumnsHandlesSubitemBoundaries()
        {
            using (var view = new TestSmartListView())
            {
                view.Columns.Add("A");
                view.Columns.Add("B");

                var item = new SmartListViewItem(view);
                view.Items.Add(item);
                item.SetValues("a", "b");

                Assert.Multiple(() =>
                {
                    Assert.That(item.GetValue(0), Is.EqualTo("a"));
                    Assert.That(item.GetValue(1), Is.EqualTo("b"));
                    Assert.Throws<ArgumentOutOfRangeException>(() => item.GetValue(2));
                });

                item.SetValue(1, null);
                Assert.That(item.GetValue(1), Is.Null);
            }
        }

        [Test]
        public void ColumnWidthsRoundTripNamedColumnsAndIgnoreUnknowns()
        {
            using (var view = new TestSmartListView())
            {
                var first = new SmartColumn(view, "First", 100, "first");
                var second = new SmartColumn(view, "Second", 80, "second");
                var unnamed = new SmartColumn(view, "Unnamed", null, 70);

                view.Columns.AddRange(new ColumnHeader[] { first, second, unnamed });
                view.AllColumns.Add(first);
                view.AllColumns.Add(second);
                view.AllColumns.Add(unnamed);

                first.Width = 140;

                IDictionary<string, int> saved = view.GetColumnWidths();
                Assert.That(saved["first"], Is.EqualTo(140));
                Assert.That(saved["second"], Is.EqualTo(-1));
                Assert.That(saved.ContainsKey(""), Is.False);

                view.SetColumnWidths(null);
                view.SetColumnWidths(new Dictionary<string, int>());
                view.SetColumnWidths(new Dictionary<string, int>
                {
                    { "first", 111 },
                    { "second", 92 },
                    { "missing", 44 }
                });

                Assert.Multiple(() =>
                {
                    Assert.That(first.Width, Is.EqualTo(111));
                    Assert.That(second.Width, Is.EqualTo(92));
                    Assert.That(unnamed.Width, Is.EqualTo(70));
                });
            }
        }

        [Test]
        public void ColumnClickSortsTogglesDirectionAndIgnoresUnsortableColumns()
        {
            using (var view = new TestSmartListView())
            {
                var first = new SmartColumn(view, "First", 100, "first");
                var second = new SmartColumn(view, "Second", 100, "second");
                second.Sortable = false;

                view.Columns.Add(first);
                view.Columns.Add(second);
                view.AllColumns.Add(first);
                view.AllColumns.Add(second);

                var firstItem = new SmartListViewItem(view);
                firstItem.SetValues("b", "2");
                view.Items.Add(firstItem);
                var secondItem = new SmartListViewItem(view);
                secondItem.SetValues("a", "1");
                view.Items.Add(secondItem);

                view.ClickColumn(0);
                Assert.Multiple(() =>
                {
                    Assert.That(view.SortColumns.Count, Is.EqualTo(1));
                    Assert.That(view.SortColumns[0], Is.SameAs(first));
                    Assert.That(first.ReverseSort, Is.False);
                });

                view.ClickColumn(0);
                Assert.That(first.ReverseSort, Is.True);

                view.ClickColumn(1);
                Assert.That(view.SortColumns[0], Is.SameAs(first));
            }
        }

        [Test]
        public void GroupingCreatesMovesFlushesAndClearsGroups()
        {
            Assert.That(SmartListView.SupportsGrouping, Is.True,
                "CI requires Windows ListView grouping support.");

            using (var view = new TestSmartListView())
            {
                var groupColumn = new SmartColumn(view, "Group", 100, "group");
                var valueColumn = new SmartColumn(view, "Value", 100, "value");

                view.Columns.Add(groupColumn);
                view.Columns.Add(valueColumn);
                view.AllColumns.Add(groupColumn);
                view.AllColumns.Add(valueColumn);
                view.GroupColumns.Add(groupColumn);

                var first = new SmartListViewItem(view);
                var second = new SmartListViewItem(view);
                view.Items.Add(first);
                view.Items.Add(second);
                first.SetValues("A", "one");
                second.SetValues("B", "two");

                Assert.That(view.Groups.Count, Is.EqualTo(2));

                second.SetValue(0, "A");
                view.RefreshGroupsAvailable();

                Assert.That(view.Groups.Count, Is.EqualTo(1));

                view.GroupColumns.Clear();
                view.RefreshGroups();

                Assert.Multiple(() =>
                {
                    Assert.That(view.Groups.Count, Is.Zero);
                    Assert.That(first.Group, Is.Null);
                    Assert.That(second.Group, Is.Null);
                });

                view.ClearItems();
                Assert.That(view.Items.Count, Is.Zero);
            }
        }

        [Test]
        public void SmartGroupConversionsPreserveWrapperAndRejectInvalidGroups()
        {
            using (var view = new TestSmartListView())
            {
                var smart = new SmartGroup(view, "key", "Header");
                ListViewGroup raw = smart;
                SmartGroup roundTrip = (SmartGroup)raw;

                Assert.That(roundTrip, Is.SameAs(smart));
                Assert.That((ListViewGroup)(SmartGroup)null, Is.Null);
                Assert.That((SmartGroup)(ListViewGroup)null, Is.Null);

                var tagged = new ListViewGroup("bad") { Tag = new object() };
                Assert.Throws<InvalidOperationException>(() => { var ignored = (SmartGroup)tagged; });

                var unattached = new ListViewGroup("none");
                Assert.Throws<InvalidOperationException>(() => { var ignored = (SmartGroup)unattached; });
            }
        }

        [Test]
        public void SelectAllChecksEligibleRowsSupportsCancelAndTracksItemChanges()
        {
            using (var view = new TestSmartListView())
            {
                view.CheckBoxes = true;
                view.ShowSelectAllCheckBox = true;

                var one = new ListViewItem("one");
                var two = new ListViewItem("two") { Tag = "skip" };
                var three = new ListViewItem("three");
                view.Items.AddRange(new[] { one, two, three });

                var args = new CancelEventArgs();
                view.ToggleSelectAll(args);

                Assert.Multiple(() =>
                {
                    Assert.That(args.Cancel, Is.True);
                    Assert.That(view.SelectAllState, Is.True);
                    Assert.That(one.Checked, Is.True);
                    Assert.That(two.Checked, Is.False);
                    Assert.That(three.Checked, Is.True);
                });

                one.Checked = false;
                Assert.That(view.SelectAllState, Is.False);

                one.Checked = true;
                Assert.That(view.SelectAllState, Is.True);

                view.CancelSelectAll = true;
                var cancelled = new CancelEventArgs();
                view.ToggleSelectAll(cancelled);

                Assert.Multiple(() =>
                {
                    Assert.That(cancelled.Cancel, Is.True);
                    Assert.That(view.SelectAllState, Is.True);
                    Assert.That(one.Checked, Is.True);
                });
            }
        }

        [Test]
        public void BeginEndUpdateDefersSelectAllRecalculation()
        {
            using (var view = new TestSmartListView())
            {
                view.CheckBoxes = true;
                view.ShowSelectAllCheckBox = true;
                var one = new ListViewItem("one");
                var two = new ListViewItem("two");
                view.Items.AddRange(new[] { one, two });

                view.BeginUpdate();
                one.Checked = true;
                two.Checked = true;
                Assert.That(view.SelectAllState, Is.False);
                view.EndUpdate();

                Assert.That(view.SelectAllState, Is.True);

                view.BeginUpdate();
                one.Checked = false;
                view.EndUpdate();
                Assert.That(view.SelectAllState, Is.False);
            }
        }

        [Test]
        public void ThemeChangeUsesSemanticPaletteAndFallsBackWithoutPalette()
        {
            using (var view = new TestSmartListView())
            {
                var palette = new AnkhThemePalette(
                    Color.FromArgb(30, 30, 30),
                    Color.FromArgb(240, 240, 240),
                    Color.FromArgb(40, 40, 40),
                    Color.White,
                    Color.Gray,
                    Color.FromArgb(80, 80, 80),
                    Color.FromArgb(60, 80, 120),
                    Color.White,
                    Color.FromArgb(50, 50, 50),
                    Color.FromArgb(45, 45, 45),
                    Color.CornflowerBlue);

                using (var services = new AnkhServiceContainer())
                {
                    services.AddService(typeof(IWinFormsThemingService), new FakeThemer(palette));
                    view.OnThemeChange(services, new CancelEventArgs(false));

                    Assert.Multiple(() =>
                    {
                        Assert.That(view.BackColor, Is.EqualTo(palette.SurfaceBackground));
                        Assert.That(view.ForeColor, Is.EqualTo(palette.SurfaceForeground));
                    });
                }

                using (var services = new AnkhServiceContainer())
                {
                    services.AddService(typeof(IWinFormsThemingService), new FakeThemer(null));
                    Assert.DoesNotThrow(() => view.OnThemeChange(services, new CancelEventArgs(true)));
                }
            }
        }

        [TestCase(true, true, true, ListViewHitTestLocations.Label, true)]
        [TestCase(true, false, true, ListViewHitTestLocations.Label, false)]
        [TestCase(false, true, true, ListViewHitTestLocations.Label, false)]
        [TestCase(true, true, false, ListViewHitTestLocations.Label, false)]
        [TestCase(true, true, true, ListViewHitTestLocations.None, false)]
        [TestCase(true, true, true, ListViewHitTestLocations.StateImage, false)]
        public void CheckboxDoubleClickSuppressionRequiresStrictNonCheckboxItemHit(
            bool checkBoxes,
            bool strict,
            bool hasItem,
            ListViewHitTestLocations location,
            bool expected)
        {
            MethodInfo method = typeof(SmartListView).GetMethod(
                "ShouldSuppressNativeCheckboxDoubleClick",
                BindingFlags.Static | BindingFlags.NonPublic);

            Assert.That(method, Is.Not.Null);
            bool actual = (bool)method.Invoke(null, new object[] { checkBoxes, strict, hasItem, location });
            Assert.That(actual, Is.EqualTo(expected));
        }

        sealed class TestSmartListView : SmartListView
        {
            public bool CancelSelectAll { get; set; }

            public bool SelectAllState
            {
                get { return SelectAllChecked; }
            }

            public void ClickColumn(int index)
            {
                OnColumnClick(new ColumnClickEventArgs(index));
            }

            public void ToggleSelectAll(CancelEventArgs args)
            {
                MethodInfo method = typeof(SmartListView).GetMethod(
                    "PerformSelectAllCheckedChange",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                method.Invoke(this, new object[] { args });
            }

            protected override bool IsPartOfSelectAll(ListViewItem item)
            {
                return !Equals(item.Tag, "skip");
            }

            protected override void OnSelectAllCheckedChanging(CancelEventArgs ce)
            {
                if (CancelSelectAll)
                    ce.Cancel = true;
                base.OnSelectAllCheckedChanging(ce);
            }
        }

        sealed class ConstantComparer : IComparer<ListViewItem>
        {
            readonly int _value;

            public ConstantComparer(int value)
            {
                _value = value;
            }

            public int Compare(ListViewItem x, ListViewItem y)
            {
                return _value;
            }
        }

        sealed class FakeThemer : IWinFormsThemingService
        {
            readonly AnkhThemePalette _palette;

            public FakeThemer(AnkhThemePalette palette)
            {
                _palette = palette;
            }

            public AnkhThemePalette ThemePalette
            {
                get { return _palette; }
            }

            public void ThemeRecursive(Control control, bool forDialog)
            {
            }

            public bool TryGetIcon(string path, out IntPtr hIcon)
            {
                hIcon = IntPtr.Zero;
                return false;
            }

            public bool GetCurrentTheme(out Guid themeGuid)
            {
                themeGuid = Guid.Empty;
                return false;
            }
        }
    }
}
