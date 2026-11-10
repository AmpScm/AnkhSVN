# Run with 64-bit Windows PowerShell (powershell.exe), not PowerShell Core.
[CmdletBinding()]
param(
    [string]$AssemblyPath
)
$ErrorActionPreference = 'Stop'
if (-not $AssemblyPath) {
    $AssemblyPath = Join-Path $PSScriptRoot '..\..\src\Ankh.VS.UnitTest\bin\Release\SharpSvn.dll'
}
if ($PSVersionTable.PSEdition -eq 'Core' -or -not [Environment]::Is64BitProcess) {
    throw 'Use 64-bit Windows PowerShell to exercise the .NET Framework x64 binary bundled in AnkhSVN.'
}
$assembly = [Reflection.Assembly]::LoadFrom((Resolve-Path -LiteralPath $AssemblyPath).Path)
Write-Host "Testing $($assembly.FullName)"
$root = Join-Path ([IO.Path]::GetTempPath()) ('AnkhSvn-Sasl-' + [Guid]::NewGuid().ToString('N'))
[IO.Directory]::CreateDirectory($root) | Out-Null
$failures = 0
try {
    foreach ($repository in @('control', 'sasl')) {
        $client = New-Object SharpSvn.SvnClient
        try {
            $config = Join-Path $root ($repository + '-config')
            [IO.Directory]::CreateDirectory($config) | Out-Null
            $client.LoadConfiguration($config)
            $client.Authentication.Clear()
            $client.Authentication.DefaultCredentials = New-Object Net.NetworkCredential('tester', 'local-test-only')
            $uri = [Uri]("svn://127.0.0.1:13690/$repository/")
            $wc = Join-Path $root $repository
            if (-not $client.CheckOut($uri, $wc)) { throw 'Checkout returned false' }
            $file = Join-Path $wc ('probe-' + [Guid]::NewGuid().ToString('N') + '.txt')
            [IO.File]::WriteAllText($file, 'Local authentication probe')
            if (-not $client.Add($file)) { throw 'Add returned false' }
            $commitArgs = New-Object SharpSvn.SvnCommitArgs
            $commitArgs.LogMessage = 'Local authentication probe'
            if (-not $client.Commit($wc, $commitArgs)) { throw 'Commit returned false' }
            Write-Host "PASS $repository : checkout and commit"
        }
        catch {
            $failures++
            Write-Host "FAIL $repository : $($_.Exception.ToString())"
        }
        finally { $client.Dispose() }
    }
}
finally {
    # Only remove the unique temporary directory created by this invocation.
    $tempRoot = [IO.Path]::GetFullPath([IO.Path]::GetTempPath()).TrimEnd('\') + '\'
    $resolvedRoot = [IO.Path]::GetFullPath($root)
    if (-not $resolvedRoot.StartsWith($tempRoot, [StringComparison]::OrdinalIgnoreCase) -or
        [IO.Path]::GetFileName($resolvedRoot) -notlike 'AnkhSvn-Sasl-*') {
        throw "Refusing to remove unexpected test directory: $resolvedRoot"
    }
    Get-ChildItem -LiteralPath $resolvedRoot -Recurse -Force -File | ForEach-Object { $_.IsReadOnly = $false }
    Remove-Item -LiteralPath $resolvedRoot -Recurse -Force
}
if ($failures) { exit 1 }
