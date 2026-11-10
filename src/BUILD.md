# Building AnkhSVN

AnkhSVN now targets **Visual Studio 2022 and later**. The pre-VS2022 package, x86 VSIX, Visual Studio IDE-host test harnesses, and older SDK-specific build paths have been removed.

## Requirements

- Visual Studio 2022 or later.
- The **Visual Studio extension development** workload.
- .NET Framework 4.8 targeting support.
- Git.
- A Subversion command-line client is recommended for development and is installed by CI.

The solution restores the Visual Studio SDK, VSSDK Build Tools, SharpSvn, and test dependencies through NuGet.

## Temporary SharpSvn SASL prerelease

Both SharpSvn packages are pinned to exactly `1.14005.404-saslfix.1`.
The repository's `NuGet.Config` maps only `SharpSvn` and `SharpSvn.UI` to
[ARMoir GitHub Packages](https://nuget.pkg.github.com/ARMoir/index.json).
Other dependencies restore from nuget.org. Use NuGet 6+/Visual Studio 2022+
so package source mapping is honored.

For local builds, supply a GitHub personal access token **(classic)** with
`read:packages` and access to these packages. Keep it outside this repository.
For example, set this environment variable in the shell used to start the build
(substitute your username and token locally; do not commit them):

```powershell
$env:NuGetPackageSourceCredentials_ARMoir = 'Username=YOUR_GITHUB_USERNAME;Password=YOUR_TOKEN;ValidAuthenticationTypes=Basic'
```

Restart Visual Studio from that shell if building in the IDE. CI supplies its
`GITHUB_TOKEN`; these packages are associated with ARMoir/AnkhSVN so the
publishing repository also has restore access. A separate fork needs package
access and its own credential. Installing the finished VSIX needs no feed token.
See [GitHub's NuGet authentication documentation](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry).

The binaries come from **exact SharpSvn commit**
`826b52eaa7258d63cf01997f090873addd85ab64`, built and tested in
[SharpSvn run 41120418118](https://github.com/ARMoir/SharpSvn/actions/runs/41120418118).
The `Publish pinned SharpSvn` workflow reuses that completed build's packages;
it never checks out a moving SharpSvn branch or rebuilds SharpSvn for an AnkhSVN
build. It verifies the run's commit and success, the artifact ID and SHA256,
and the source commit in each package. It changes only package metadata and
adds `docs/SOURCE.json`, then checks every other payload entry is byte-identical.
SharpSvn.UI requires exactly the same SharpSvn prerelease.
The source artifacts were numbered `1.14005.1` by that fork's first CI run;
the distinct NuGet prerelease version does not change the tested assembly versions.

The packages are published once to the feed. Normal restores do not depend on
the original Actions artifact, which expires on 2027-01-28. The publication
workflow fails if that artifact is unavailable or a package version already
exists; it never silently substitutes a newer build. Its output artifact
contains both packages, provenance, and SHA256 checksums. Preserve those outputs
if another publication is needed after the source artifact expires.

Tracking: [ARMoir/AnkhSVN #25](https://github.com/ARMoir/AnkhSVN/issues/25);
related report: [AmpScm/AnkhSVN #99](https://github.com/AmpScm/AnkhSVN/issues/99).
The local regression covers DIGEST-MD5, not every possible SASL mechanism.

Once an official SharpSvn release contains the fix, validate it with
`testing/sasl/Test-SharpSvn.ps1`, update both versions in
`src/Directory.Packages.props`, remove the ARMoir source/mapping from
`NuGet.Config` and the CI credential, and retire the prerelease publication
workflow/script. Run the solution build, both test suites, and VSIX checks.

## Build

Open `src/AnkhSvn.sln` in Visual Studio 2022 or later and build the solution, or build from a Developer Command Prompt with:

```bat
msbuild /m /restore /t:rebuild /p:UseVsSdkVersion=17.0 /p:Configuration=Release src\AnkhSvn.sln
```

`VisualStudioVersion` and `UseVsSdkVersion` default to `17.0`, which is the minimum supported Visual Studio version.

The generated VSIX is written to:

```text
src\Ankh.Package\bin\Release\Ankh.Package.vsix
```

The VSIX is x64 and uses the Visual Studio 2022+ manifest in `Ankh.Package\x64`.

## Tests

Run both active test projects after building:

```bat
dotnet test src\Ankh.Tests\Ankh.Tests.csproj --configuration Release --no-build --no-restore -- RunConfiguration.TreatNoTestsAsError=true

dotnet test src\Ankh.VS.UnitTest\Ankh.VS.UnitTest.csproj --configuration Release --no-build --no-restore -- RunConfiguration.TreatNoTestsAsError=true
```

CI intentionally treats zero discovered tests as a failure.

Legacy tests are retained only when the behavior remains relevant to Visual Studio 2022+. Tests that depended on retired Visual Studio IDE-host, QualityTools, add-in, x86 package, or pre-2022 SDK infrastructure should be redesigned around the current APIs or removed.

## Debug in the Experimental Instance

Set `Ankh.Package` as the startup project and launch Visual Studio with:

```text
/rootSuffix Exp
```

The package uses the modern `AsyncPackage` model and supports background loading.

## Installing a Local Build

Install the generated `.vsix` directly. The old `RegPkg.exe`, VS2005/2008 registry registration, and pre-VS2022 installation paths are no longer supported.

## CI

`.github/workflows/MSBuild.yml` is the reference build. It builds the solution with the VS17 SDK path, runs both NUnit test projects, verifies that `Ankh.UI.dll` is present in the generated VSIX, and publishes VS2022+ artifacts/releases on non-PR builds.
