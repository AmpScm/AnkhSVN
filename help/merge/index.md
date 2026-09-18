---
title: Merge and Merge Tracking
---

# Merge and Merge Tracking

AnkhSVN's merge workflow exposes Subversion merge operations inside Visual Studio.

## Typical workflow

1. Update the working copy that will receive the merge.
2. Open the **Merge** command for the target working copy.
3. Choose the merge source and revision range or tree comparison.
4. Review the selected source and revisions before starting the merge.
5. Resolve any conflicts, test the result, and commit the merged changes.

The merge wizard supports the normal Subversion merge scenarios, including revision-range and two-tree merges. Merge tracking information is stored in the standard `svn:mergeinfo` property when the repository and operation use merge tracking.

If the merge wizard is missing controls or appears blank, use the current AnkhSVN build; a regression in the wizard's embedded resources was fixed in the 2.9.x development line.

[Back to AnkhSVN Help](../)
