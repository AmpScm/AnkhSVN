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

using Ankh.UI.IssueTracker;
using Ankh.UI.PendingChanges;
using System;
using System.Reflection;
using System.Linq;
using System.Windows.Forms;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Dialogs
{
    [TestFixture]
    public class PendingIssuesPageTests
    {
        [TestCase(false, 0, false)]
        [TestCase(false, 1, false)]
        [TestCase(true, 0, false)]
        [TestCase(true, 1, true)]
        [TestCase(true, 3, true)]
        public void SetupCommandRequiresVersionedRootAndConnector(
            bool rootIsVersioned,
            int connectorCount,
            bool expected)
        {
            Assert.That(
                IssueTrackerAvailabilityLogic.CanConfigure(
                    rootIsVersioned,
                    connectorCount),
                Is.EqualTo(expected));
        }

        [Test]
        public void EmptyStateDistinguishesMissingConnectorFromMissingConfiguration()
        {
            Assert.Multiple(() =>
            {
                Assert.That(
                    IssueTrackerAvailabilityLogic.GetEmptyState(0, false),
                    Is.EqualTo(IssueTrackerEmptyState.NoConnectors));
                Assert.That(
                    IssueTrackerAvailabilityLogic.GetEmptyState(1, false),
                    Is.EqualTo(IssueTrackerEmptyState.NotConfigured));
                Assert.That(
                    IssueTrackerAvailabilityLogic.GetEmptyState(1, true),
                    Is.EqualTo(IssueTrackerEmptyState.ConfigurationUnavailable));
            });
        }

        [Test, Apartment(System.Threading.ApartmentState.STA)]
        public void ProviderStateCanDisableAndRestorePendingChangesNavigation()
        {
            using (var tool = new PendingChangesToolControl())
            {
                MethodInfo apply = typeof(PendingChangesToolControl).GetMethod(
                    "ApplyNavigationProviderState",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                FieldInfo tabsField = typeof(PendingChangesToolControl).GetField(
                    "pendingChangesTabs",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                Assert.That(apply, Is.Not.Null);
                Assert.That(tabsField, Is.Not.Null);

                ToolStrip tabs = (ToolStrip)tabsField.GetValue(tool);

                apply.Invoke(tool, new object[] { false });
                Assert.That(tabs.Enabled, Is.False);

                apply.Invoke(tool, new object[] { true });
                Assert.That(
                    tabs.Enabled,
                    Is.True,
                    "A later active provider state must always recover navigation without recreating the tool window.");
            }
        }

        [Test, Apartment(System.Threading.ApartmentState.STA)]
        public void ConfiguredIssueTrackerExposesChangeAndRemoveActions()
        {
            using (var page = new PendingIssuesPage())
            {
                var repository = new LocalSvnIssuesRepository(
                    null,
                    new LocalSvnIssuesSettings(".ankh/issues.xml"));

                using (FlowLayoutPanel bar =
                    page.CreateTrackerManagementBar(repository))
                {
                    Button change = bar.Controls
                        .OfType<Button>()
                        .Single(b => b.Name == "changeIssueTrackerButton");
                    Button remove = bar.Controls
                        .OfType<Button>()
                        .Single(b => b.Name == "removeIssueTrackerButton");
                    Label current = bar.Controls
                        .OfType<Label>()
                        .Single(l => l.Name == "currentIssueTrackerLabel");

                    Assert.Multiple(() =>
                    {
                        Assert.That(change.Text, Is.EqualTo("Change Tracker..."));
                        Assert.That(remove.Text, Is.EqualTo("Remove Tracker"));
                        Assert.That(remove.Enabled, Is.True);
                        Assert.That(current.Text, Does.Contain("Local SVN Issues"));
                    });
                }
            }
        }

        [Test, Apartment(System.Threading.ApartmentState.STA)]
        public void TrackerManagementBarUsesSeparateRowFromProviderContent()
        {
            using (var page = new PendingIssuesPage())
            using (var provider = new Panel { Name = "providerContent" })
            {
                var repository = new LocalSvnIssuesRepository(
                    null,
                    new LocalSvnIssuesSettings(".ankh/issues.xml"));

                using (TableLayoutPanel host =
                    page.CreateTrackerHost(provider, repository))
                {
                    Control management =
                        host.Controls.Find(
                            "issueTrackerManagementBar",
                            true).Single();

                    Assert.Multiple(() =>
                    {
                        Assert.That(host.GetRow(management), Is.EqualTo(0));
                        Assert.That(host.GetRow(provider), Is.EqualTo(1));
                        Assert.That(
                            host.RowStyles[0].SizeType,
                            Is.EqualTo(SizeType.AutoSize));
                        Assert.That(
                            host.RowStyles[1].SizeType,
                            Is.EqualTo(SizeType.Percent));
                    });
                }
            }
        }

        [Test, Apartment(System.Threading.ApartmentState.STA)]
        public void UnconfiguredIssuesPageOffersAddTracker()
        {
            using (var page = new PendingIssuesPage())
            using (TableLayoutPanel host =
                page.CreateEmptyTrackerHost(
                    IssueTrackerEmptyState.NotConfigured,
                    true))
            {
                Button add = host.Controls
                    .Find("addIssueTrackerButton", true)
                    .OfType<Button>()
                    .Single();

                Assert.Multiple(() =>
                {
                    Assert.That(add.Text, Is.EqualTo("Add Tracker..."));
                    Assert.That(add.Enabled, Is.True);
                });
            }
        }

        [Test]
        public void RemovingTrackerKeepsProviderDataByDesign()
        {
            Assert.Multiple(() =>
            {
                Assert.That(
                    IssueTrackerAssociationManager.DeleteLocalIssueDataOnRemove,
                    Is.False);
                Assert.That(
                    IssueTrackerAssociationManager.DeleteBugtraqPropertiesOnRemove,
                    Is.False);
            });
        }

        [Test]
        public void EmptyStateMessagesExplainTheActualNextStep()
        {
            string noConnector = IssueTrackerAvailabilityLogic.GetMessage(
                IssueTrackerEmptyState.NoConnectors);
            string notConfigured = IssueTrackerAvailabilityLogic.GetMessage(
                IssueTrackerEmptyState.NotConfigured);
            string unavailable = IssueTrackerAvailabilityLogic.GetMessage(
                IssueTrackerEmptyState.ConfigurationUnavailable);

            Assert.Multiple(() =>
            {
                Assert.That(noConnector, Does.Contain("Generic Bugtraq"));
                Assert.That(noConnector, Does.Contain("Local SVN Issues"));
                Assert.That(notConfigured, Does.Contain("Issue Tracker Setup"));
                Assert.That(notConfigured, Does.Contain("Generic Bugtraq"));
                Assert.That(unavailable, Does.Contain("could not be loaded"));
                Assert.That(noConnector, Does.Contain(IssueTrackerAvailabilityLogic.HelpLinkText));
                Assert.That(notConfigured, Does.Contain(IssueTrackerAvailabilityLogic.HelpLinkText));
                Assert.That(unavailable, Does.Contain(IssueTrackerAvailabilityLogic.HelpLinkText));
            });
        }
    }
}
