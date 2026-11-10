[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$OutputDirectory,
    [switch]$Publish,
    # Allows offline validation against the same checksum-pinned artifact.
    [string]$ArtifactArchive
)
$ErrorActionPreference = 'Stop'
$sourceCommit = '826b52eaa7258d63cf01997f090873addd85ab64'
$sourceRun = 41120418118
$artifactId = 13013322160
$artifactSha256 = '06f6643d0dad0401385d6bf0701f1936e9c9fe63f98438a1a95cba49d59e26de'
$version = '1.14005.404-saslfix.1'
$feed = 'https://nuget.pkg.github.com/ARMoir/index.json'
if (Test-Path $OutputDirectory) {
    throw 'Use a new output directory so stale packages cannot be published.'
}
[void](New-Item -ItemType Directory -Path $OutputDirectory)
$OutputDirectory = (Resolve-Path $OutputDirectory).Path
$out = Join-Path $OutputDirectory 'out'
[void](New-Item -ItemType Directory -Path $out)

if (-not $ArtifactArchive) {
    if (-not $env:GITHUB_TOKEN) { throw 'GITHUB_TOKEN is required to download the tested build artifact.' }
    $headers = @{
        Authorization = "Bearer $env:GITHUB_TOKEN"
        Accept = 'application/vnd.github+json'
        'X-GitHub-Api-Version' = '2022-11-28'
    }
    $api = 'https://api.github.com/repos/ARMoir/SharpSvn'
    $run = Invoke-RestMethod "$api/actions/runs/$sourceRun" -Headers $headers
    if ($run.head_sha -ne $sourceCommit -or $run.conclusion -ne 'success') {
        throw 'The pinned source build is not a successful build of the tested commit.'
    }
    $artifact = Invoke-RestMethod "$api/actions/artifacts/$artifactId" -Headers $headers
    if ($artifact.expired -or $artifact.workflow_run.id -ne $sourceRun -or
        $artifact.workflow_run.head_sha -ne $sourceCommit -or $artifact.name -ne 'nupkg-files') {
        throw 'The pinned package artifact is expired or has unexpected provenance.'
    }
    $ArtifactArchive = Join-Path $OutputDirectory 'tested-packages.zip'
    Invoke-WebRequest "$api/actions/artifacts/$artifactId/zip" -Headers $headers -OutFile $ArtifactArchive
}
if ((Get-FileHash $ArtifactArchive -Algorithm SHA256).Hash -ne $artifactSha256) {
    throw 'Tested package archive SHA256 mismatch.'
}
$inputPath = Join-Path $OutputDirectory 'tested'
Expand-Archive -LiteralPath $ArtifactArchive -DestinationPath $inputPath
Add-Type -AssemblyName System.IO.Compression.FileSystem
$provenance = [ordered]@{
    sourceRepository = 'https://github.com/ARMoir/SharpSvn'
    sourceCommit = $sourceCommit
    sourceRun = "https://github.com/ARMoir/SharpSvn/actions/runs/$sourceRun"
    artifactId = $artifactId
    artifactSha256 = $artifactSha256
    packageVersion = $version
    sourcePackageVersion = '1.14005.1'
    packagingRepository = 'https://github.com/ARMoir/AnkhSVN'
    packagingCommit = $env:GITHUB_SHA
    binaryChanges = 'None: original package payloads are preserved byte-for-byte.'
}
$provenanceJson = $provenance | ConvertTo-Json
Set-Content (Join-Path $out 'SOURCE.json') $provenanceJson

function Read-ZipText($Entry) {
    $reader = [IO.StreamReader]::new($Entry.Open())
    try { $reader.ReadToEnd() } finally { $reader.Dispose() }
}
function Write-ZipText($Zip, [string]$Name, [string]$Text) {
    $entry = $Zip.CreateEntry($Name)
    $writer = [IO.StreamWriter]::new($entry.Open(), [Text.UTF8Encoding]::new($false))
    try { $writer.Write($Text) } finally { $writer.Dispose() }
}
function Get-PayloadHashes([string]$Path) {
    $zip = [IO.Compression.ZipFile]::OpenRead($Path)
    $hashes = @{}
    try {
        foreach ($entry in $zip.Entries) {
            if ($entry.FullName -like '*.nuspec' -or $entry.FullName -like '*.psmdcp' -or
                $entry.FullName -eq '[Content_Types].xml' -or $entry.FullName -eq 'docs/SOURCE.json') { continue }
            $stream = $entry.Open()
            $sha = [Security.Cryptography.SHA256]::Create()
            try { $hashes[$entry.FullName] = [Convert]::ToBase64String($sha.ComputeHash($stream)) }
            finally { $sha.Dispose(); $stream.Dispose() }
        }
    } finally { $zip.Dispose() }
    return $hashes
}

foreach ($id in @('SharpSvn', 'SharpSvn.UI')) {
    $original = Join-Path $inputPath "$id.1.14005.1.nupkg"
    if (-not (Test-Path $original)) { throw "Missing tested package: $original" }
    $package = Join-Path $out "$id.$version.nupkg"
    Copy-Item -LiteralPath $original -Destination $package
    $zip = [IO.Compression.ZipFile]::Open($package, [IO.Compression.ZipArchiveMode]::Update)
    try {
        if ($zip.GetEntry('.signature.p7s')) { throw 'Refusing to modify a signed NuGet package.' }
        $entry = $zip.GetEntry("$id.nuspec")
        if (-not $entry) { throw "Missing nuspec for $id" }
        [xml]$spec = Read-ZipText $entry
        if ($spec.package.metadata.id -ne $id -or $spec.package.metadata.version -ne '1.14005.1' -or
            $spec.package.metadata.repository.commit -ne $sourceCommit) {
            throw "Unexpected package identity or source commit in $id"
        }
        $spec.package.metadata.version = $version
        # Associate with the publishing repo so its GITHUB_TOKEN can restore.
        # The original source repository/commit is retained in docs/SOURCE.json.
        $spec.package.metadata.repository.SetAttribute('url', 'https://github.com/ARMoir/AnkhSVN')
        $spec.package.metadata.repository.RemoveAttribute('branch')
        $spec.package.metadata.repository.SetAttribute('commit', [string]$env:GITHUB_SHA)
        $spec.package.metadata.description += " ARMoir SASL prerelease from SharpSvn commit $sourceCommit. See docs/SOURCE.json."
        foreach ($dependency in $spec.SelectNodes("//*[local-name()='dependency' and @id='SharpSvn']")) {
            $dependency.SetAttribute('version', "[$version]")
        }
        $entry.Delete()
        Write-ZipText $zip "$id.nuspec" $spec.OuterXml
        Write-ZipText $zip 'docs/SOURCE.json' $provenanceJson
        # Keep the OPC package metadata consistent with the new version/file.
        $typesEntry = $zip.GetEntry('[Content_Types].xml')
        [xml]$types = Read-ZipText $typesEntry
        if (-not $types.SelectSingleNode("//*[local-name()='Default' and @Extension='json']")) {
            $jsonType = $types.CreateElement('Default', $types.DocumentElement.NamespaceURI)
            $jsonType.SetAttribute('Extension', 'json')
            $jsonType.SetAttribute('ContentType', 'application/json')
            [void]$types.DocumentElement.AppendChild($jsonType)
        }
        $typesEntry.Delete()
        Write-ZipText $zip '[Content_Types].xml' $types.OuterXml
        foreach ($propertiesEntry in @($zip.Entries | Where-Object FullName -Like '*.psmdcp')) {
            $propertiesName = $propertiesEntry.FullName
            [xml]$properties = Read-ZipText $propertiesEntry
            $properties.SelectSingleNode("//*[local-name()='version']").InnerText = $version
            $propertiesEntry.Delete()
            Write-ZipText $zip $propertiesName $properties.OuterXml
        }
    } finally { $zip.Dispose() }
    $before = Get-PayloadHashes $original
    $after = Get-PayloadHashes $package
    if ($before.Count -ne $after.Count) { throw "Package payload entry count changed: $id" }
    foreach ($name in $before.Keys) {
        if ($before[$name] -ne $after[$name]) { throw "Package payload changed: $id/$name" }
    }
    Write-Host "Verified unchanged payload: $id $version"
}
Get-ChildItem $out -Filter '*.nupkg' | ForEach-Object {
    "$((Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant())  $($_.Name)"
} | Set-Content (Join-Path $out 'SHA256SUMS')

if ($Publish) {
    if (-not $env:GITHUB_TOKEN) { throw 'GITHUB_TOKEN is required to publish.' }
    foreach ($id in @('SharpSvn', 'SharpSvn.UI')) {
        dotnet nuget push (Join-Path $out "$id.$version.nupkg") --api-key $env:GITHUB_TOKEN --source $feed
        if ($LASTEXITCODE -ne 0) { throw "Publishing $id failed; existing versions must not be overwritten." }
    }
}
