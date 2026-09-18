---
title: Commit and Pending Changes
---

# Commit and Pending Changes

The **Pending Changes** window is the main place to review local Subversion changes before sending them to the repository.

A commit publishes selected working-copy changes as one repository revision. Until the commit succeeds, edits, adds, deletes, moves, and property changes remain local.

**Add is not Commit.** Adding or scheduling a path only changes the working copy; nothing is published to repository history until commit succeeds.

## Recommended commit workflow

1. **Update or check status** so you understand changes made by other people.
2. Review modified, added, deleted, moved, conflicted, and property-changed items.
3. Diff important files before committing.
4. Select only the files that belong to this logical change.
5. Resolve any conflicts.
6. Enter a useful log message that explains *why* the change was made.
7. Review issue references or changelists if your project uses them.
8. Commit.
9. Check Pending Changes afterward for anything you expected to include but did not.

## What happens during commit

Subversion sends your selected working-copy changes to the repository as a transaction. A successful repository commit is **atomic**: the selected changes become one new repository revision together. A failed commit does not partially publish some of the selected changes as a normal successful revision.

Committing does **not** automatically update every untouched path in your working copy to HEAD. Your working copy can therefore contain mixed BASE revisions.

## Out-of-date errors

Subversion protects repository history from blindly overwriting newer changes. If a path you are trying to commit is out of date, the commit may be rejected.

When that happens:

1. Preserve any important local work.
2. Update the affected working copy.
3. Resolve any conflicts created by the update.
4. Review and test the combined result.
5. Commit again.

An update is not a substitute for reviewing what changed.

## Added and deleted files

**Add** schedules an unversioned item for addition. The file does not exist in repository history until commit.

**Delete** schedules a versioned item for deletion. The repository still contains its prior history; the deletion becomes part of history when committed.

Moves and renames are also working-copy changes until committed.

## Property changes

Properties such as `svn:ignore`, `svn:externals`, `svn:eol-style`, and `svn:mergeinfo` are versioned changes too. Review them alongside file-content changes.

## Changelists

Subversion changelists are **local organizational labels** for working-copy files. They are useful when your working copy contains changes for more than one task.

Changelists:

- do not create repository branches,
- do not become repository history,
- are not shared with other developers,
- do not allow two unrelated changes to the same file to be separated automatically.

## Before committing a merge

A merge changes your working copy first. Before committing a merge:

- review the diff,
- verify any `svn:mergeinfo` changes,
- resolve conflicts,
- build/test the result,
- avoid mixing unrelated local edits into the merge commit.

See [Merge and merge tracking](../merge/).

## Before committing generated or unexpected files

If an item appeared unexpectedly, determine whether it should be versioned before selecting it. Build output, caches, local settings, secrets, and machine-specific files usually belong in ignore rules rather than repository history.

### Further reading

- [SVN Book: basic work cycle](https://svnbook.red-bean.com/en/1.8/svn.tour.cycle.html)
- [SVN Book: commit](https://svnbook.red-bean.com/en/1.8/svn.ref.svn.c.commit.html)
- [Conflicts and Resolve](../conflicts/)

[Back to AnkhSVN Help](../)
