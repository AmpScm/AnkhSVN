---
title: Issue Tracking Integration
---

# Issue Tracking Integration

AnkhSVN supports issue tracking through two built-in options plus the original external connector extension API.

## Built-in options

### Generic Bugtraq

**Generic Bugtraq** uses the standard Subversion `bugtraq:*` properties also understood by TortoiseSVN.

Use it when your tracker can open an issue from a URL such as:

`https://example/issues/%BUGID%`

The setup page lets you configure the URL, commit-message pattern, label, issue-ID type, reminder behavior, append behavior, and log-message regular expression. These are written as normal SVN `bugtraq:*` properties on the solution working-copy root, so TortoiseSVN and other compatible clients can use the same configuration.

The Issues tab provides an **Open Issue** field rather than attempting to enumerate issues from a service that has no listing API.

### Local SVN Issues

**Local SVN Issues** is a small tracker stored directly in the working copy. By default it uses:

`.ankh/issues.xml`

The file is added to Subversion so issue records can be committed and shared like any other project data. The Issues tab can:

- create issues,
- edit titles and descriptions,
- mark issues closed or reopen them,
- sort the issue list,
- open an issue by ID.

When Local SVN Issues is active, AnkhSVN also exposes an **Issue** field in the commit UI. Entering an issue ID appends `Issue #<id>` to the commit message.

The local tracker is intentionally small. SVN remains responsible for history, branching, merging, and synchronization of the issue file.

## Configure an issue repository

1. Make sure the solution root is versioned in Subversion.
2. In **Solution Explorer**, right-click the **solution** node.
3. Choose **Issue Tracker Setup**.
4. Choose **Generic Bugtraq**, **Local SVN Issues**, or an installed external connector.
5. Configure the selected provider and choose **OK**.
6. Return to **Pending Changes > Issues**.

The AnkhSVN association itself is stored as SVN properties on the solution root. Commit those property changes if the selection should be shared with other users.

For Generic Bugtraq, the standard `bugtraq:*` properties are also written to the solution root. For Local SVN Issues, the configured issue file is created and scheduled for addition to SVN.

## Existing TortoiseSVN bugtraq properties

If you already configured `bugtraq:*` properties with TortoiseSVN, choose **Generic Bugtraq** in **Issue Tracker Setup**. AnkhSVN pre-populates the built-in configuration from the project commit settings it already reads.

This keeps the generic integration compatible with existing working copies instead of introducing a second URL/message format.

## Visual Studio themes and accessibility

The built-in issue-tracker UI uses AnkhSVN's shared Visual Studio semantic WinForms palette rather than fixed light/dark colors.

- **Issue Tracker Setup** re-themes a provider's configuration page immediately when you switch providers.
- The **Issues** tab re-themes the active provider UI when Visual Studio changes theme at runtime.
- **Local SVN Issues** uses the shared `SmartListView` selection, hover, header, and high-contrast behavior.
- The local issue editor uses `VSDialogForm`, so its caption, text boxes, combo box, buttons, labels, and high-contrast behavior follow the same dialog theme path as other AnkhSVN dialogs.
- Generic Bugtraq uses standard WinForms controls and is themed recursively by the same service.

External connectors remain supported. AnkhSVN applies the shared recursive theme service to the connector control it hosts; custom owner-drawn content supplied by an external connector remains that connector's responsibility.

## External connectors are still supported

No existing connector mechanism has been removed. Extensions deriving from `Ankh.ExtensionPoints.IssueTracker.IssueRepositoryConnector` are still discovered through the original registration mechanism and appear alongside the two built-in options.

If an external connector uses the same registered name as a built-in connector, the externally registered connector takes precedence.

## Remove or change an association

When a tracker is configured, the **Issues** tab shows the active provider and two management actions:

- **Change Tracker...** opens **Issue Tracker Setup** with the current provider selected. Choose another built-in or external connector and save it.
- **Remove Tracker** asks for confirmation and removes the AnkhSVN issue-repository association from the solution.

The older **None** choice in **Issue Tracker Setup** remains supported for compatibility, so no existing workflow is removed.

Removing an association is intentionally non-destructive. It does **not** delete a Local SVN Issues data file and does **not** erase standard `bugtraq:*` properties. Those remain ordinary versioned project data/properties and can be reused, edited, or removed separately.

## Troubleshooting

If **Issue Tracker Setup** is missing, verify that the solution root is versioned in Subversion.

If Generic Bugtraq cannot open an issue:

- verify that the URL contains `%BUGID%`,
- verify the resulting URL is reachable,
- inspect the inherited/direct `bugtraq:*` properties.

If Local SVN Issues cannot save:

- verify the configured path is relative to the working-copy root,
- verify the working copy is writable,
- check Pending Changes for the issue file or its parent directory.

If an external connector stops working, verify that its extension is still installed and registered.

[Commit and Pending Changes](../commit/)

[Back to AnkhSVN Help](../)
