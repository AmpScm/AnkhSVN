---
title: Troubleshooting AnkhSVN
---

# Troubleshooting AnkhSVN

Start by separating the problem into one of four areas:

1. **Visual Studio / AnkhSVN UI**
2. **local Subversion working copy**
3. **network/authentication**
4. **repository/server**

That prevents a UI symptom from being mistaken for repository corruption.

## Quick diagnostic checklist

1. Record the exact error text.
2. Confirm the AnkhSVN and Visual Studio versions.
3. Check Pending Changes/status before making destructive repairs.
4. Verify the working copy with another current Subversion client when possible.
5. Verify the exact repository URL.
6. Check whether the problem affects one path, one working copy, or every repository.
7. Check externals separately if the operation reaches additional repositories.

## Common symptoms

### Working copy locked / previous operation did not finish

An interrupted update, commit, merge, or similar operation can leave internal working-copy administrative locks.

Use **Cleanup** at an appropriate working-copy root.

This is different from a repository lock held by another user.

### Tree conflict

A structural operation collided with another structural/local change.

Do not simply mark it resolved. Decide what the final file/folder location and content should be, repair the tree, then mark it resolved.

See [Conflicts and Resolve](../conflicts/).

### Text/property conflict

Resolve the actual content/property difference first. Then mark the item resolved.

### Commit says item is out of date

The repository contains a newer relevant change.

Update, resolve any resulting conflicts, review/test, then commit again.

### Not a working copy

Verify that you selected a path inside the actual checkout and that its working-copy administrative data has not been deleted or replaced.

Copying only project files to a new directory does not create a Subversion working copy.

### Repository moved or URL changed

If the same repository was moved to a new server or base URL, do not use **Switch** merely because the URL changed. Switch is normally for changing the working-copy path to another branch/location within the same repository history.

A repository relocation operation is appropriate when the repository identity/history is the same and only its root URL changed. Verify the old and new repository URLs carefully before changing working-copy metadata.

If repository identity changed as well, a fresh checkout may be safer than pretending the new repository is the old one.

### Path not found / repository path missing

Check:

- spelling and case,
- current repository URL,
- whether the path was moved/deleted,
- revision being browsed,
- permissions that may hide the path.

### Authentication failed

Check the repository URL, username, cached credentials, server access, and proxy/TLS configuration.

If only an external fails, remember it may contact a different server.

### Certificate / TLS error

Verify hostname, expiration, issuing CA, and whether a corporate proxy is intercepting TLS. Do not permanently accept an unexpected identity merely to make the warning disappear.

### Obstructed path

An unversioned local file/folder may occupy a location where Update/Checkout needs to create a versioned item.

Preserve anything important, identify why the obstruction exists, then move/remove it as appropriate.

### Externals fail during Update

Inspect the `svn:externals` definition and test each external URL. Check credentials and whether the destination already exists.

See [Externals](../externals/).

### Status icons / Pending Changes do not reflect the file state

If Visual Studio does not show the status you expect:

- refresh Pending Changes/status,
- verify the file is inside the working-copy root,
- verify the file is not part of a separate external working copy,
- check whether it is ignored or unversioned,
- confirm another Subversion client reports the same status,
- reopen the solution or Visual Studio if the working copy changed substantially outside the IDE.

If command-line or another SVN client also reports unexpected status, investigate the working copy rather than treating it as only a Visual Studio display problem.

### Expected file does not appear in Pending Changes

Check whether it is:

- ignored,
- outside the working-copy root,
- unchanged according to Subversion,
- inside a separate external working copy,
- excluded by a view/filter/changelist.

### Annotate shows unexpected authors/revisions

Try the appropriate whitespace/EOL options, verify the revision range, and consider retrieving merged revisions.

See [Annotate / Blame](../annotate/).

### Merge repeats or skips revisions unexpectedly

Inspect source/target URLs and `svn:mergeinfo`. Do not edit mergeinfo solely to force the desired eligibility result.

See [Merge and merge tracking](../merge/).

## Before deleting working-copy metadata or checking out again

A fresh checkout can be a useful comparison, but do not destroy the original working copy until all uncommitted local changes have been identified and preserved.

## Reporting an AnkhSVN problem

Include:

- AnkhSVN version,
- Visual Studio version,
- Windows version when UI/DPI behavior matters,
- operation being performed,
- exact error text,
- whether another Subversion client reproduces it,
- repository URL shape/protocol with secrets removed,
- screenshot for UI problems,
- minimal reproduction steps.

Check [AnkhSVN GitHub issues](https://github.com/AmpScm/AnkhSVN/issues) for known problems.

### Further reading

- [SVN Book: basic work cycle](https://svnbook.red-bean.com/en/1.8/svn.tour.cycle.html)
- [Working-copy operations](../working-copy/)
- [Settings](../settings/)

[Back to AnkhSVN Help](../)
