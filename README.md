# AnkhSVN - Subversion Support for Visual Studio

![Build](badges/build.svg)
![Tests](badges/tests.svg)
![Line coverage](badges/coverage.svg)
![Branch coverage](badges/branches.svg)

[View detailed coverage report](CoverageReport/SummaryGithub.md)

AnkhSVN integrates Apache Subversion working-copy and repository operations directly into Visual Studio.

> **Supported Visual Studio versions:** Visual Studio 2022 and later. Current packages are x64-only; the pre-VS2022 package and test infrastructure have been retired.

AnkhSVN is actively maintained with ongoing modernization, documentation, regression testing, and Visual Studio compatibility improvements.

## Highlights

- Work with Subversion working copies from inside Visual Studio.
- Review pending changes, diff files, commit, update, revert, switch, and resolve conflicts.
- Browse repositories and check out projects through Repository Explorer.
- Use Annotate / Blame, merge tracking, properties, externals, Local SVN Issues, Generic Bugtraq, and external issue-tracker connectors.
- Build and test against the current Visual Studio 2022+ toolchain.
- Run regression tests and merged line/branch coverage on every CI build.

## Getting started

### Install

Current builds are published through this repository's [GitHub Releases](../../releases).

Download the Visual Studio 2022+ VSIX from the latest release, install it, and restart Visual Studio if prompted.

### Open a Subversion project

From Visual Studio:

1. Choose **File -> Open -> Subversion Project**.
2. Select the repository project or solution.
3. Choose a local working-copy location.
4. Open the resulting solution.

For repository and working-copy behavior, start with the [AnkhSVN Help index](help/index.md).

### Open the source

The main solution is:

```text
src/AnkhSvn.sln
```

See [Building AnkhSVN](src/BUILD.md) for prerequisites, build commands, test commands, and debugging information.

## Documentation

### Project and development

- [Building AnkhSVN](src/BUILD.md) — requirements, build/test commands, VSIX output, and development notes.
- [Issue status](ISSUE_STATUS.md) — current issue-triage and fix-status snapshot.
- [Code of Conduct](CODE_OF_CONDUCT.md) — contributor community standards.

### User help

- [AnkhSVN Help](help/index.md) — help index and Subversion terminology.
- [Annotate / Blame](help/annotate/index.md) — revision attribution and annotation options.
- [Commit and Pending Changes](help/commit/index.md) — reviewing and committing working-copy changes.
- [Conflicts and Resolve](help/conflicts/index.md) — text, property, and tree conflicts.
- [Subversion Externals](help/externals/index.md) — external working copies and reproducible definitions.
- [Issue Tracking Integration](help/issues/index.md) — Local SVN Issues, Generic Bugtraq, changing/removing trackers, and external connector integration.
- [Merge and Merge Tracking](help/merge/index.md) — sync merges, revision ranges, mergeinfo, and review workflow.
- [Subversion Properties](help/properties/index.md) — common versioned properties and property changes.
- [Repository Explorer and Checkout](help/repository/index.md) — repository URLs, history, checkout, and export.
- [AnkhSVN Settings](help/settings/index.md) — authentication, proxy, certificates, and external tools.
- [Source Control Setup](help/source-control/index.md) — adding solutions and repairing source-control associations.
- [Troubleshooting AnkhSVN](help/troubleshooting/index.md) — diagnostic workflow and common failure modes.
- [Working Copy Operations](help/working-copy/index.md) — update, switch, revert, cleanup, locks, and working-copy state.

## Build, tests, and coverage

The GitHub Actions workflow builds the Visual Studio 2022+ solution, runs both active test projects, and merges their coverage results into one report.

```bat
dotnet test src\Ankh.Tests\Ankh.Tests.csproj --configuration Release --no-build --no-restore -- RunConfiguration.TreatNoTestsAsError=true

dotnet test src\Ankh.VS.UnitTest\Ankh.VS.UnitTest.csproj --configuration Release --no-build --no-restore -- RunConfiguration.TreatNoTestsAsError=true
```

The badges at the top of this README are updated from the latest main-branch CI results. Full HTML/Cobertura coverage reports are also attached to CI runs and release artifacts.

## Contributing

Bug reports, fixes, tests, documentation improvements, and compatibility updates are welcome.

Use [GitHub Issues](../../issues) for bug reports and feature requests, and [Pull Requests](../../pulls) for proposed changes.

## License

AnkhSVN is licensed under the [Apache License 2.0](LICENSE).
