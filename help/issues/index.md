---
title: Issue Tracking Integration
---

# Issue Tracking Integration

AnkhSVN has an extension API for associating a versioned solution with an external issue repository. **The current AnkhSVN VSIX does not include an issue-tracker connector implementation**, so the Issues tab cannot be configured by the stock VSIX alone.

A separate extension must register an AnkhSVN `IssueRepositoryConnector` before **Issue Tracker Setup** becomes available.

## What the Issues tab means

The Issues tab now distinguishes these states:

- **No connector installed** — AnkhSVN has the integration framework, but no provider is available. **Issue Tracker Setup** is intentionally unavailable.
- **Connector installed, not configured** — right-click the solution in **Solution Explorer** and choose **Issue Tracker Setup**.
- **Configured repository cannot load** — verify that the configured connector is still installed and that its repository settings are valid.
- **Configured and available** — the Issues tab hosts the UI supplied by the connector.

## Configure an issue repository

These steps apply only after a compatible issue-tracker connector has been installed and registered:

1. Make sure the solution root is versioned in Subversion.
2. In **Solution Explorer**, right-click the **solution** node.
3. Choose **Issue Tracker Setup**.
4. Select the issue-tracker connector.
5. Enter the repository/project URL and any provider-specific settings requested by that connector.
6. Choose **OK**.
7. Return to **Pending Changes > Issues**.

AnkhSVN stores the association as Subversion properties on the solution root. Setting or changing the association therefore creates local property changes. **Commit those property changes** if the issue-repository association should be shared with other users of the working copy.

## If "Issue Tracker Setup" is missing

The command is visible only when:

- the solution root is versioned in Subversion, and
- at least one AnkhSVN issue-tracker connector is registered.

A stock installation of this VSIX currently has no connector implementation, so not seeing the command is expected until a compatible connector extension is installed.

## Remove or change an association

When a connector is available, open **Issue Tracker Setup** again from the solution node. Select another connector to change the association, or select **None** to remove it.

Removing the association deletes the corresponding issue-repository SVN properties. Commit those property changes if the removal should be shared with the repository.

## Connector developers

Connector integrations derive from `Ankh.ExtensionPoints.IssueTracker.IssueRepositoryConnector` and provide both repository creation and a configuration page. AnkhSVN discovers registered connectors at startup and exposes **Issue Tracker Setup** only when at least one is available.

## Troubleshooting

If a previously configured Issues tab stops working:

- verify the connector extension is still installed and loads successfully,
- reopen **Issue Tracker Setup** when the command is available and verify the repository settings,
- confirm the solution root has the expected issue-repository SVN properties,
- check whether those property changes were committed and updated into this working copy,
- verify authentication to the external issue system,
- test whether the issue system is reachable outside AnkhSVN.

If source-control operations succeed but issue updates fail, diagnose the SVN repository and issue tracker as separate systems.

[Commit and Pending Changes](../commit/)

[Back to AnkhSVN Help](../)
