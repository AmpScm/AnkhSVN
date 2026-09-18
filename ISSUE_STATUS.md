# AnkhSVN Issue Status

Status review of the upstream [AmpScm/AnkhSVN issue tracker](https://github.com/AmpScm/AnkhSVN/issues).

**Snapshot date:** September 17, 2026

This file is a triage snapshot, not a replacement for the GitHub issue tracker. The GitHub issue remains the source of truth for discussion and reproduction details.

## Resolved upstream but still open in the tracker

### #81 — Visual Studio 2026 support

**Status: Resolved in upstream v2.9.191.**

Visual Studio 2026 support was published upstream. The later VS 2026 Update 3 Commit-window layout problem reported in the same issue was also addressed, and a reporter confirmed that the controls now move correctly when the window is resized.

Issue: https://github.com/AmpScm/AnkhSVN/issues/81

Recommended tracker action: close as completed.

### #31 — How commit unmodified file to SVN

**Status: Answered support question; not an AnkhSVN defect.**

The issue already contains an answer explaining the Subversion behavior and points to the relevant workaround.

Issue: https://github.com/AmpScm/AnkhSVN/issues/31

Recommended tracker action: close as not planned / support question.

## Fixed on ARMoir/main, pending upstream merge

### #88 — Merge Branch UI Missing in latest Version

**Status: Fixed and regression-tested on ARMoir/AnkhSVN main.**

Root cause: the SDK-style project emitted `MergeWizard/WizardFramework/Wizard.resx` with a path-derived manifest resource name instead of the runtime name expected by `Ankh.UI.WizardFramework.Wizard`. That caused a `MissingManifestResourceException` and left the Merge wizard UI blank.

The fix explicitly emits the resource as:

`Ankh.UI.WizardFramework.Wizard.resources`

A regression test verifies the manifest resource and constructs the Wizard UI. CI is green.

Fork merge commit: https://github.com/ARMoir/AnkhSVN/commit/16af7d2aa1dce5c18aab3d81825e50d2c352816c

Issue: https://github.com/AmpScm/AnkhSVN/issues/88

Recommended tracker action: keep open until the fix is merged upstream, then close as completed.

## Awaiting confirmation against the current upstream build

These issues have been explicitly asked to retest against `v2.9.191`, but no reporter confirmation has been recorded yet.

- **#17 — Pending Changes ignoring some files in VS2019**  
  https://github.com/AmpScm/AnkhSVN/issues/17  
  Related to project/file discovery and Pending Changes refresh behavior.

- **#65 — Pending changes are not reflected in VS2019**  
  https://github.com/AmpScm/AnkhSVN/issues/65  
  Strong overlap with #17, especially for files changed outside Visual Studio.

- **#80 — Revision status doesn't update from external file structure changes**  
  https://github.com/AmpScm/AnkhSVN/issues/80  
  Also overlaps the Pending Changes/status-cache refresh family represented by #17 and #65.

- **#85 — PendingChangeSelector UI error when revert**  
  https://github.com/AmpScm/AnkhSVN/issues/85  
  Reporter was asked to verify `v2.9.191`; no confirmation is recorded yet.

These should remain open until somebody reproduces them on the current build or confirms they are resolved.

## Outstanding functional issues

### Annotate viewer

- **#7 — Annotate viewer broken under mixed-DPI**  
  https://github.com/AmpScm/AnkhSVN/issues/7

- **#47 — Annotate screen doesn't work VS2022**  
  https://github.com/AmpScm/AnkhSVN/issues/47

These appear closely related. The discussion around #7 already describes a more modern WPF-based Annotate implementation as a possible replacement for the old WinForms/editor-hosting approach.

### Pending Changes / commit selection

- **#52 — Double-clicking entry in changes list toggles selected status**  
  https://github.com/AmpScm/AnkhSVN/issues/52

A very similar issue, #26, was closed after PR #46, but later reports in #52 show the behavior still occurs. Treat #52 as unresolved rather than assuming the old fix fully solved it.

- **#22 — Conflicting files get added**  
  https://github.com/AmpScm/AnkhSVN/issues/22

Conflict artifacts such as `.mine` and revision files can be automatically added when auto-add is enabled.

### Source-control provider / lifecycle

- **#53 — Source control provider associated with this solution could not be found**  
  https://github.com/AmpScm/AnkhSVN/issues/53

Still has reports against later Visual Studio 2022 versions. It resembles older provider-binding issues but has not been demonstrated as resolved.

- **#64 — Visual Studio 2022 devenv.exe not closing**  
  https://github.com/AmpScm/AnkhSVN/issues/64

The reporter confirmed that a fix in the external FallenWorlds/AnkhSVN 3.0.1 fork solved the process-exit problem. An equivalent fix has not been verified as present in AmpScm/AnkhSVN, so the upstream issue should remain open until that fix is ported or independently reproduced as resolved.

### Explorer/status indicators

- **#56 — New file indicator missing from Solution Explorer**  
  https://github.com/AmpScm/AnkhSVN/issues/56

The thread asks whether a related setting is enabled, but no resolution or reporter confirmation follows.

- **#37 — Conflict folder/files not detected**  
  https://github.com/AmpScm/AnkhSVN/issues/37

Needs a current reproduction and clearer steps before a targeted fix can be designed.

## Outstanding UI / DPI / theme issues

- **#27 — VS dark mode and history viewer colors**  
  https://github.com/AmpScm/AnkhSVN/issues/27

- **#33 — Bad file icons in Pending Changes with 200% DPI**  
  https://github.com/AmpScm/AnkhSVN/issues/33

These are still open and have no recorded current-build resolution.

## Suggested consolidation

The following issue groups likely belong to the same engineering workstreams and should be investigated together:

1. **Pending Changes/status refresh:** #17, #65, #80
2. **Annotate viewer modernization:** #7, #47
3. **Pending Changes selection behavior:** #52, with historical context from closed #26 and #43
4. **Visual Studio scaling/layout:** #27, #33, #85

## Recent completed work not currently represented by an upstream issue

The following work is complete on `ARMoir/AnkhSVN` `main` but does not currently map to a dedicated open AmpScm issue:

- Branch Solution now rejects cross-repository destinations before SharpSvn throws a cross-repository copy exception.
- In-app Help no longer uses the retired `svc.ankhsvn.net` service and now targets GitHub Pages documentation.
- GitHub Actions setup dependencies were modernized and actionable build warnings were reduced.
- The NuGet package now includes and declares the repository README.
- CI builds, tests, verifies the VSIX, packages, and publishes successfully.

