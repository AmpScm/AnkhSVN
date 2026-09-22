---
title: Unified Diff
---

# Unified Diff

The **Unified Diff** command generates a patch-style text comparison for the selected Subversion items and opens the result in Visual Studio.

## Selecting files

The list at the top of the dialog contains the files that can be included in the diff. Checked items are written to the generated patch; unchecked items are omitted.

Versioned files can be included directly. An unversioned file that is versionable and belongs to the solution can also be included; AnkhSVN schedules it for add so Subversion can represent the new file in the generated diff.

## From and To revisions

The two revision selectors define the comparison range.

The normal working-copy comparison is:

- **From: BASE** — the repository revision recorded for the working-copy item before local edits.
- **To: Working** — the current local working-copy content.

You can choose other supported revision kinds or numbered revisions when you need to compare a different range.

A working copy can contain mixed BASE revisions, so BASE is evaluated for the selected working-copy items rather than assuming that every file is at the same repository revision.

## Suppressing the dialog

Hold **Shift** while invoking Unified Diff to suppress the selection dialog and use the command's current/default selection and revision range.

## Output

AnkhSVN asks Subversion to generate a unified patch and opens the resulting temporary `.patch` file in Visual Studio.

The patch is comparison output; viewing it does not commit anything to the repository.

If an individual diff operation fails, AnkhSVN can include the error text in the generated patch output rather than discarding the entire result.

## Related help

- [Working-copy operations](../working-copy/)
- [Commit and Pending Changes](../commit/)
- [Conflicts and Resolve](../conflicts/)

[Back to AnkhSVN Help](../)
