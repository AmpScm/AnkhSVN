---
title: Subversion Externals
---

# Subversion Externals

The `svn:externals` property lets one working-copy tree include additional versioned directories or files from another repository location.

An external is effectively another checkout managed as part of the surrounding working-copy workflow.

## Why use externals?

Typical uses include:

- shared libraries,
- common build tools,
- dependencies maintained elsewhere,
- common content reused by several projects.

The external definition is versioned because it is stored in a normal Subversion property.

## Prefer pinned revisions for reproducible builds

An external that follows a moving HEAD can change even when the parent project's own revision has not changed.

For reproducible historical builds, define externals so the dependency can be recreated at a known revision when practical.

This is especially important for release branches and tags.

## Update behavior

When a directory with `svn:externals` is updated, Subversion can also update the external working copies described by that property.

Therefore an apparently simple parent update may contact additional repository URLs and change files beneath external directories.

## Same-repository vs cross-repository externals

Externals may refer to another location in the same repository or to a different repository.

Cross-repository externals have an important consequence: repository commits cannot become one atomic revision spanning two independent repositories. Changes in a different-repository external must be committed to that repository separately.

## Editing an external definition

Changing `svn:externals` changes the definition, not necessarily every already-checked-out external immediately.

After committing or receiving a changed definition, update the parent working copy so Subversion can synchronize the external checkout.

## Common external problems

**External path already exists** — an unversioned or unrelated local directory may obstruct the checkout target.

**Authentication succeeds for the parent but fails for the external** — the external may use a different server/repository and therefore different credentials.

**Build changes unexpectedly after update** — check whether an unpinned external advanced to a newer revision.

**Commit does not include external changes** — verify whether the external is a separate working copy and whether it points to a different repository.

**External URL stopped working after repository move** — inspect whether the definition uses an absolute URL or an appropriate relative URL.

## Externals and tags

A tag that depends on floating externals may not reproduce the dependency versions that existed when the tag was created. Review external definitions when creating long-lived release snapshots.

### Further reading

- [SVN Book: externals definitions](https://svnbook.red-bean.com/en/1.8/svn.advanced.externals.html)
- [Subversion properties](../properties/)
- [Working-copy operations](../working-copy/)

[Back to AnkhSVN Help](../)
