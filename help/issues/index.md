---
title: Issue Tracking Integration
---

# Issue Tracking Integration

AnkhSVN can associate source-control work with external issue-tracking information when the project/repository is configured for it.

The issue tracker remains the system that owns the issue. AnkhSVN supplies integration around commit workflow and references.

## Typical workflow

1. Open **Pending Changes**.
2. Review the files included in the commit.
3. Select or enter the appropriate issue reference when the integration is configured.
4. Make sure the log message and issue association describe the same logical change.
5. Commit.
6. Verify the external issue tracker reflects the expected reference/state when your integration performs that action.

## Repository/project configuration

Issue integration can depend on repository properties, project configuration, or an AnkhSVN issue-tracker provider.

If issue controls are missing or empty for one project but not another, compare the repository/project configuration rather than only Visual Studio settings.

## Issue references are not a replacement for log messages

Even when an issue ID is recorded, write a useful commit message.

Repository history should remain understandable when the external issue tracker is offline, renamed, migrated, or inaccessible to a future developer.

## Troubleshooting issue integration

If the issue selector does not show expected issues:

- verify the project is configured for the intended provider,
- verify authentication to the external issue system,
- check whether the issue query/filter excludes the item,
- confirm you are operating in the intended repository/project,
- test whether the issue system itself is reachable outside AnkhSVN.

If source control succeeds but issue updates fail, treat them as separate systems when diagnosing the problem.

[Commit and Pending Changes](../commit/)

[Back to AnkhSVN Help](../)
