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

namespace Ankh.UI.IssueTracker
{
    internal enum IssueTrackerEmptyState
    {
        NoConnectors,
        NotConfigured,
        ConfigurationUnavailable
    }

    internal static class IssueTrackerAvailabilityLogic
    {
        internal const string HelpLinkText = "Click here";

        internal static bool CanConfigure(bool rootIsVersioned, int connectorCount)
        {
            return rootIsVersioned && connectorCount > 0;
        }

        internal static IssueTrackerEmptyState GetEmptyState(
            int connectorCount,
            bool hasConfiguredRepository)
        {
            if (connectorCount <= 0)
                return IssueTrackerEmptyState.NoConnectors;

            return hasConfiguredRepository
                ? IssueTrackerEmptyState.ConfigurationUnavailable
                : IssueTrackerEmptyState.NotConfigured;
        }

        internal static string GetMessage(IssueTrackerEmptyState state)
        {
            switch (state)
            {
                case IssueTrackerEmptyState.NoConnectors:
                    return
                        "No issue tracker connectors are available.\r\n\r\n"
                        + "AnkhSVN normally includes Generic Bugtraq and Local SVN "
                        + "Issues. If neither is available, repair or reinstall the "
                        + "extension.\r\n\r\n"
                        + "Click here for more information.";

                case IssueTrackerEmptyState.NotConfigured:
                    return
                        "This solution is not associated with an Issue Repository.\r\n\r\n"
                        + "Right-click the solution in Solution Explorer and choose "
                        + "Issue Tracker Setup. You can use Generic Bugtraq, Local SVN "
                        + "Issues, or an installed external connector.\r\n\r\n"
                        + "Click here for more information.";

                case IssueTrackerEmptyState.ConfigurationUnavailable:
                    return
                        "The configured Issue Repository could not be loaded.\r\n\r\n"
                        + "Verify that its connector is installed and that the repository "
                        + "settings are valid.\r\n\r\n"
                        + "Click here for troubleshooting.";

                default:
                    return string.Empty;
            }
        }
    }
}
