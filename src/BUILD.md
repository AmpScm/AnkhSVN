# Building AnkhSVN

AnkhSVN now targets **Visual Studio 2022 and later**. The pre-VS2022 package, x86 VSIX, Visual Studio IDE-host test harnesses, and older SDK-specific build paths have been removed.

## Requirements

- Visual Studio 2022 or later.
- The **Visual Studio extension development** workload.
- .NET Framework 4.7.2 targeting support.
- Git.
- A Subversion command-line client is recommended for development and is installed by CI.

The solution restores the Visual Studio SDK, VSSDK Build Tools, SharpSvn, and test dependencies through NuGet.

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
