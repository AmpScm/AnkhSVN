---
title: Annotate / Blame
---

# Annotate / Blame

**Annotate** answers a historical question: *which revision last changed each line, who committed that revision, and when?* Subversion calls the same operation **blame**, **praise**, or **annotate**.

AnkhSVN opens the selected file in Visual Studio's text editor and adds an annotation margin on the left.

## What the annotation margin shows

For each block of lines, the margin shows:

- the Subversion revision,
- the author,
- the revision date.

Hover over an annotation to see the revision, author, time, and log message. Selecting an annotation highlights that region in the margin.

Lines that represent local working-copy changes do not yet have a repository revision and are shown as local changes.

## Annotate options

The Annotate dialog can limit or adjust the history query:

- **Start revision** — the oldest revision to inspect.
- **End revision** — the newest revision to inspect. Annotating a working-copy file can include the current working state.
- **Ignore line endings** — prevents EOL-only differences from changing attribution.
- **Ignore spacing** — reduces attribution changes caused only by whitespace according to the selected spacing mode.
- **Retrieve merged revisions** — asks Subversion to follow merge history where merge tracking is available.

Ignoring whitespace can be useful after formatting-only changes, but remember that it intentionally changes how Subversion decides which revision receives credit for a line.

## Annotating from different places

AnkhSVN can annotate:

- a versioned file in the working copy,
- the active document,
- a file selected in Repository Explorer,
- a path/revision selected from history.

When you annotate a repository or log-history item, the selected repository revision becomes the endpoint rather than your current working file.

## Merged revisions

When **Retrieve merged revisions** is enabled, Subversion may attribute a line to the revision where the change originally occurred rather than only the revision that merged it into the current line of history. This is most useful in repositories that maintain reliable `svn:mergeinfo`.

## Binary files

Subversion normally rejects blame/annotate for files whose MIME type indicates binary content. If that happens, AnkhSVN can ask whether you want to continue anyway. The result is only useful when the file content is genuinely line-oriented despite its MIME classification.

## If Annotate looks wrong

Check these first:

1. Confirm that you annotated the intended path and revision.
2. If a large formatting commit changed most attribution, try ignoring whitespace/EOL differences.
3. If branch history appears to stop at a merge, try retrieving merged revisions.
4. Remember that copied/moved files can have history that depends on Subversion copy ancestry.
5. For uncommitted lines, there is no repository author or revision yet.

### Further reading

- [SVN Book: svn blame / annotate](https://svnbook.red-bean.com/en/1.8/svn.ref.svn.c.blame.html)
- [SVN Book: examining history](https://svnbook.red-bean.com/en/1.8/svn.tour.history.html)
- [Merge and merge tracking](../merge/)

[Back to AnkhSVN Help](../)
