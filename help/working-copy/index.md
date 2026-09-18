---
title: Working Copy Operations
---

# Working Copy Operations

A Subversion **working copy** is more than a folder of files. It records repository URLs, BASE revisions, pristine state, properties, and local scheduling information so Subversion can determine what changed.

AnkhSVN exposes the common working-copy operations directly in Visual Studio.

## Update

**Update** brings repository changes into the working copy and can move items to another repository revision.

Update can:

- modify files,
- add/delete paths,
- change properties,
- create conflicts,
- update externals,
- leave different items at different BASE revisions when a particular revision/depth is used.

Updating does not discard normal local edits; Subversion attempts to combine them with incoming repository changes.

## Update to revision

Updating to an older numbered revision is useful for investigation and reproduction, but it is not the same as undoing history.

To create a new commit that reverses an old repository change, use an appropriate reverse merge rather than merely updating the working copy backward.

## Switch

**Switch** changes a working-copy path so it tracks a different repository URL while preserving its working-copy role.

A common use is moving a working copy between trunk and a branch in the **same repository**.

Before switching:

- preserve unrelated local work,
- verify the destination URL,
- expect conflicts if local changes overlap differences between the old and new locations.

If the repository itself moved to a new server URL without changing logical history, that may be a **relocate** situation rather than a branch switch.

## Revert

**Revert is destructive.** It discards selected uncommitted working-copy changes and restores Subversion's recorded state.

Examples:

- reverting a modified file discards local edits,
- reverting a scheduled add unschedules it (the local file may remain unversioned),
- reverting a scheduled delete restores the versioned item.

Subversion does not provide a repository revision for edits you never committed. Save a patch/copy first if you may need the work later.

## Cleanup

Cleanup repairs working-copy administrative state after interrupted or failed Subversion operations.

Use Cleanup for errors such as a working copy being administratively locked or an operation having been interrupted.

A Subversion **working-copy lock** is not the same thing as a repository file lock held by a user.

Run Cleanup at an appropriate working-copy root when an interrupted operation affected multiple children.

## Lock / Unlock

A repository **Lock** reserves a versioned path so another user normally cannot commit to that path until the lock is released or broken.

This is mainly useful for files that cannot be merged safely.

The `svn:needs-lock` property can make a working-copy file read-only until a lock is obtained, serving as a reminder to lock before editing.

Do not confuse this with the internal working-copy locks repaired by Cleanup.

## Resolve

Resolve records your decision after handling a conflict.

Do not mark an item resolved merely to make the warning disappear. First make the working content/properties/tree state correct, then mark it resolved.

See [Conflicts and Resolve](../conflicts/).

## BASE vs HEAD

**BASE** is the repository revision currently recorded for a working-copy item.

**HEAD** is the newest repository revision.

They are often different. A working copy is not automatically at HEAD simply because it was committed successfully.

## Status before destructive operations

Before a large Update, Switch, Revert, or Cleanup action, check Pending Changes/status so you know what local work exists.

### Further reading

- [SVN Book: basic work cycle](https://svnbook.red-bean.com/en/1.8/svn.tour.cycle.html)
- [TortoiseSVN: Cleanup](https://www.tortoisesvn.net/docs/release/TortoiseSVN_en/tsvn-dug-cleanup.html)
- [TortoiseSVN: Locking](https://tortoisesvn.net/docs/release/TortoiseSVN_en/tsvn-dug-locking.html)

[Back to AnkhSVN Help](../)
