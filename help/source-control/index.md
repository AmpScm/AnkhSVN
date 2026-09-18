---
title: Source Control Setup
---

# Source Control Setup

AnkhSVN connects Visual Studio solutions/projects with Subversion working-copy state. It is useful to distinguish three related things:

1. the files on disk,
2. the Subversion working copy containing them,
3. Visual Studio / AnkhSVN's understanding of that working copy.

A problem with one layer does not always mean the other two are damaged.

## Adding a solution to Subversion

Typical workflow:

1. Place the solution beneath the intended working-copy/repository layout.
2. Use **Add Solution to Subversion** or the appropriate source-control command.
3. Review the proposed repository location.
4. Review which files should actually be versioned.
5. Add/schedule the files.
6. Commit them.

**Add is not Commit.** Scheduling a file for addition only changes the working copy. Repository history begins when the addition is committed.

## Files that should not be added

Before the first commit, identify generated or machine-specific content such as:

- build output,
- caches,
- user-specific IDE state,
- temporary files,
- local secrets/configuration.

Use appropriate ignore rules rather than committing them accidentally.

## Moving or renaming a solution

Subversion tracks versioned moves/renames through working-copy and repository history.

After moving a solution or project, AnkhSVN may need to rediscover the correct working-copy root or source-control association. Avoid "repairing" a move by deleting and re-adding files if retaining history matters.

## Change Source Control

**Change Source Control** can repair or change the Visual Studio/AnkhSVN association when:

- a solution moved,
- the detected working-copy root is wrong,
- a project is no longer associated with the expected repository location,
- source-control metadata/binding no longer reflects the actual working copy.

It should not be used to disguise a broken working copy. If command-line Subversion also reports the working copy as invalid, repair the working copy itself first.

## Existing working copies

You do not normally need to re-add files just because you opened an existing valid Subversion working copy in Visual Studio.

If AnkhSVN is not recognizing it:

1. verify the directory is a valid working copy,
2. check the selected source-control provider/integration,
3. inspect the solution/project path relative to the working-copy root,
4. use Change Source Control when the Visual Studio association is the part that is wrong.

## Repository layout

If your repository uses `trunk`, `branches`, and `tags`, choose the intended destination before the initial add. Moving the project later is possible, but planning the layout avoids unnecessary history operations.

### Further reading

- [SVN Book: getting data into the repository](https://svnbook.red-bean.com/en/1.8/svn.tour.importing.html)
- [Repository Explorer and Checkout](../repository/)
- [Subversion Properties and ignores](../properties/)

[Back to AnkhSVN Help](../)
