using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Ankh;
using Ankh.UI.PendingChanges;
using Ankh.UI.PendingChanges.Commits;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.PendingChanges
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class PendingCommitsSelectAllTests
    {
        sealed class TestView : PendingCommitsView
        {
            protected override bool IsPartOfSelectAll(ListViewItem item)
            {
                return !Equals(item.Tag, "ignore-on-commit");
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void UncheckAllClearsEveryGroupIncludingManuallyCheckedIgnoredItems(bool createHandle)
        {
            using (var view = new TestView())
            {
                var first = new ListViewGroup("First");
                var second = new ListViewGroup("Second");
                view.Groups.AddRange(new[] { first, second });
                view.Items.Add(new ListViewItem("a", first) { Checked = true });
                view.Items.Add(new ListViewItem("b", second) { Checked = false });
                view.Items.Add(new ListViewItem("ignored", second)
                    { Checked = true, Tag = "ignore-on-commit" });
                if (createHandle)
                    Assert.That(view.Handle, Is.Not.EqualTo(IntPtr.Zero));

                Assert.That(view.AllItemsCheckState, Is.EqualTo(CheckState.Indeterminate));
                int notifications = 0;
                view.CheckedItemsChanged += (s, e) => notifications++;
                view.SetAllItemsChecked(false);

                Assert.That(view.Items.Cast<ListViewItem>().Any(i => i.Checked), Is.False);
                Assert.That(view.AllItemsCheckState, Is.EqualTo(CheckState.Unchecked));
                Assert.That(notifications, Is.EqualTo(1));

                view.SetAllItemsChecked(true);
                Assert.That(view.Items[0].Checked, Is.True);
                Assert.That(view.Items[1].Checked, Is.True);
                Assert.That(view.Items[2].Checked, Is.False);
                Assert.That(view.AllItemsCheckState, Is.EqualTo(CheckState.Checked));
                view.SetAllItemsChecked(false);
                Assert.That(view.CheckedItems.Count, Is.Zero);
            }
        }

        [Test]
        public void SeparateCheckboxSurvivesHeaderThemingAndClearsMixedSelection()
        {
            using (var page = new PendingCommitsPage())
            using (var services = new AnkhServiceContainer())
            {
                typeof(PendingCommitsPage).GetMethod("InsertPendingChanges",
                    BindingFlags.Instance | BindingFlags.NonPublic).Invoke(page, null);
                var checkbox = (CheckBox)page.Controls.Find("selectAllChanges", true).Single();
                var view = (PendingCommitsView)checkbox.Parent.Controls
                    .Cast<Control>().Single(c => c is PendingCommitsView);
                Assert.That(checkbox.Text, Is.Not.Empty);
                Assert.That(checkbox.Enabled, Is.False);
                Assert.That(checkbox.Dock, Is.EqualTo(DockStyle.Top));
                Assert.That(checkbox.AutoCheck, Is.False);
                checkbox.Parent.PerformLayout();
                Assert.That(view.Top, Is.GreaterThanOrEqualTo(checkbox.Bottom),
                    "The checkbox must occupy its own row above the file list.");

                view.Items.Add(new ListViewItem("checked") { Checked = true });
                view.Items.Add(new ListViewItem("unchecked"));
                Assert.That(view.Handle, Is.Not.EqualTo(IntPtr.Zero));
                view.OnThemeChange(services, new CancelEventArgs());
                Assert.That(view.ShowSelectAllCheckBox, Is.False);
                Assert.That(checkbox.Visible, Is.True);

                // Exercise the actual Click event used by mouse and keyboard.
                typeof(Control).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(checkbox, new object[] { EventArgs.Empty });
                Assert.That(view.CheckedItems.Count, Is.Zero);
                Assert.That(checkbox.CheckState, Is.EqualTo(CheckState.Unchecked));
            }
        }

        [Test]
        public void EmptyListAndIndividualChangesKeepAggregateStateAccurate()
        {
            using (var view = new TestView())
            {
                view.SetAllItemsChecked(true);
                Assert.That(view.AllItemsCheckState, Is.EqualTo(CheckState.Unchecked));
                view.Items.Add(new ListViewItem("a"));
                view.Items.Add(new ListViewItem("b"));
                Assert.That(view.Handle, Is.Not.EqualTo(IntPtr.Zero));
                view.Items[0].Checked = true;
                Assert.That(view.AllItemsCheckState, Is.EqualTo(CheckState.Indeterminate));
                view.Items[1].Checked = true;
                Assert.That(view.AllItemsCheckState, Is.EqualTo(CheckState.Checked));
                view.Items[0].Checked = false;
                Assert.That(view.AllItemsCheckState, Is.EqualTo(CheckState.Indeterminate));
                view.Items[1].Checked = false;
                Assert.That(view.AllItemsCheckState, Is.EqualTo(CheckState.Unchecked));
            }
        }
    }
}
