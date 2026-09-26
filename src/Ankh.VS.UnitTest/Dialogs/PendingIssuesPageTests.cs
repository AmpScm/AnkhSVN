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
