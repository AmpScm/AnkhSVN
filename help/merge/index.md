---
title: Merge and Merge Tracking
---

# Merge and Merge Tracking

AnkhSVN's Merge workflow applies Subversion merge operations to a **working copy**. A merge does not immediately change the repository. It produces local changes that you review, test, resolve, and then commit.

That distinction is important: the working copy receiving the merge is the **target**.

## Before merging

A clean, up-to-date target working copy is the easiest state to reason about.

Before a significant merge:

1. Commit, revert, or otherwise preserve unrelated local work.
2. Update the target working copy.
3. Verify that you are on the intended branch/path.
4. Know the exact merge source.
5. Build or test the target before the merge if you need a clean baseline.

Subversion can merge into a modified working copy, but mixing pre-existing edits with merge results makes review and conflict resolution harder.

## Sync merges

A **sync merge** brings a branch up to date with changes from its parent/source line of development, commonly bringing newer trunk changes into a feature branch before reintegration or continued work.

When merge tracking is available, Subversion can avoid reapplying revisions that are already recorded as merged. Review the source, target, and eligible revision set before starting; a sync merge should not be treated as "merge everything blindly."

## Revision-range / cherry-pick merges

Use a revision-range merge when you want changes introduced by particular revisions from a source line of history.

Typical uses include:

- bringing selected fixes from trunk to a release branch,
- synchronizing a feature branch with newer trunk revisions,
- applying a specific change without merging unrelated revisions.

When merge tracking is available, Subversion can distinguish revisions already merged from revisions still eligible to merge.

## Two-tree merges

A two-tree merge applies the difference between two repository trees or two points in history to the target working copy.

This is useful when the desired change is best described as "make this target reflect the difference between A and B" rather than "apply revisions 100 through 120."

Check the two source URLs and revisions carefully before running the operation.

## Merge tracking and svn:mergeinfo

Subversion records merge tracking in the versioned `svn:mergeinfo` property. A merge may therefore change `svn:mergeinfo` even when the visible file changes are small.

Subversion normally maintains this property automatically.

**Do not manually rewrite `svn:mergeinfo` just to make a merge appear eligible or complete unless you understand the consequences.** Incorrect mergeinfo can make later merges skip changes or attempt changes again.

Mergeinfo can also exist below the branch root as **subtree mergeinfo**, which can make merge history more complicated.

## Eligible and already-merged revisions

Merge tracking can answer two useful questions:

- Which revisions from the source have already been merged?
- Which revisions are still eligible to merge?

These answers depend on the source path, target path, ancestry, and existing mergeinfo. They are not simply "all revisions before/after a number."

## After the merge

Always treat the merge result as a working-copy change that requires review.

1. Review file and property changes.
2. Inspect unexpected adds, deletes, or moves.
3. Review `svn:mergeinfo`.
4. Resolve all conflicts.
5. Build and test.
6. Commit the merge as a coherent change.

Do not assume "merge completed" means "merge result is correct."

## Reverse merges

A merge can also apply a change in reverse, effectively undoing a previously committed change while preserving that undo as new repository history.

That is different from **Revert**, which discards uncommitted working-copy changes.

## Merges and moved/copied history

Subversion tracks ancestry through repository copy history. Moves and branch copies can affect how a merge determines related history. If a merge source was recreated rather than copied with history, merge tracking may not behave as expected.

## Common merge problems

**Unexpected conflicts** — update the target, confirm the source/range, and inspect local changes that existed before the merge.

**Revision appears already merged** — inspect mergeinfo and source/target paths before changing anything.

**Revision appears missing from eligible revisions** — verify that it changed the source path you are querying and inspect inherited/subtree mergeinfo.

**Large unexpected mergeinfo changes** — check whether the operation touched subtrees with their own merge history.

### Further reading

- [SVN Book: basic merging and mergeinfo](https://svnbook.red-bean.com/en/1.8/svn.branchmerge.basicmerging.html)
- [SVN Book: advanced merging](https://svnbook.red-bean.com/en/1.8/svn.branchmerge.advanced.html)
- [Conflicts and Resolve](../conflicts/)

[Back to AnkhSVN Help](../)
