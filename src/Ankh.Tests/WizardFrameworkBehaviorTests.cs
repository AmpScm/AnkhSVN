using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Threading;
using Ankh.UI.WizardFramework;
using NUnit.Framework;

namespace Ankh.Tests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class WizardFrameworkBehaviorTests
    {
        [Test]
        public void PageCollectionTracksOwnershipReplacementAndRemoval()
        {
            using (var wizard = new TestWizard())
            {
                Assert.Throws<ArgumentNullException>(() => new WizardPageCollection(null));

                var first = new TestPage("first");
                var replacement = new TestPage("replacement");
                wizard.Pages.Add(first);

                Assert.Multiple(() =>
                {
                    Assert.That(first.Wizard, Is.SameAs(wizard));
                    Assert.That(first.Container, Is.SameAs(wizard));
                    Assert.That(wizard.GetPage("first"), Is.SameAs(first));
                    Assert.That(wizard.GetPage("missing"), Is.Null);
                    Assert.That(wizard.GetPage<TestPage>(), Is.SameAs(first));
                });

                wizard.Pages[0] = replacement;
                Assert.Multiple(() =>
                {
                    Assert.That(first.Wizard, Is.Null);
                    Assert.That(replacement.Wizard, Is.SameAs(wizard));
                });

                wizard.Pages.Remove(replacement);
                Assert.That(replacement.Wizard, Is.Null);
            }
        }

        [Test]
        public void PageCannotBelongToTwoWizardsAtTheSameTime()
        {
            using (var one = new TestWizard())
            using (var two = new TestWizard())
            {
                var page = new TestPage("shared");
                one.Pages.Add(page);

                Assert.Throws<InvalidOperationException>(() => two.Pages.Add(page));

                one.Pages.Remove(page);
                Assert.DoesNotThrow(() => two.Pages.Add(page));
                Assert.That(page.Wizard, Is.SameAs(two));
            }
        }

        [Test]
        public void PageNameDescriptionMessageAndImageCoverFallbacks()
        {
            using (var wizard = new TestWizard())
            using (var defaultImage = new Bitmap(2, 2))
            using (var pageImage = new Bitmap(2, 2))
            {
                wizard.DefaultPageImage = defaultImage;
                var page = new TestPage(null);
                wizard.Pages.Add(page);

                Assert.That(page.Name, Is.EqualTo(typeof(TestPage).FullName));
                page.Name = "page";
                Assert.That(page.Name, Is.EqualTo("page"));

                Assert.That(page.Description, Is.Empty);
                page.Description = null;
                Assert.That(page.Description, Is.Empty);
                page.Description = "Description";
                Assert.That(page.Description, Is.EqualTo("Description"));

                Assert.That(page.Image, Is.SameAs(defaultImage));
                page.Image = pageImage;
                Assert.That(page.Image, Is.SameAs(pageImage));
                page.Image = null;
                Assert.That(page.Image, Is.SameAs(defaultImage));

                page.Message = null;
                Assert.Multiple(() =>
                {
                    Assert.That(page.MessageText, Is.Empty);
                    Assert.That(page.MessageType, Is.EqualTo(WizardMessage.MessageType.None));
                });

                page.Message = new WizardMessage("warning", WizardMessage.MessageType.Warning);
                Assert.Multiple(() =>
                {
                    Assert.That(page.Message.Message, Is.EqualTo("warning"));
                    Assert.That(page.Message.Type, Is.EqualTo(WizardMessage.MessageType.Warning));
                });
            }
        }

        [Test]
        public void ShowPageSupportsCancellationEventsAndPreviousPageTracking()
        {
            using (var wizard = new TestWizard())
            {
                var first = new TestPage("first") { Text = "First", Description = "Start" };
                var second = new TestPage("second") { Text = "Second", Description = "Next" };
                wizard.Pages.Add(first);
                wizard.Pages.Add(second);

                int changing = 0;
                int changed = 0;
                wizard.PageChanging += delegate(object sender, WizardPageChangingEventArgs e)
                {
                    changing++;
                    if (e.NewPage == second && wizard.CancelSecondPage)
                        e.Cancel = true;
                };
                wizard.PageChanged += delegate { changed++; };

                Assert.Throws<ArgumentNullException>(() => wizard.ShowPage(null));

                wizard.ShowPage(first);
                Assert.That(wizard.CurrentPage, Is.SameAs(first));

                wizard.CancelSecondPage = true;
                wizard.ShowPage(second);
                Assert.That(wizard.CurrentPage, Is.SameAs(first));

                wizard.CancelSecondPage = false;
                wizard.ShowPage(second);

                Assert.Multiple(() =>
                {
                    Assert.That(wizard.CurrentPage, Is.SameAs(second));
                    Assert.That(second.PreviousPage, Is.SameAs(first));
                    Assert.That(changing, Is.EqualTo(3));
                    Assert.That(changed, Is.EqualTo(2));
                    Assert.That(wizard.PageContainer.Controls.Contains(second), Is.True);
                });
            }
        }

        [Test]
        public void NavigationHelpersCoverEdgesUnknownPagesAndCompletion()
        {
            using (var wizard = new TestWizard())
            {
                var first = new TestPage("first");
                var second = new TestPage("second");
                wizard.Pages.Add(first);
                wizard.Pages.Add(second);

                Assert.Multiple(() =>
                {
                    Assert.That(wizard.StartingPage, Is.SameAs(first));
                    Assert.That(wizard.PageCount, Is.EqualTo(2));
                    Assert.That(wizard.GetPreviousPage(first), Is.Null);
                    Assert.That(wizard.GetPreviousPage(second), Is.SameAs(first));
                    Assert.That(wizard.GetNextPage(first), Is.SameAs(second));
                    Assert.That(wizard.GetNextPage(second), Is.Null);
                    Assert.That(wizard.GetNextPage(new TestPage("outside")), Is.Null);
                    Assert.That(wizard.GetPreviousPage(new TestPage("outside")), Is.Null);
                    Assert.That(wizard.NextIsFinish, Is.False);
                });

                wizard.ShowPage(first);
                first.IsPageComplete = true;
                second.IsPageComplete = true;
                Assert.That(wizard.NextIsFinish, Is.True);

                second.IsPageComplete = false;
                Assert.That(wizard.NextIsFinish, Is.False);

                using (var empty = new TestWizard())
                    Assert.That(empty.StartingPage, Is.Null);
            }
        }

        [Test]
        public void PageNavigationPropertiesReflectCompletionAndContainer()
        {
            using (var wizard = new TestWizard())
            {
                var first = new TestPage("first");
                var second = new TestPage("second");
                wizard.Pages.Add(first);
                wizard.Pages.Add(second);
                wizard.ShowPage(first);

                Assert.That(first.CanFlipToNextPage, Is.False);
                first.IsPageComplete = true;
                Assert.That(first.CanFlipToNextPage, Is.True);

                Assert.That(first.NextPage, Is.SameAs(second));
                Assert.That(second.NextPage, Is.Null);
                Assert.That(first.PreviousPage, Is.Null);
                Assert.That(second.PreviousPage, Is.SameAs(first));

                second.PreviousPage = null;
                Assert.That(second.PreviousPage, Is.SameAs(first));
                second.PreviousPage = first;
                Assert.That(second.PreviousPage, Is.SameAs(first));

                var orphan = new TestPage("orphan");
                Assert.Multiple(() =>
                {
                    Assert.That(orphan.NextPage, Is.Null);
                    Assert.That(orphan.PreviousPage, Is.Null);
                    Assert.That(orphan.Image, Is.Null);
                    Assert.That(orphan.Container, Is.Null);
                    Assert.That(orphan.Wizard, Is.Null);
                    Assert.That(orphan.CanFlipToNextPage, Is.False);
                });
            }
        }

        [TestCase(WizardMessage.MessageType.Error)]
        [TestCase(WizardMessage.MessageType.Information)]
        [TestCase(WizardMessage.MessageType.Warning)]
        [TestCase(WizardMessage.MessageType.None)]
        public void ShowPageUpdatesEachMessageType(WizardMessage.MessageType type)
        {
            using (var wizard = new TestWizard())
            {
                var page = new TestPage("message")
                {
                    Text = "Title",
                    Description = "Description",
                    IsPageComplete = true,
                    Message = new WizardMessage(type == WizardMessage.MessageType.None ? "" : "message", type)
                };

                Assert.DoesNotThrow(() => wizard.ShowPage(page));
                Assert.That(wizard.Pages.Contains(page), Is.True,
                    "ShowPage should add pages that are not already registered.");
                Assert.That(wizard.CurrentPage, Is.SameAs(page));
            }
        }

        [Test]
        public void EnableMethodsAndButtonsRoundTripContainerState()
        {
            using (var wizard = new TestWizard())
            {
                var page = new TestPage("page");
                wizard.Pages.Add(page);
                wizard.ShowPage(page);

                wizard.EnablePageAndButtons(false);
                Assert.Multiple(() =>
                {
                    Assert.That(wizard.PageContainer.Enabled, Is.False);
                    Assert.That(wizard.NavigationContainer.Enabled, Is.False);
                });

                wizard.EnablePage(true);
                wizard.EnableButtons(true);
                Assert.Multiple(() =>
                {
                    Assert.That(wizard.PageContainer.Enabled, Is.True);
                    Assert.That(wizard.NavigationContainer.Enabled, Is.True);
                });

                Assert.DoesNotThrow(() =>
                {
                    wizard.UpdateButtons();
                    wizard.UpdateTitleBar();
                    wizard.UpdateMessage();
                });
            }
        }

        [Test]
        public void BackNextAndFinishHandlersFollowPageState()
        {
            using (var wizard = new TestWizard())
            {
                var first = new TestPage("first") { IsPageComplete = true };
                var second = new TestPage("second") { IsPageComplete = false };
                wizard.Pages.Add(first);
                wizard.Pages.Add(second);
                wizard.ShowPage(first);

                Invoke(wizard, "nextButton_Click");
                Assert.That(wizard.CurrentPage, Is.SameAs(second));

                Invoke(wizard, "backButton_Click");
                Assert.That(wizard.CurrentPage, Is.SameAs(first));

                second.IsPageComplete = true;
                wizard.ShowPage(second);
                Assert.That(wizard.NextIsFinish, Is.True);

                Invoke(wizard, "nextButton_Click");
                Assert.That(wizard.FinishCount, Is.EqualTo(1));

                wizard.CancelFinish = true;
                Invoke(wizard, "finishButton_Click");
                Assert.That(wizard.FinishCount, Is.EqualTo(2));

                wizard.Pages.Clear();
                Invoke(wizard, "backButton_Click");
            }
        }

        static void Invoke(Wizard wizard, string methodName)
        {
            MethodInfo method = typeof(Wizard).GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            method.Invoke(wizard, new object[] { null, EventArgs.Empty });
        }

        sealed class TestWizard : Wizard
        {
            public bool CancelSecondPage { get; set; }
            public bool CancelFinish { get; set; }
            public int FinishCount { get; private set; }

            public override void OnFinish(CancelEventArgs e)
            {
                FinishCount++;
                e.Cancel = CancelFinish;
            }
        }

        sealed class TestPage : WizardPage
        {
            public TestPage(string name)
            {
                if (name != null)
                    Name = name;
            }
        }
    }
}
