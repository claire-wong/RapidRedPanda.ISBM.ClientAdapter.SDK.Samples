param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [switch]$AllowDirty
)

$ErrorActionPreference = 'Stop'

function Invoke-Git {
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Arguments
    )

    $output = & git @Arguments 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "git $($Arguments -join ' ') failed: $output"
    }

    return $output
}

function Test-IncludedPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    return (
        $Path -eq 'README.md' -or
        $Path -eq 'LICENSE' -or
        $Path.StartsWith('Documents/') -or
        $Path.StartsWith('CSharp/Windows/') -or
        $Path.StartsWith('CSharp/Raspberry-Pi-OS/')
    )
}

function Get-ProhibitedReason {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    $segments = $Path -split '/'
    foreach ($segment in $segments) {
        $lower = $segment.ToLowerInvariant()
        if ($lower -in @('.git', '.vs', 'bin', 'obj', 'packages', 'validationrunner')) {
            return "prohibited path segment '$segment'"
        }
    }

    $fileName = [System.IO.Path]::GetFileName($Path)
    $lowerName = $fileName.ToLowerInvariant()

    if ($lowerName -eq 'configs.json') {
        return "prohibited local configuration file '$fileName'"
    }

    if ($Path -eq 'CSharp/NuGet.config') {
        return "prohibited local NuGet configuration file"
    }

    if ($lowerName.EndsWith('.nupkg') -or $lowerName.EndsWith('.snupkg')) {
        return "prohibited NuGet package artifact '$fileName'"
    }

    if ($lowerName.EndsWith('.user')) {
        return "prohibited user-specific file '$fileName'"
    }

    if ($Path.StartsWith('artifacts/releases/')) {
        return "prohibited generated release artifact path"
    }

    return $null
}

if ($Version.StartsWith('v')) {
    throw "Pass the SDK version without a leading 'v'. Example: .\scripts\Package-Sdk.ps1 -Version 0.5.0"
}

if ($Version -notmatch '^\d+\.\d+\.\d+(-[0-9A-Za-z][0-9A-Za-z.-]*)?$') {
    throw "Invalid version '$Version'. Use X.Y.Z or X.Y.Z-prerelease, for example 0.5.0 or 0.0.0-test."
}

$repoRoot = (Invoke-Git -Arguments @('rev-parse', '--show-toplevel')).Trim()
Set-Location -LiteralPath $repoRoot

$dirtyStatus = Invoke-Git -Arguments @('status', '--porcelain')
if ($dirtyStatus.Count -gt 0 -and -not $AllowDirty) {
    throw "Working tree is dirty. Commit or stash changes before packaging, or pass -AllowDirty for a non-release validation package."
}

$branch = (Invoke-Git -Arguments @('branch', '--show-current')).Trim()
$commit = (Invoke-Git -Arguments @('rev-parse', 'HEAD')).Trim()
$releaseName = "ISBM-2.0-Client-SDK-v$Version"
$releaseDir = Join-Path $repoRoot 'artifacts/releases'
$zipPath = Join-Path $releaseDir "$releaseName.zip"
$manifestPath = Join-Path $releaseDir "$releaseName-manifest.txt"
$stagingRoot = Join-Path $releaseDir '.staging'
$stagingDir = Join-Path $stagingRoot ([System.Guid]::NewGuid().ToString('N'))
$packageRoot = Join-Path $stagingDir $releaseName

if (Test-Path -LiteralPath $zipPath) {
    throw "Refusing to overwrite existing ZIP: $zipPath"
}

if (Test-Path -LiteralPath $manifestPath) {
    throw "Refusing to overwrite existing manifest: $manifestPath"
}

New-Item -ItemType Directory -Path $packageRoot -Force | Out-Null

try {
    $trackedFiles = Invoke-Git -Arguments @('ls-files')
    $candidateFiles = @($trackedFiles | Where-Object { Test-IncludedPath $_ })
    [array]::Sort($candidateFiles, [System.StringComparer]::Ordinal)

    if ($candidateFiles.Count -eq 0) {
        throw "No package candidates were selected."
    }

    foreach ($path in $candidateFiles) {
        $reason = Get-ProhibitedReason $path
        if ($reason) {
            throw "Refusing to package '$path': $reason."
        }
    }

    $manifestEntries = $candidateFiles | ForEach-Object { "$releaseName/$_" }

    New-Item -ItemType Directory -Path $releaseDir -Force | Out-Null
    $manifestEntries | Set-Content -LiteralPath $manifestPath -Encoding UTF8

    foreach ($path in $candidateFiles) {
        $source = Join-Path $repoRoot $path
        $destination = Join-Path $packageRoot $path
        $destinationDirectory = Split-Path -Parent $destination

        if ($destinationDirectory) {
            New-Item -ItemType Directory -Path $destinationDirectory -Force | Out-Null
        }

        Copy-Item -LiteralPath $source -Destination $destination
    }

    Write-Host "Branch: $branch"
    Write-Host "Commit: $commit"
    Write-Host "SDK package version: $Version"
    Write-Host "ZIP output: $zipPath"
    Write-Host "Manifest output: $manifestPath"
    Write-Host "Package file count: $($candidateFiles.Count)"
    Write-Host "Manifest preview:"
    $manifestEntries | Select-Object -First 20 | ForEach-Object { Write-Host "  $_" }
    if ($manifestEntries.Count -gt 20) {
        Write-Host "  ... $($manifestEntries.Count - 20) more files"
    }

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::CreateFromDirectory(
        $stagingDir,
        $zipPath,
        [System.IO.Compression.CompressionLevel]::Optimal,
        $false
    )
    Write-Host "ZIP created."
}
finally {
    if (Test-Path -LiteralPath $stagingDir) {
        Remove-Item -LiteralPath $stagingDir -Recurse -Force
    }
}
