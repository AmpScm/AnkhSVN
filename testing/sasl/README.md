# Local SASL authentication probe

This disposable server distinguishes ordinary svn:// authentication from Cyrus
SASL support in the actual x64 .NET Framework SharpSvn binary bundled by AnkhSVN.
It uses synthetic credentials (`tester` / `local-test-only`) and binds only to
127.0.0.1:13690. Do not use this configuration for a production repository.

Prerequisites: Docker Desktop with Linux containers, and a Release solution build
(see `src/BUILD.md`). Run from the repository root:

```powershell
docker compose -p ankh-sasl-test -f testing/sasl/compose.yaml up -d --build --wait
powershell.exe -NoProfile -File testing/sasl/Test-SharpSvn.ps1
```

The container health check authenticates with the Linux Subversion client against
the SASL repository. A healthy container establishes that the server and test
credentials work independently of SharpSvn. For a visible check:

```powershell
docker compose -p ankh-sasl-test -f testing/sasl/compose.yaml exec -T svn svn info svn://127.0.0.1/sasl --username tester --password local-test-only --non-interactive --no-auth-cache
```

The Windows probe checks out each repository, adds a uniquely named file, and
commits it. Each client uses a temporary configuration and explicit credentials
instead of the user's saved authentication. Temporary working copies are removed
afterward. Exit code 1 means at least one probe failed.

- `control` uses built-in CRAM-MD5 with a repository password file.
- `sasl` offers only DIGEST-MD5, which requires SASL support. Anonymous access and
  the built-in CRAM-MD5 fallback are disabled. Encryption is disabled in this
  localhost-only fixture to isolate authentication from security-layer support.

If control passes and SASL fails with `Cannot negotiate authentication mechanism`
while the container health check succeeds, the tested client cannot negotiate
this server's SASL mechanism. This does not identify the mechanism used by the
reporter of issue #99 or establish support for other SASL mechanisms.

To compare another x64 .NET Framework SharpSvn build, run a fresh PowerShell
process with `-AssemblyPath` pointing to that build's DLL. Keep its native
dependencies beside it. Do not load two versions into the same process.

Remove the disposable repositories and server when finished:

```powershell
docker compose -p ankh-sasl-test -f testing/sasl/compose.yaml down
```

No volumes or host repository directories are mounted; recreating the container
starts with empty repositories. Stop with `down`, then use `up` to rerun.

Configuration follows the Subversion book's
[SASL example](https://svnbook.red-bean.com/en/1.8/svn.serverconfig.svnserve.html#svn.serverconfig.svnserve.sasl).

## Observed result (2026-10-30)

With the bundled x64 .NET Framework SharpSvn 1.14003.272.0:

- Linux SVN authenticated to the SASL repository successfully (container healthy).
- Windows SharpSvn control checkout and commit passed.
- Windows SharpSvn SASL checkout failed with `Cannot negotiate authentication mechanism`.

Repeating the same probe with SharpSvn 1.14005.390.0 after retargeting AnkhSVN
to .NET Framework 4.8 produced the same result: control checkout and commit
passed, but SASL checkout failed with `Cannot negotiate authentication mechanism`.
The Release solution build and all 1,050 unit tests passed. The generated VSIX
contains the new x64 SharpSvn binaries and still targets Visual Studio 2022+.

The Linux client also rejected an incorrect password, confirming authentication
is enforced. This reproduces the error text from issue #99 for DIGEST-MD5;
the reporter's server configuration is still unknown. Upgrading the NuGet
packages alone does not resolve this tested SASL negotiation failure.
