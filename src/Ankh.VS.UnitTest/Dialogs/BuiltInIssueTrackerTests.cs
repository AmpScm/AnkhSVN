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

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Ankh.UI;
using Ankh.UI.VSSelectionControls;
using Ankh.VS;
using Ankh;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.UI.IssueTracker;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Dialogs
{
    [TestFixture]
    public class BuiltInIssueTrackerTests
    {
        [Test]
        public void BuiltInConnectorsIncludeGenericBugtraqAndLocalSvnIssues()
        {
            string[] names = BuiltInIssueTrackerConnectors.Create(null)
                .Select(c => c.Name)
                .ToArray();

            Assert.That(
                names,
                Is.EqualTo(new[]
                {
                    "Generic Bugtraq",
                    "Local SVN Issues"
                }));
        }

        [TestCase("", false)]
        [TestCase("https://tracker.example/issues/", false)]
        [TestCase("https://tracker.example/issues/%BUGID%", true)]
        [TestCase("^/issues/%BUGID%", true)]
        public void GenericBugtraqRequiresBugIdPlaceholder(
            string url,
            bool expected)
        {
            var settings = new GenericBugtraqSettings
            {
                UrlTemplate = url
            };

            Assert.That(settings.IsComplete, Is.EqualTo(expected));
        }

        [TestCase(".ankh/issues.xml", true)]
        [TestCase("issues.xml", true)]
        [TestCase("folder/issues.xml", true)]
        [TestCase("../issues.xml", false)]
        [TestCase(@"C:\issues.xml", false)]
        [TestCase("", false)]
        public void LocalIssuePathMustStayRelativeToWorkingCopy(
            string path,
            bool expected)
        {
            Assert.That(
                LocalIssueStore.IsValidRelativePath(path),
                Is.EqualTo(expected));
        }

        [Test]
        public void LocalIssueStoreRoundTripsIssueData()
        {
            string directory = Path.Combine(
                Path.GetTempPath(),
                "AnkhLocalIssues-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);

            try
            {
                string file = Path.Combine(directory, "issues.xml");
                DateTime created = new DateTime(
                    2026, 9, 25, 20, 0, 0, DateTimeKind.Utc);
                DateTime updated = created.AddMinutes(15);

                LocalIssueStore.SaveFile(
                    file,
                    new[]
                    {
                        new LocalIssueRecord
                        {
                            Id = 7,
                            Status = "Open",
                            Title = "Theme the history list",
                            Description = "Keep semantic colors after selection.",
                            CreatedUtc = created,
                            UpdatedUtc = updated
                        }
                    });

                LocalIssueRecord issue =
                    LocalIssueStore.LoadFile(file).Single();

                Assert.Multiple(() =>
                {
                    Assert.That(issue.Id, Is.EqualTo(7));
                    Assert.That(issue.Status, Is.EqualTo("Open"));
                    Assert.That(issue.Title, Is.EqualTo("Theme the history list"));
                    Assert.That(
                        issue.Description,
                        Is.EqualTo("Keep semantic colors after selection."));
                    Assert.That(issue.CreatedUtc, Is.EqualTo(created));
                    Assert.That(issue.UpdatedUtc, Is.EqualTo(updated));
                });
            }
            finally
            {
                Directory.Delete(directory, true);
            }
        }

        [Test]
        public void LocalIssueCommitAddsReferenceOnlyOnce()
        {
            Assert.Multiple(() =>
            {
                Assert.That(
                    LocalSvnIssuesRepository.AppendIssueReference(
                        "Fix the thing",
                        "12"),
                    Is.EqualTo(
                        "Fix the thing"
                        + Environment.NewLine
                        + "Issue #12"));

                Assert.That(
                    LocalSvnIssuesRepository.AppendIssueReference(
                        "Fix the thing"
                        + Environment.NewLine
                        + "Issue #12",
                        "12"),
                    Is.EqualTo(
                        "Fix the thing"
                        + Environment.NewLine
                        + "Issue #12"));
            });
        }

        [Test]
        public void EmbeddedIssueControlsUseSharedThemingService()
        {
            using (var services = new AnkhServiceContainer())
            using (var control = new Panel())
            {
                var themer = new RecordingThemer();
                services.AddService(typeof(IWinFormsThemingService), themer);

                bool themed = IssueTrackerThemeLogic.ThemeEmbeddedControl(
                    services,
                    control,
                    true);

                Assert.Multiple(() =>
                {
                    Assert.That(themed, Is.True);
                    Assert.That(themer.Control, Is.SameAs(control));
                    Assert.That(themer.ForDialog, Is.True);
                    Assert.That(themer.CallCount, Is.EqualTo(1));
                });
            }
        }

        [Test]
        public void EmbeddedIssueThemeHelperIsSafeWithoutServices()
        {
            using (var control = new Panel())
            {
                Assert.Multiple(() =>
                {
                    Assert.That(
                        IssueTrackerThemeLogic.ThemeEmbeddedControl(
                            null,
                            control,
                            false),
                        Is.False);
                    Assert.That(
                        IssueTrackerThemeLogic.ThemeEmbeddedControl(
                            new AnkhServiceContainer(),
                            null,
                            false),
                        Is.False);
                });
            }
        }

        [Test, Apartment(System.Threading.ApartmentState.STA)]
        public void LocalIssueUiUsesVisualStudioThemeInfrastructure()
        {
            using (var editor = new LocalIssueEditDialog(
                new LocalIssueRecord
                {
                    Id = 1,
                    Status = "Open",
                    Title = "Theme test"
                }))
            using (var view = new LocalSvnIssuesView(
                null,
                new LocalSvnIssuesRepository(
                    null,
                    new LocalSvnIssuesSettings(".ankh/issues.xml")),
                new LocalIssueStore(null, ".ankh/issues.xml")))
            {
                var listField = typeof(LocalSvnIssuesView).GetField(
                    "_list",
                    System.Reflection.BindingFlags.Instance
                        | System.Reflection.BindingFlags.NonPublic);
                var list = (SmartListView)listField.GetValue(view);

                Assert.Multiple(() =>
                {
                    Assert.That(editor, Is.InstanceOf<VSDialogForm>());
                    Assert.That(editor.EnableTheming, Is.True);
                    Assert.That(list.AllowDarkNativeTheme, Is.False);
                    Assert.That(list.PreserveItemForeColorWhenSelected, Is.True);
                    Assert.That(list.PreserveItemForeColorWhenHot, Is.True);
                    Assert.That(list.HideSelection, Is.False);
                });
            }
        }

        [Test, Apartment(System.Threading.ApartmentState.STA)]
        public void LocalIssueEditorAppliesThemeAgainAfterHandlesExist()
        {
            using (var services = new AnkhServiceContainer())
            using (var editor = new LocalIssueEditDialog(
                new LocalIssueRecord
                {
                    Id = 3,
                    Status = "Open",
                    Title = "Theme the issue editor"
                }))
            {
                var themer = new RecordingThemer();
                services.AddService(typeof(IWinFormsThemingService), themer);
                editor.Context = services;

                // Force the form and child control tree to exist as it does at
                // display time, then exercise the explicit visible-theme pass.
                IntPtr handle = editor.Handle;
                foreach (Control child in editor.Controls)
                    child.CreateControl();

                bool themed = editor.ApplyVisibleTheme();

                Assert.Multiple(() =>
                {
                    Assert.That(themed, Is.True);
                    Assert.That(themer.CallCount, Is.EqualTo(1));
                    Assert.That(themer.Control, Is.SameAs(editor));
                    Assert.That(themer.ForDialog, Is.True);
                    Assert.That(editor.IsHandleCreated, Is.True);
                    Assert.That(editor.Controls.Count, Is.GreaterThan(0));
                });
            }
        }

        [Test]
        public void BuiltInRepositoriesUseDistinctPersistentConnectorNames()
        {
            IssueRepository generic =
                new GenericBugtraqConnector(null).Create(
                    new GenericBugtraqSettings
                    {
                        UrlTemplate = "https://tracker.example/%BUGID%"
                    });
            IssueRepository local =
                new LocalSvnIssuesConnector(null).Create(
                    new LocalSvnIssuesSettings(".ankh/issues.xml"));

            Assert.Multiple(() =>
            {
                Assert.That(generic.ConnectorName, Is.EqualTo("Generic Bugtraq"));
                Assert.That(local.ConnectorName, Is.EqualTo("Local SVN Issues"));
                Assert.That(generic.RepositoryUri, Is.Not.EqualTo(local.RepositoryUri));
            });
        }
        sealed class RecordingThemer : IWinFormsThemingService
        {
            public int CallCount { get; private set; }
            public Control Control { get; private set; }
            public bool ForDialog { get; private set; }

            public AnkhThemePalette ThemePalette
            {
                get { return null; }
            }

            public void ThemeRecursive(Control control, bool forDialog)
            {
                CallCount++;
                Control = control;
                ForDialog = forDialog;
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
