---
title: Conflicts and Resolve
---

# Conflicts and Resolve

A conflict means Subversion could not safely combine repository history with your working-copy state without your decision.

Conflicts commonly appear during **Update**, **Switch**, and **Merge**.

## Types of conflicts

### Text conflicts

Two changes affect overlapping content in a text file. Subversion may place conflict markers in the working file and preserve comparison files representing your local content and repository revisions.

### Property conflicts

Both sides changed the same versioned property in incompatible ways. The file content may be fine while a property such as `svn:externals` or another custom property remains conflicted.

### Tree conflicts

The conflict is structural rather than just textual—for example:

- one side deletes a file while the other modifies it,
- both sides move or rename the same item differently,
- a local item obstructs a path the repository needs to create,
- directory structure changed incompatibly.

Tree conflicts deserve extra care because choosing the correct final repository structure is part of the resolution.

## Resolve does not decide the answer for you

The important distinction is:

**Fix the content or structure first; mark it resolved second.**

Marking a conflict **Resolved** tells Subversion that you have handled the conflict and that the item may participate in later operations such as commit. It does not automatically determine which version of the content is correct.

## Safe conflict workflow

1. Identify whether the conflict is text, property, or tree related.
2. Preserve a copy of important local work if you are uncertain.
3. Compare your local changes, the incoming repository changes, and the common/base content.
4. Edit or merge until the working copy contains the result you actually want.
5. Build or test when appropriate.
6. Mark the conflict resolved.
7. Review the final diff before commit.

## Mine, theirs, BASE, and working content

Conflict tools often use terms such as:

- **mine / working** — your local working-copy content,
- **BASE** — the pristine revision your working copy started from,
- **theirs / incoming** — content received from the repository operation.

The exact labels vary by conflict tool. Do not choose solely by label—verify the content.

## Revert is not Resolve

**Revert** discards local working-copy changes. It can be a valid way to abandon your side of a conflict, but it is destructive.

If the local edits matter, save or diff them before reverting.

## Binary conflicts

Subversion cannot line-merge arbitrary binary files. Resolution usually means explicitly choosing or recreating the desired final binary content, then marking the conflict resolved.

## Tree-conflict questions to answer

Before resolving a tree conflict, decide:

- Should the item exist at all?
- What should its final repository path be?
- Was the item intentionally renamed or moved?
- Are there local modifications that must be preserved?
- Did an unversioned local file obstruct an incoming versioned path?

### Further reading

- [SVN Book: resolve conflicts](https://svnbook.red-bean.com/en/1.8/svn.tour.cycle.html#svn.tour.cycle.resolve)
- [TortoiseSVN: resolving conflicts](https://tortoisesvn.net/docs/release/TortoiseSVN_en/tsvn-dug-conflicts.html)
- [Working-copy operations](../working-copy/)

[Back to AnkhSVN Help](../)
