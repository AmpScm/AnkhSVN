---
title: Issue Tracking Integration
---

# Issue Tracking Integration

AnkhSVN can associate a versioned solution with an external issue repository when an issue-tracker connector is installed and available.

## Configure an issue repository

1. In **Solution Explorer**, right-click the **solution** node.
2. Choose **Issue Tracker Setup**.
3. Select the issue-tracker connector you want to use.
4. Enter the repository/project URL and any provider-specific settings requested by that connector.
5. Choose **OK**.
6. Return to **Pending Changes > Issues**. The issue repository's view should replace the setup message.

AnkhSVN stores the association as Subversion properties on the solution root. Setting or changing the association therefore creates local property changes. **Commit those property changes** if the issue-repository association should be shared with other users of the working copy.

## If "Issue Tracker Setup" is missing

The command is only enabled when:

- the solution root is versioned in Subversion, and
- AnkhSVN has at least one issue-tracker connector available.

If the command is missing, first verify the solution itself is under source control. Then verify that the intended issue-tracker connector is installed and loads successfully.

## Remove or change an association

Open **Issue Tracker Setup** again from the solution node. Select another connector to change the association, or select **None** to remove it.

Removing the association deletes the corresponding issue-repository SVN properties. Commit those property changes if the removal should be shared with the repository.

## Using the Issues tab

Once configured, the **Issues** tab hosts the UI supplied by the selected issue-tracker connector. The exact fields, authentication behavior, filters, and available actions depend on that connector.

The issue tracker remains the system that owns the issue. AnkhSVN supplies integration around the working-copy and commit workflow.

## Troubleshooting

If the Issues tab remains empty or does not show the expected repository:

- reopen **Issue Tracker Setup** and verify the selected connector and repository URL,
- verify authentication to the external issue system,
- confirm the solution root has the expected issue-repository SVN properties,
- check whether those property changes were committed and updated into this working copy,
- confirm the connector itself is installed and enabled,
- test whether the issue system is reachable outside AnkhSVN.

If source-control operations succeed but issue updates fail, diagnose the SVN repository and issue tracker as separate systems.

[Commit and Pending Changes](../commit/)

[Back to AnkhSVN Help](../)
