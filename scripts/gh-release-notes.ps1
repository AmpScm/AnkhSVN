param(
    [Parameter(Mandatory = $true)]
    [string]$Repository,

    [Parameter(Mandatory = $true)]
    [string]$Version,

    [Parameter(Mandatory = $true)]
    [string]$Tag,

    [Parameter(Mandatory = $true)]
    [string]$TargetCommit,

    [Parameter(Mandatory = $true)]
    [string]$Workspace,

    [Parameter(Mandatory = $true)]
    [string]$OutputPath
)

$ErrorActionPreference = "Stop"

function Invoke-GhJson {
    param([Parameter(Mandatory = $true)][string[]]$Arguments)

    $output = & gh @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "gh $($Arguments -join ' ') failed with exit code $LASTEXITCODE."
    }

    $text = ($output -join [Environment]::NewLine)
    if ([string]::IsNullOrWhiteSpace($text)) {
        return $null
    }

    return $text | ConvertFrom-Json
}

function Get-CommitSubject {
    param([string]$Message)

    if ([string]::IsNullOrWhiteSpace($Message)) {
        return "(no commit message)"
    }

    return ($Message -split "\r?\n", 2)[0].Trim()
}

function Get-ChangeCategory {
    param([string]$Subject)

    if ($Subject -match '(?i)\b(fix|fixed|bug|correct|prevent|restore|resolve|regression|crash|clipping|overflow|broken)\b') {
        return "Fixes"
    }

    if ($Subject -match '(?i)\b(ci|build|workflow|coverage|test|tests|testing|package|packaging|release|dependabot|dependency|dependencies|readme|docs|documentation|chore|maintenance)\b') {
        return "Engineering"
    }

    if ($Subject -match '(?i)\b(add|added|introduce|implement|support|enable|improve|improved|polish|modernize|native|copilot|theme|theming|icon|navigation|visual studio|ui)\b') {
        return "Highlights"
    }

    return "Other"
}

function Add-Section {
    param(
        [System.Collections.Generic.List[string]]$Lines,
        [string]$Title,
        [object[]]$Items
    )

    if ($null -eq $Items -or @($Items).Count -eq 0) {
        return
    }

    $Lines.Add("## $Title")
    $Lines.Add("")
    foreach ($item in @($Items)) {
        $Lines.Add([string]$item)
    }
    $Lines.Add("")
}

Write-Host "Preparing release notes for $Tag at $TargetCommit"

$releases = @(Invoke-GhJson -Arguments @(
    "api",
    "repos/$Repository/releases?per_page=30"
))

if ($releases.Count -eq 1 -and $null -ne $releases[0] -and $releases[0].PSObject.Properties.Name -contains "tag_name") {
    $releaseItems = @($releases[0])
}
elseif ($releases.Count -eq 1 -and $releases[0] -is [System.Array]) {
    $releaseItems = @($releases[0])
}
else {
    $releaseItems = @($releases)
}

$previousRelease = $releaseItems |
    Where-Object { -not $_.draft -and $_.tag_name -ne $Tag } |
    Select-Object -First 1

$previousTag = $null
if ($previousRelease) {
    $previousTag = [string]$previousRelease.tag_name
    Write-Host "Previous release: $previousTag"
}
else {
    Write-Host "No previous published release found."
}

$commits = New-Object System.Collections.Generic.List[object]
$files = @()

if ($previousTag) {
    $page = 1
    $totalCommits = $null

    do {
        $comparePath = "repos/{0}/compare/{1}...{2}?per_page=100&page={3}" -f $Repository, $previousTag, $TargetCommit, $page

        $compare = Invoke-GhJson -Arguments @(
            "api",
            $comparePath
        )

        if ($page -eq 1) {
            $files = @($compare.files)
            $totalCommits = [int]$compare.total_commits
        }

        foreach ($commit in @($compare.commits)) {
            $commits.Add($commit)
        }

        $pageCount = @($compare.commits).Count
        $page++
    }
    while ($pageCount -eq 100 -and ($null -eq $totalCommits -or $commits.Count -lt $totalCommits))
}
else {
    $commit = Invoke-GhJson -Arguments @(
        "api",
        "repos/$Repository/commits/$TargetCommit"
    )
    $commits.Add($commit)
    $files = @($commit.files)
}

$entries = New-Object System.Collections.Generic.List[object]
foreach ($commit in $commits) {
    $message = [string]$commit.commit.message
    $subject = Get-CommitSubject $message

    if ($subject -match '^Update README metrics for run #\d+$') {
        continue
    }

    $sha = [string]$commit.sha
    $shortSha = if ($sha.Length -ge 8) { $sha.Substring(0, 8) } else { $sha }
    $url = "https://github.com/$Repository/commit/$sha"

    $entries.Add([pscustomobject]@{
        Subject = $subject
        Message = $message
        Sha = $sha
        ShortSha = $shortSha
        Url = $url
        Category = Get-ChangeCategory $subject
    })
}

$highlights = @($entries | Where-Object Category -eq "Highlights" | ForEach-Object {
    "- $($_.Subject) ([$($_.ShortSha)]($($_.Url)))"
})
$fixes = @($entries | Where-Object Category -eq "Fixes" | ForEach-Object {
    "- $($_.Subject) ([$($_.ShortSha)]($($_.Url)))"
})
$engineering = @($entries | Where-Object Category -eq "Engineering" | ForEach-Object {
    "- $($_.Subject) ([$($_.ShortSha)]($($_.Url)))"
})
$other = @($entries | Where-Object Category -eq "Other" | ForEach-Object {
    "- $($_.Subject) ([$($_.ShortSha)]($($_.Url)))"
})

$issueNumbers = New-Object System.Collections.Generic.HashSet[int]
foreach ($entry in $entries) {
    foreach ($match in [regex]::Matches($entry.Message, '(?<![A-Za-z0-9_])#(?<number>\d+)\b')) {
        [void]$issueNumbers.Add([int]$match.Groups["number"].Value)
    }
}

$resolvedIssues = New-Object System.Collections.Generic.List[string]
foreach ($number in ($issueNumbers | Sort-Object)) {
    $rawIssue = & gh api "repos/$Repository/issues/$number" 2>$null
    if ($LASTEXITCODE -ne 0) {
        continue
    }

    $issue = ($rawIssue -join [Environment]::NewLine) | ConvertFrom-Json
    if ($issue.pull_request -or $issue.state -ne "closed") {
        continue
    }

    $resolvedIssues.Add("- #$number — $($issue.title) ([issue]($($issue.html_url)))")
}

$total = 0
$passed = 0
$failed = 0
$skipped = 0
$trxRoot = Join-Path $Workspace "TestResults"
if (Test-Path $trxRoot) {
    $trxFiles = Get-ChildItem $trxRoot -Recurse -Filter "*.trx" -ErrorAction SilentlyContinue
    foreach ($trxFile in $trxFiles) {
        [xml]$trx = Get-Content -Raw $trxFile.FullName
        $counters = $trx.SelectSingleNode("/*[local-name()='TestRun']/*[local-name()='ResultSummary']/*[local-name()='Counters']")
        if ($null -eq $counters) {
            continue
        }

        $total += [int]$counters.GetAttribute("total")
        $passed += [int]$counters.GetAttribute("passed")
        $failed += [int]$counters.GetAttribute("failed")

        $notExecuted = $counters.GetAttribute("notExecuted")
        if ($notExecuted) {
            $skipped += [int]$notExecuted
        }
    }
}

$lineCoverage = $null
$branchCoverage = $null
$coveragePath = Join-Path $Workspace "CoverageReport\Cobertura.xml"
if (Test-Path $coveragePath) {
    [xml]$coverage = Get-Content -Raw $coveragePath
    $coverageRoot = $coverage.SelectSingleNode("/*[local-name()='coverage']")
    if ($coverageRoot) {
        $lineRate = [double]::Parse(
            $coverageRoot.GetAttribute("line-rate"),
            [Globalization.CultureInfo]::InvariantCulture)
        $branchRate = [double]::Parse(
            $coverageRoot.GetAttribute("branch-rate"),
            [Globalization.CultureInfo]::InvariantCulture)

        $lineCoverage = ($lineRate * 100).ToString("0.0", [Globalization.CultureInfo]::InvariantCulture)
        $branchCoverage = ($branchRate * 100).ToString("0.0", [Globalization.CultureInfo]::InvariantCulture)
    }
}

$generatedArguments = @(
    "api",
    "--method", "POST",
    "repos/$Repository/releases/generate-notes",
    "-f", "tag_name=$Tag",
    "-f", "target_commitish=$TargetCommit",
    "-f", "configuration_file_path=.github/release.yml"
)

if ($previousTag) {
    $generatedArguments += @("-f", "previous_tag_name=$previousTag")
}

$generated = Invoke-GhJson -Arguments $generatedArguments
$generatedBody = [string]$generated.body

# Nest GitHub's generated headings under our Detailed changelog heading.
if (-not [string]::IsNullOrWhiteSpace($generatedBody)) {
    $generatedLines = $generatedBody -split "\r?\n"
    $generatedBody = ($generatedLines | ForEach-Object {
        if ($_ -match '^(#{2,5})(\s+.*)$') {
            return "#" + $Matches[1] + $Matches[2]
        }

        return $_
    }) -join [Environment]::NewLine
}

$lines = New-Object System.Collections.Generic.List[string]
$lines.Add("Changes included in **AnkhSVN $Version**.")
$lines.Add("")

if ($previousTag) {
    $compareUrl = "https://github.com/$Repository/compare/$previousTag...$Tag"
    $lines.Add("This release contains changes since [$previousTag]($compareUrl).")
    $lines.Add("")
}

if ($entries.Count -eq 0) {
    $lines.Add("## Changes")
    $lines.Add("")
    $lines.Add("- No user-facing source changes were detected in the release commit range.")
    $lines.Add("")
}
else {
    Add-Section -Lines $lines -Title "Highlights" -Items $highlights
    Add-Section -Lines $lines -Title "Fixes" -Items $fixes
    Add-Section -Lines $lines -Title "Engineering & reliability" -Items $engineering
    Add-Section -Lines $lines -Title "Other changes" -Items $other
}

Add-Section -Lines $lines -Title "Resolved issues" -Items @($resolvedIssues)

$lines.Add("## Validation")
$lines.Add("")
if ($total -gt 0) {
    $testText = "- Tests: **$passed passed / $total total**"
    if ($failed -gt 0) {
        $testText += " ($failed failed)"
    }
    elseif ($skipped -gt 0) {
        $testText += " ($skipped skipped)"
    }
    $lines.Add($testText)
}
else {
    $lines.Add("- Tests: results were not available to the release-note generator.")
}

if ($lineCoverage -ne $null) {
    $lines.Add("- Line coverage: **$lineCoverage%**")
}
if ($branchCoverage -ne $null) {
    $lines.Add("- Branch coverage: **$branchCoverage%**")
}
if ($env:GITHUB_RUN_ID) {
    $lines.Add("- CI run: [#$env:GITHUB_RUN_NUMBER](https://github.com/$Repository/actions/runs/$env:GITHUB_RUN_ID)")
}
$lines.Add("")

if ($files.Count -gt 0) {
    $lines.Add("## Scope")
    $lines.Add("")

    $areas = New-Object System.Collections.Generic.HashSet[string]
    foreach ($file in $files) {
        $path = [string]$file.filename
        if ($path -like "src/Ankh.Copilot/*") { [void]$areas.Add("Copilot integration") }
        elseif ($path -like "src/Ankh.UI/PendingChanges/*") { [void]$areas.Add("Pending Changes UI") }
        elseif ($path -like "src/Ankh.UI/*") { [void]$areas.Add("Visual Studio UI") }
        elseif ($path -like ".github/*" -or $path -like "scripts/*") { [void]$areas.Add("CI / release automation") }
        elseif ($path -match '(?i)(docs|help|readme)') { [void]$areas.Add("Documentation") }
        elseif ($path -like "src/Ankh.VS.UnitTest/*" -or $path -like "src/Ankh.Tests/*") { [void]$areas.Add("Tests") }
        else { [void]$areas.Add("Core AnkhSVN") }
    }

    $lines.Add("- Areas changed: " + (($areas | Sort-Object) -join ", "))
    $lines.Add("- Files changed: **$($files.Count)**")
    $lines.Add("")
}

$lines.Add("## Detailed changelog")
$lines.Add("")
if ([string]::IsNullOrWhiteSpace($generatedBody)) {
    $lines.Add("GitHub did not return additional generated changelog details for this release.")
}
else {
    foreach ($line in ($generatedBody -split "\r?\n")) {
        $lines.Add($line)
    }
}
$lines.Add("")

$outputDirectory = Split-Path -Parent $OutputPath
if (-not [string]::IsNullOrWhiteSpace($outputDirectory)) {
    New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null
}

Set-Content -Path $OutputPath -Value ($lines -join [Environment]::NewLine) -Encoding utf8
Write-Host "Release notes written to $OutputPath"
