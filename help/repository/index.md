---
title: Repository Explorer and Checkout
---

# Repository Explorer and Checkout

**Repository Explorer** works with repository URLs rather than only the local working copy. Use it to browse versioned folders/files, inspect history, and start repository-oriented operations.

## Repository URLs

A repository URL identifies a versioned location on a Subversion server. The same repository can be reachable through HTTP/HTTPS, svn protocols, or local/file access depending on server configuration.

Authentication and authorization can allow one path while denying another.

## HEAD and numbered revisions

**HEAD** means the youngest repository revision at the time of the request.

A numbered revision gives you a stable historical point. Browsing or checking out a numbered revision is useful when reproducing an older state.

Remember that a Subversion revision number identifies a repository-wide transaction, while a particular path may or may not have changed in that revision.

## Checkout

Checkout creates a Subversion working copy with administrative metadata and versioned state.

Typical workflow:

1. Select the repository URL/path.
2. Choose the local destination.
3. Select HEAD or a specific revision.
4. Complete checkout.
5. Open the solution/project from the resulting working copy.

Choose a destination that will not obstruct the paths Subversion needs to create.

## Checkout vs Export

A **checkout** creates a working copy that can be updated, committed, switched, and otherwise managed by Subversion.

An **export** produces ordinary files without normal working-copy metadata. Use export for a clean snapshot when you do not intend to perform working-copy operations there.

## Common repository layout

Many Subversion repositories use a convention such as:

- `trunk/`
- `branches/`
- `tags/`

This is a convention, not a server requirement. AnkhSVN should use the actual repository layout rather than assuming every repository follows it.

## Copy history, branches, and tags

Subversion branches and tags are commonly created as repository copies. Because the copy is recorded in history, tools such as Log and Annotate can often follow ancestry across the copy.

A folder manually recreated at another path is not equivalent to a repository copy with history.

## Sparse/depth working copies

Subversion supports working copies where only part of a tree is present at a chosen depth. If expected children appear to be missing, check whether the working copy was intentionally created or updated with limited depth.

## Peg revisions and renamed paths

A path's current URL is not always enough to identify older history after renames or moves. Subversion can use a **peg revision** to identify the historical line of ancestry being requested.

This is why a historical command may include both a path and a revision context.

## Externals during checkout/update

A checkout or update can also process `svn:externals` definitions below the selected tree. That means one repository operation can contact additional URLs, create additional working copies, and require additional credentials.

If the primary checkout succeeds but the overall operation reports failures, inspect the external definitions and test those URLs separately.

See [Externals](../externals/).

## Repository access problems

If browsing works but checkout fails, or vice versa, check:

- exact URL,
- credentials,
- server authorization,
- proxy/TLS configuration,
- destination path permissions,
- externals that contact additional locations.

### Further reading

- [SVN Book: creating a working copy](https://svnbook.red-bean.com/en/1.8/svn.tour.initial.html)
- [SVN Book: browsing repository history](https://svnbook.red-bean.com/en/1.8/svn.tour.history.html)
- [Externals](../externals/)

[Back to AnkhSVN Help](../)
