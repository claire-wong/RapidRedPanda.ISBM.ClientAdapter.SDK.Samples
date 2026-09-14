param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [switch]$SourceOnly,
    [switch]$SkipHtml,
    [switch]$AllowDirty
)

$ErrorActionPreference = 'Stop'

function Invoke-Checked {
    param(
        [Parameter(Mandatory = $true)]
        [string]$FilePath,

        [Parameter(Mandatory = $true)]
        [string[]]$Arguments,

        [string]$WorkingDirectory
    )

    $previousLocation = Get-Location
    if ($WorkingDirectory) {
        Set-Location -LiteralPath $WorkingDirectory
    }

    try {
        $output = & $FilePath @Arguments 2>&1
        if ($LASTEXITCODE -ne 0) {
            throw "$FilePath $($Arguments -join ' ') failed:`n$($output -join "`n")"
        }

        return $output
    }
    finally {
        if ($WorkingDirectory) {
            Set-Location -LiteralPath $previousLocation
        }
    }
}

function Invoke-Git {
    param([Parameter(Mandatory = $true)][string[]]$Arguments)
    return Invoke-Checked -FilePath 'git' -Arguments $Arguments
}

function Convert-ToPackagePath {
    param([Parameter(Mandatory = $true)][string]$Path)
    return ($Path -replace '\\', '/')
}

function Test-IncludedPath {
    param([Parameter(Mandatory = $true)][string]$Path)
    return (
        $Path -eq 'README.md' -or
        $Path -eq 'LICENSE' -or
        $Path.StartsWith('Documents/') -or
        $Path.StartsWith('CSharp/Windows/') -or
        $Path.StartsWith('CSharp/Raspberry-Pi-OS/')
    )
}

function Get-ProhibitedReason {
    param([Parameter(Mandatory = $true)][string]$Path)

    $normalized = Convert-ToPackagePath $Path
    $segments = $normalized -split '/'
    foreach ($segment in $segments) {
        $lower = $segment.ToLowerInvariant()
        if ($lower -in @('.git', '.vs', 'bin', 'obj', 'packages', 'validationrunner')) {
            return "prohibited path segment '$segment'"
        }
    }

    $fileName = [System.IO.Path]::GetFileName($normalized)
    $lowerName = $fileName.ToLowerInvariant()

    if ($lowerName -eq 'configs.json') {
        return "prohibited local configuration file '$fileName'"
    }
    if ($normalized -eq 'CSharp/NuGet.config') {
        return "prohibited local NuGet configuration file"
    }
    if ($lowerName.EndsWith('.nupkg') -or $lowerName.EndsWith('.snupkg')) {
        return "prohibited NuGet package artifact '$fileName'"
    }
    if ($lowerName.EndsWith('.csproj.user') -or $lowerName.EndsWith('.pubxml.user') -or $lowerName.EndsWith('.user')) {
        return "prohibited user-specific file '$fileName'"
    }
    if ($lowerName.EndsWith('.pdb')) {
        return "prohibited debug symbol file '$fileName'"
    }
    if ($normalized.StartsWith('artifacts/releases/')) {
        return "prohibited generated release artifact path"
    }

    return $null
}

function Get-RelativePath {
    param(
        [Parameter(Mandatory = $true)][string]$BasePath,
        [Parameter(Mandatory = $true)][string]$TargetPath
    )

    $baseUri = [System.Uri]((Resolve-Path -LiteralPath $BasePath).Path.TrimEnd('\') + '\')
    $targetUri = [System.Uri]((Resolve-Path -LiteralPath $TargetPath).Path)
    return [System.Uri]::UnescapeDataString($baseUri.MakeRelativeUri($targetUri).ToString())
}

function Copy-DirectoryContents {
    param(
        [Parameter(Mandatory = $true)][string]$Source,
        [Parameter(Mandatory = $true)][string]$Destination
    )

    New-Item -ItemType Directory -Path $Destination -Force | Out-Null
    Get-ChildItem -LiteralPath $Source -Force | ForEach-Object {
        Copy-Item -LiteralPath $_.FullName -Destination $Destination -Recurse -Force
    }
}

function Copy-RequiredFile {
    param(
        [Parameter(Mandatory = $true)][string]$Source,
        [Parameter(Mandatory = $true)][string]$Destination
    )

    if (-not (Test-Path -LiteralPath $Source)) {
        throw "Required file is missing: $Source"
    }

    $destinationDirectory = Split-Path -Parent $Destination
    if ($destinationDirectory) {
        New-Item -ItemType Directory -Path $destinationDirectory -Force | Out-Null
    }

    Copy-Item -LiteralPath $Source -Destination $Destination -Force
}

function Copy-OptionalFile {
    param(
        [Parameter(Mandatory = $true)][string]$Source,
        [Parameter(Mandatory = $true)][string]$Destination
    )

    if (Test-Path -LiteralPath $Source) {
        Copy-RequiredFile -Source $Source -Destination $Destination
    }
}

function Find-MSBuild {
    $candidates = @()
    $vsWhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (Test-Path -LiteralPath $vsWhere) {
        $result = & $vsWhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' 2>$null
        if ($LASTEXITCODE -eq 0 -and $result) {
            $candidates += $result
        }
    }

    $candidates += @(
        (Join-Path ${env:ProgramFiles} 'Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe'),
        (Join-Path ${env:ProgramFiles} 'Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe'),
        (Join-Path ${env:ProgramFiles} 'Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe'),
        (Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe')
    )

    foreach ($candidate in $candidates | Where-Object { $_ } | Select-Object -Unique) {
        if (Test-Path -LiteralPath $candidate) {
            return $candidate
        }
    }

    $pathCommand = Get-Command 'MSBuild.exe' -ErrorAction SilentlyContinue
    if ($pathCommand) {
        return $pathCommand.Source
    }

    throw 'MSBuild.exe was not found. Install Visual Studio 2022 or Build Tools with MSBuild.'
}

function Convert-DocReference {
    param(
        [Parameter(Mandatory = $true)][string]$Reference,
        [Parameter(Mandatory = $true)][string]$MarkdownDirectory,
        [Parameter(Mandatory = $true)][string]$HtmlDirectory,
        [Parameter(Mandatory = $true)][bool]$ForHtmlLink
    )

    if ($Reference -match '^[a-zA-Z][a-zA-Z0-9+.-]*:' -or $Reference.StartsWith('#')) {
        return $Reference
    }

    $pathPart = $Reference
    $suffix = ''
    $hashIndex = $pathPart.IndexOf('#')
    if ($hashIndex -ge 0) {
        $suffix = $pathPart.Substring($hashIndex)
        $pathPart = $pathPart.Substring(0, $hashIndex)
    }

    $workingPath = $pathPart
    if ($ForHtmlLink -and $workingPath.EndsWith('.md', [System.StringComparison]::OrdinalIgnoreCase)) {
        $workingPath = [System.IO.Path]::ChangeExtension($workingPath, '.html')
    }

    $targetPath = if ([System.IO.Path]::IsPathRooted($workingPath)) {
        Join-Path $script:PackageRootForHtml $workingPath.TrimStart('\', '/')
    }
    else {
        Join-Path $MarkdownDirectory $workingPath
    }

    $targetDirectory = Split-Path -Parent $targetPath
    if (-not (Test-Path -LiteralPath $targetDirectory)) {
        return (Convert-ToPackagePath $workingPath) + $suffix
    }

    $relative = Get-RelativePath -BasePath $HtmlDirectory -TargetPath $targetPath
    return (Convert-ToPackagePath $relative) + $suffix
}

function Convert-InlineMarkdown {
    param(
        [Parameter(Mandatory = $true)][string]$Text,
        [Parameter(Mandatory = $true)][string]$MarkdownDirectory,
        [Parameter(Mandatory = $true)][string]$HtmlDirectory
    )

    $encoded = [System.Net.WebUtility]::HtmlEncode($Text)
    $encoded = [regex]::Replace($encoded, '!\[([^\]]*)\]\(([^)]+)\)', {
        param($match)
        $alt = $match.Groups[1].Value
        $target = [System.Net.WebUtility]::HtmlDecode($match.Groups[2].Value)
        $src = Convert-DocReference -Reference $target -MarkdownDirectory $MarkdownDirectory -HtmlDirectory $HtmlDirectory -ForHtmlLink:$false
        return "<img src=""$([System.Net.WebUtility]::HtmlEncode($src))"" alt=""$alt"">"
    })
    $encoded = [regex]::Replace($encoded, '\[([^\]]+)\]\(([^)]+)\)', {
        param($match)
        $text = $match.Groups[1].Value
        $target = [System.Net.WebUtility]::HtmlDecode($match.Groups[2].Value)
        $href = Convert-DocReference -Reference $target -MarkdownDirectory $MarkdownDirectory -HtmlDirectory $HtmlDirectory -ForHtmlLink:$true
        return "<a href=""$([System.Net.WebUtility]::HtmlEncode($href))"">$text</a>"
    })
    $encoded = [regex]::Replace($encoded, '`([^`]+)`', '<code>$1</code>')
    $encoded = [regex]::Replace($encoded, '\*\*([^*]+)\*\*', '<strong>$1</strong>')
    return $encoded
}

function Get-HeadingId {
    param([string]$Text)
    $plain = [regex]::Replace($Text.ToLowerInvariant(), '[^a-z0-9\s-]', '')
    $plain = [regex]::Replace($plain.Trim(), '\s+', '-')
    return $plain
}

function Convert-MarkdownToHtml {
    param(
        [Parameter(Mandatory = $true)][string]$MarkdownPath,
        [Parameter(Mandatory = $true)][string]$HtmlPath,
        [switch]$AddNavigation
    )

    $markdownDirectory = Split-Path -Parent $MarkdownPath
    $htmlDirectory = Split-Path -Parent $HtmlPath
    New-Item -ItemType Directory -Path $htmlDirectory -Force | Out-Null

    $lines = Get-Content -LiteralPath $MarkdownPath
    $body = New-Object System.Collections.Generic.List[string]
    $inCode = $false
    $inList = $false

    foreach ($line in $lines) {
        if ($line -match '^\s*```') {
            if ($inList) { $body.Add('</ul>'); $inList = $false }
            if ($inCode) { $body.Add('</code></pre>'); $inCode = $false }
            else { $body.Add('<pre><code>'); $inCode = $true }
            continue
        }

        if ($inCode) {
            $body.Add([System.Net.WebUtility]::HtmlEncode($line))
            continue
        }

        if ($line.Trim().Length -eq 0) {
            if ($inList) { $body.Add('</ul>'); $inList = $false }
            continue
        }

        if ($line -match '^(#{1,6})\s+(.+)$') {
            if ($inList) { $body.Add('</ul>'); $inList = $false }
            $level = $matches[1].Length
            $headingText = $matches[2].Trim()
            $content = Convert-InlineMarkdown -Text $headingText -MarkdownDirectory $markdownDirectory -HtmlDirectory $htmlDirectory
            $body.Add("<h$level id=""$(Get-HeadingId $headingText)"">$content</h$level>")
            continue
        }

        if ($line -match '^\s*(?:[-*]|\d+\.)\s+(.+)$') {
            if (-not $inList) { $body.Add('<ul>'); $inList = $true }
            $content = Convert-InlineMarkdown -Text $matches[1].Trim() -MarkdownDirectory $markdownDirectory -HtmlDirectory $htmlDirectory
            $body.Add("<li>$content</li>")
            continue
        }

        if ($inList) { $body.Add('</ul>'); $inList = $false }
        $paragraph = Convert-InlineMarkdown -Text $line.Trim() -MarkdownDirectory $markdownDirectory -HtmlDirectory $htmlDirectory
        $body.Add("<p>$paragraph</p>")
    }

    if ($inList) { $body.Add('</ul>') }
    if ($inCode) { $body.Add('</code></pre>') }

    $title = [System.Net.WebUtility]::HtmlEncode([System.IO.Path]::GetFileNameWithoutExtension($MarkdownPath))
    $navigation = ''
    if ($AddNavigation) {
        $navigation = @'
<nav class="package-nav">
  <strong>Package navigation</strong>
  <a href="README.md">README.md</a>
  <a href="Documents/Use_Cases/Smart-Agriculture-Monitoring-System.html">Smart Agriculture</a>
  <a href="Documents/Use_Cases/Fleet_Management.html">Fleet Management</a>
  <a href="Documents/Use_Cases/Flood-Management.html">Flood Management</a>
  <a href="CSharp/Windows/">Windows samples</a>
  <a href="CSharp/Raspberry-Pi-OS/">Raspberry Pi OS samples</a>
  <a href="Sample%20Runtime/">Sample Runtime</a>
  <a href="Self-contained%20Deployment/">Self-contained Deployment</a>
</nav>
'@
    }

    $html = @"
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>$title</title>
  <style>
    body { color: #1f2933; font-family: "Segoe UI", Arial, sans-serif; line-height: 1.55; margin: 0 auto; max-width: 980px; padding: 32px 24px; }
    img { border: 1px solid #d8dee4; box-sizing: border-box; height: auto; max-width: 100%; }
    code { background: #f3f4f6; border-radius: 4px; padding: 0.1em 0.25em; }
    pre { background: #f3f4f6; border-radius: 6px; overflow-x: auto; padding: 16px; }
    pre code { background: transparent; padding: 0; }
    a { color: #0b63ce; }
    h1, h2, h3 { line-height: 1.2; }
    .package-nav { background: #f6f8fa; border: 1px solid #d8dee4; border-radius: 6px; display: flex; flex-wrap: wrap; gap: 10px 16px; margin-bottom: 28px; padding: 12px 14px; }
    .package-nav strong { flex-basis: 100%; }
  </style>
</head>
<body>
$navigation
$($body -join "`n")
</body>
</html>
"@
    Set-Content -LiteralPath $HtmlPath -Value $html -Encoding UTF8
}

function Export-HeadContent {
    param([Parameter(Mandatory = $true)][string]$Destination)

    $archivePath = Join-Path ([System.IO.Path]::GetTempPath()) "isbm-sdk-source-$([System.Guid]::NewGuid().ToString('N')).zip"
    try {
        Invoke-Git -Arguments @(
            'archive',
            '--format=zip',
            "--output=$archivePath",
            'HEAD',
            '--',
            'README.md',
            'LICENSE',
            'Documents',
            'CSharp/Windows',
            'CSharp/Raspberry-Pi-OS'
        ) | Out-Null
        Expand-Archive -LiteralPath $archivePath -DestinationPath $Destination
    }
    finally {
        if (Test-Path -LiteralPath $archivePath) {
            Remove-Item -LiteralPath $archivePath -Force
        }
    }
}

function Copy-PublishDirectory {
    param(
        [Parameter(Mandatory = $true)][string]$Source,
        [Parameter(Mandatory = $true)][string]$Destination
    )

    New-Item -ItemType Directory -Path $Destination -Force | Out-Null
    Get-ChildItem -LiteralPath $Source -File -Recurse -Force | ForEach-Object {
        if ($_.Name -eq 'Configs.json' -or $_.Extension -eq '.pdb') {
            return
        }
        $relative = Get-RelativePath -BasePath $Source -TargetPath $_.FullName
        Copy-RequiredFile -Source $_.FullName -Destination (Join-Path $Destination $relative)
    }
}

function Stage-WindowsRuntime {
    param(
        [Parameter(Mandatory = $true)][string]$BuildRoot,
        [Parameter(Mandatory = $true)][string]$PackageRoot,
        [Parameter(Mandatory = $true)][string]$MsBuildPath
    )

    $projects = @(
        @{ Name = 'ISBM20ChannelManagementTestCSharp'; Path = 'CSharp\Windows\ISBM20ChannelManagementTestCSharp\ISBM20ChannelManagementTestCSharp.csproj' },
        @{ Name = 'ISBM20ConsumerPublicationTestCSharp'; Path = 'CSharp\Windows\ISBM20ConsumerPublicationTestCSharp\ISBM20ConsumerPublicationTestCSharp.csproj' },
        @{ Name = 'ISBM20ProviderPublicationTestCSharp'; Path = 'CSharp\Windows\ISBM20ProviderPublicationTestCSharp\ISBM20ProviderPublicationTestCSharp.csproj' },
        @{ Name = 'ISBM20ConsumerRequestTestCSharp'; Path = 'CSharp\Windows\ISBM20ConsumerRequestTestCSharp\ISBM20ConsumerRequestTestCSharp.csproj' },
        @{ Name = 'ISBM20ProviderRequestTestCSharp'; Path = 'CSharp\Windows\ISBM20ProviderRequestTestCSharp\ISBM20ProviderRequestTestCSharp.csproj' }
    )

    foreach ($project in $projects) {
        $projectPath = Join-Path $BuildRoot $project.Path
        $outputDir = Join-Path $BuildRoot "runtime-build\windows\$($project.Name)"
        New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
        Invoke-Checked -FilePath $MsBuildPath -Arguments @(
            $projectPath,
            '/restore',
            '/p:Configuration=Release',
            '/p:Platform=AnyCPU',
            "/p:OutDir=$outputDir\",
            '/v:minimal'
        ) | Out-Null

        $destination = Join-Path $PackageRoot "Sample Runtime\Windows\$($project.Name)"
        Copy-RequiredFile -Source (Join-Path $outputDir "$($project.Name).exe") -Destination (Join-Path $destination "$($project.Name).exe")
        Copy-OptionalFile -Source (Join-Path $outputDir "$($project.Name).exe.config") -Destination (Join-Path $destination "$($project.Name).exe.config")
        Copy-RequiredFile -Source (Join-Path $outputDir 'RapidRedPanda.ISBM.ClientAdapter.dll') -Destination (Join-Path $destination 'RapidRedPanda.ISBM.ClientAdapter.dll')
        Copy-RequiredFile -Source (Join-Path $outputDir 'Newtonsoft.Json.dll') -Destination (Join-Path $destination 'Newtonsoft.Json.dll')

        $bodSource = Join-Path $outputDir 'BODs'
        if (Test-Path -LiteralPath $bodSource) {
            Copy-DirectoryContents -Source $bodSource -Destination (Join-Path $destination 'BODs')
        }
    }
}

function Assert-PiFrameworkRuntime {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$ProjectName,
        [Parameter(Mandatory = $true)][string]$Payload
    )

    foreach ($file in @("$ProjectName.dll", "$ProjectName.deps.json", "$ProjectName.runtimeconfig.json", 'RapidRedPanda.ISBM.ClientAdapter.dll', 'Newtonsoft.Json.dll', 'Configs-Example.json', $Payload)) {
        if (-not (Test-Path -LiteralPath (Join-Path $Path $file))) {
            throw "Raspberry Pi framework-dependent runtime is missing $file for $ProjectName."
        }
    }
    if (Test-Path -LiteralPath (Join-Path $Path $ProjectName)) {
        throw "Raspberry Pi framework-dependent runtime should not include an apphost for $ProjectName."
    }
}

function Assert-PiSelfContainedRuntime {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$ProjectName,
        [Parameter(Mandatory = $true)][string]$Payload
    )

    foreach ($file in @($ProjectName, "$ProjectName.dll", "$ProjectName.deps.json", "$ProjectName.runtimeconfig.json", 'RapidRedPanda.ISBM.ClientAdapter.dll', 'Newtonsoft.Json.dll', 'Configs-Example.json', $Payload, 'libcoreclr.so', 'libhostfxr.so', 'libhostpolicy.so')) {
        if (-not (Test-Path -LiteralPath (Join-Path $Path $file))) {
            throw "Raspberry Pi self-contained runtime is missing $file for $ProjectName."
        }
    }
}

function Stage-PiRuntime {
    param(
        [Parameter(Mandatory = $true)][string]$BuildRoot,
        [Parameter(Mandatory = $true)][string]$PackageRoot
    )

    $projects = @(
        @{ Name = 'ISBM20Pi3PublicationTestNet8'; Path = 'CSharp\Raspberry-Pi-OS\ISBM20Pi3PublicationTestNet8\ISBM20Pi3PublicationTestNet8.csproj'; Payload = 'SyncMeasurements.json' },
        @{ Name = 'ISBM20Pi3RequestTestNet8'; Path = 'CSharp\Raspberry-Pi-OS\ISBM20Pi3RequestTestNet8\ISBM20Pi3RequestTestNet8.csproj'; Payload = 'ShowMeasurements.json' }
    )

    foreach ($project in $projects) {
        $projectPath = Join-Path $BuildRoot $project.Path
        $projectDirectory = Split-Path -Parent $projectPath

        $frameworkOutput = Join-Path $BuildRoot "runtime-build\pi-framework\$($project.Name)"
        Invoke-Checked -FilePath 'dotnet' -Arguments @(
            'publish', $projectPath, '-c', 'Release',
            '--self-contained', 'false',
            '-p:UseAppHost=false',
            '-o', $frameworkOutput,
            '-v', 'minimal'
        ) | Out-Null

        $frameworkDestination = Join-Path $PackageRoot "Sample Runtime\Raspberry-Pi-OS\$($project.Name)"
        Copy-PublishDirectory -Source $frameworkOutput -Destination $frameworkDestination
        Copy-RequiredFile -Source (Join-Path $projectDirectory 'Configs-Example.json') -Destination (Join-Path $frameworkDestination 'Configs-Example.json')
        Copy-RequiredFile -Source (Join-Path $projectDirectory $project.Payload) -Destination (Join-Path $frameworkDestination $project.Payload)
        Assert-PiFrameworkRuntime -Path $frameworkDestination -ProjectName $project.Name -Payload $project.Payload

        $selfContainedOutput = Join-Path $BuildRoot "runtime-build\pi-self-contained\$($project.Name)"
        Invoke-Checked -FilePath 'dotnet' -Arguments @(
            'publish', $projectPath, '-c', 'Release',
            '-r', 'linux-arm',
            '--self-contained', 'true',
            '-p:PublishSingleFile=false',
            '-p:PublishTrimmed=false',
            '-o', $selfContainedOutput,
            '-v', 'minimal'
        ) | Out-Null

        $selfContainedDestination = Join-Path $PackageRoot "Self-contained Deployment\Raspberry-Pi-OS\$($project.Name)"
        Copy-PublishDirectory -Source $selfContainedOutput -Destination $selfContainedDestination
        Copy-RequiredFile -Source (Join-Path $projectDirectory 'Configs-Example.json') -Destination (Join-Path $selfContainedDestination 'Configs-Example.json')
        Copy-RequiredFile -Source (Join-Path $projectDirectory $project.Payload) -Destination (Join-Path $selfContainedDestination $project.Payload)
        Assert-PiSelfContainedRuntime -Path $selfContainedDestination -ProjectName $project.Name -Payload $project.Payload
    }
}

function Assert-WindowsRuntime {
    param([Parameter(Mandatory = $true)][string]$PackageRoot)

    $projects = @(
        @{ Name = 'ISBM20ChannelManagementTestCSharp'; Payload = $null },
        @{ Name = 'ISBM20ConsumerPublicationTestCSharp'; Payload = $null },
        @{ Name = 'ISBM20ProviderPublicationTestCSharp'; Payload = 'BODs\SyncMeasurements.json' },
        @{ Name = 'ISBM20ConsumerRequestTestCSharp'; Payload = 'BODs\GetMeasurements.json' },
        @{ Name = 'ISBM20ProviderRequestTestCSharp'; Payload = 'BODs\ShowMeasurements.json' }
    )

    foreach ($project in $projects) {
        $path = Join-Path $PackageRoot "Sample Runtime\Windows\$($project.Name)"
        foreach ($file in @("$($project.Name).exe", "$($project.Name).exe.config", 'RapidRedPanda.ISBM.ClientAdapter.dll', 'Newtonsoft.Json.dll')) {
            if (-not (Test-Path -LiteralPath (Join-Path $path $file))) {
                throw "Windows runtime is missing $file for $($project.Name)."
            }
        }
        if ($project.Payload -and -not (Test-Path -LiteralPath (Join-Path $path $project.Payload))) {
            throw "Windows runtime is missing payload $($project.Payload) for $($project.Name)."
        }
    }
}

function Assert-NoProhibitedContent {
    param([Parameter(Mandatory = $true)][string]$Root)

    Get-ChildItem -LiteralPath $Root -File -Recurse -Force | ForEach-Object {
        $relative = Get-RelativePath -BasePath $Root -TargetPath $_.FullName
        $reason = Get-ProhibitedReason $relative
        if ($reason) {
            throw "Refusing to package '$relative': $reason."
        }
    }
}

function Assert-HtmlReferences {
    param([Parameter(Mandatory = $true)][string]$PackageRoot)

    Get-ChildItem -LiteralPath $PackageRoot -Filter '*.html' -File -Recurse -Force | ForEach-Object {
        $htmlPath = $_.FullName
        $htmlDirectory = Split-Path -Parent $htmlPath
        $content = Get-Content -Raw -LiteralPath $htmlPath
        foreach ($match in [regex]::Matches($content, '(?:href|src)="([^"]+)"')) {
            $reference = [System.Net.WebUtility]::HtmlDecode($match.Groups[1].Value)
            if ($reference -match '^[a-zA-Z][a-zA-Z0-9+.-]*:' -or $reference.StartsWith('#')) {
                continue
            }
            $pathPart = ($reference -split '#')[0]
            if ($pathPart.Length -eq 0) {
                continue
            }
            if (-not (Test-Path -LiteralPath (Join-Path $htmlDirectory $pathPart))) {
                throw "Generated HTML reference does not resolve from '$htmlPath': $reference"
            }
        }
    }
}

function Assert-RequiredPackageContent {
    param(
        [Parameter(Mandatory = $true)][string]$PackageRoot,
        [switch]$RuntimeExpected,
        [switch]$HtmlExpected
    )

    foreach ($path in @('README.md', 'LICENSE', 'Documents', 'CSharp\Windows', 'CSharp\Raspberry-Pi-OS')) {
        if (-not (Test-Path -LiteralPath (Join-Path $PackageRoot $path))) {
            throw "Required package content is missing: $path"
        }
    }

    foreach ($path in @(
        'CSharp\Windows\ISBM20ChannelManagementTestCSharp\ISBM20ChannelManagementTestCSharp.csproj',
        'CSharp\Windows\ISBM20ConsumerPublicationTestCSharp\ISBM20ConsumerPublicationTestCSharp.csproj',
        'CSharp\Windows\ISBM20ProviderPublicationTestCSharp\ISBM20ProviderPublicationTestCSharp.csproj',
        'CSharp\Windows\ISBM20ConsumerRequestTestCSharp\ISBM20ConsumerRequestTestCSharp.csproj',
        'CSharp\Windows\ISBM20ProviderRequestTestCSharp\ISBM20ProviderRequestTestCSharp.csproj',
        'CSharp\Raspberry-Pi-OS\ISBM20Pi3PublicationTestNet8\ISBM20Pi3PublicationTestNet8.csproj',
        'CSharp\Raspberry-Pi-OS\ISBM20Pi3RequestTestNet8\ISBM20Pi3RequestTestNet8.csproj'
    )) {
        if (-not (Test-Path -LiteralPath (Join-Path $PackageRoot $path))) {
            throw "Required source project is missing: $path"
        }
    }

    if ($HtmlExpected) {
        foreach ($path in @(
            'README.html',
            'Documents\Use_Cases\Smart-Agriculture-Monitoring-System.html',
            'Documents\Use_Cases\Fleet_Management.html',
            'Documents\Use_Cases\Flood-Management.html'
        )) {
            if (-not (Test-Path -LiteralPath (Join-Path $PackageRoot $path))) {
                throw "Required generated HTML is missing: $path"
            }
        }
        Assert-HtmlReferences -PackageRoot $PackageRoot
    }

    if ($RuntimeExpected) {
        Assert-WindowsRuntime -PackageRoot $PackageRoot
        foreach ($project in @(
            @{ Name = 'ISBM20Pi3PublicationTestNet8'; Payload = 'SyncMeasurements.json' },
            @{ Name = 'ISBM20Pi3RequestTestNet8'; Payload = 'ShowMeasurements.json' }
        )) {
            Assert-PiFrameworkRuntime -Path (Join-Path $PackageRoot "Sample Runtime\Raspberry-Pi-OS\$($project.Name)") -ProjectName $project.Name -Payload $project.Payload
            Assert-PiSelfContainedRuntime -Path (Join-Path $PackageRoot "Self-contained Deployment\Raspberry-Pi-OS\$($project.Name)") -ProjectName $project.Name -Payload $project.Payload
        }
    }
}

function New-ManifestEntries {
    param([Parameter(Mandatory = $true)][string]$StagingDir)

    return @(Get-ChildItem -LiteralPath $StagingDir -File -Recurse -Force |
        ForEach-Object { Convert-ToPackagePath (Get-RelativePath -BasePath $StagingDir -TargetPath $_.FullName) } |
        Sort-Object)
}

function Assert-ZipMatchesManifest {
    param(
        [Parameter(Mandatory = $true)][string]$ZipPath,
        [Parameter(Mandatory = $true)][string]$ManifestPath
    )

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $zip = [System.IO.Compression.ZipFile]::OpenRead($ZipPath)
    try {
        $zipEntries = @($zip.Entries | Where-Object { $_.FullName -notlike '*/' } | ForEach-Object { $_.FullName.TrimEnd('/') } | Sort-Object)
    }
    finally {
        $zip.Dispose()
    }

    $manifestEntries = @(Get-Content -LiteralPath $ManifestPath | Sort-Object)
    $diff = Compare-Object -ReferenceObject $manifestEntries -DifferenceObject $zipEntries
    if ($diff) {
        throw "ZIP entries do not match manifest."
    }
}

function Test-ExtractedPackage {
    param(
        [Parameter(Mandatory = $true)][string]$ZipPath,
        [Parameter(Mandatory = $true)][string]$ReleaseName,
        [switch]$RuntimeExpected,
        [switch]$HtmlExpected
    )

    $extractRoot = Join-Path ([System.IO.Path]::GetTempPath()) "isbm-sdk-validate-$([System.Guid]::NewGuid().ToString('N').Substring(0,8))"
    $cacheRoot = Join-Path ([System.IO.Path]::GetTempPath()) "isbm-sdk-nuget-$([System.Guid]::NewGuid().ToString('N'))"
    New-Item -ItemType Directory -Path $cacheRoot | Out-Null
    Expand-Archive -LiteralPath $ZipPath -DestinationPath $extractRoot

    $packageRoot = Join-Path $extractRoot $ReleaseName
    Assert-RequiredPackageContent -PackageRoot $packageRoot -RuntimeExpected:$RuntimeExpected -HtmlExpected:$HtmlExpected
    Assert-NoProhibitedContent -Root $packageRoot

    $oldNuGetPackages = $env:NUGET_PACKAGES
    $env:NUGET_PACKAGES = $cacheRoot
    try {
        foreach ($relativeProject in @(
            'CSharp\Windows\ISBM20ChannelManagementTestCSharp\ISBM20ChannelManagementTestCSharp.csproj',
            'CSharp\Windows\ISBM20ConsumerPublicationTestCSharp\ISBM20ConsumerPublicationTestCSharp.csproj',
            'CSharp\Windows\ISBM20ProviderPublicationTestCSharp\ISBM20ProviderPublicationTestCSharp.csproj',
            'CSharp\Windows\ISBM20ConsumerRequestTestCSharp\ISBM20ConsumerRequestTestCSharp.csproj',
            'CSharp\Windows\ISBM20ProviderRequestTestCSharp\ISBM20ProviderRequestTestCSharp.csproj'
        )) {
            $projectPath = Join-Path $packageRoot $relativeProject
            $restoreOutput = Invoke-Checked -FilePath 'dotnet' -Arguments @('restore', $projectPath, '-v', 'minimal')
            $buildOutput = Invoke-Checked -FilePath 'dotnet' -Arguments @('build', $projectPath, '-c', 'Release', '--no-restore', '-v', 'minimal')
            $combined = (@($restoreOutput) + @($buildOutput)) -join "`n"
            if ($combined -match 'MSB3245') {
                throw "Extracted source build produced MSB3245 for $relativeProject."
            }
            if ($combined -match 'CS0246' -and $combined -match 'RapidRedPanda|Newtonsoft') {
                throw "Extracted source build produced missing ClientAdapter/Newtonsoft errors for $relativeProject."
            }
        }
    }
    finally {
        if ($null -eq $oldNuGetPackages) {
            Remove-Item Env:\NUGET_PACKAGES -ErrorAction SilentlyContinue
        }
        else {
            $env:NUGET_PACKAGES = $oldNuGetPackages
        }
    }
}

if ($Version.StartsWith('v')) {
    throw "Pass the SDK version without a leading 'v'. Example: .\scripts\Package-Sdk.ps1 -Version 0.5.0"
}
if ($Version -notmatch '^\d+\.\d+\.\d+(-[0-9A-Za-z][0-9A-Za-z.-]*)?$') {
    throw "Invalid version '$Version'. Use X.Y.Z or X.Y.Z-prerelease, for example 0.5.0 or 0.0.0-test."
}

$repoRoot = (Invoke-Git -Arguments @('rev-parse', '--show-toplevel')).Trim()
Set-Location -LiteralPath $repoRoot

$dirtyStatus = @(Invoke-Git -Arguments @('status', '--porcelain'))
if ($dirtyStatus.Count -gt 0 -and -not $AllowDirty) {
    throw "Working tree is dirty. Commit or stash changes before packaging, or pass -AllowDirty for a non-release validation package."
}

$branch = (Invoke-Git -Arguments @('branch', '--show-current')).Trim()
$commit = (Invoke-Git -Arguments @('rev-parse', 'HEAD')).Trim()
$releaseName = "ISBM-2.0-Client-SDK-v$Version"
$releaseDir = Join-Path $repoRoot 'artifacts/releases'
$zipPath = Join-Path $releaseDir "$releaseName.zip"
$manifestPath = Join-Path $releaseDir "$releaseName-manifest.txt"
$stagingDir = Join-Path ([System.IO.Path]::GetTempPath()) "isbm-sdk-package-$([System.Guid]::NewGuid().ToString('N'))"
$sourceRoot = Join-Path $stagingDir 'source'
$buildRoot = Join-Path $stagingDir 'build'
$packageStaging = Join-Path $stagingDir 'package'
$packageRoot = Join-Path $packageStaging $releaseName
$script:PackageRootForHtml = $packageRoot

if (Test-Path -LiteralPath $zipPath) {
    throw "Refusing to overwrite existing ZIP: $zipPath"
}
if (Test-Path -LiteralPath $manifestPath) {
    throw "Refusing to overwrite existing manifest: $manifestPath"
}

New-Item -ItemType Directory -Path $sourceRoot, $buildRoot, $packageRoot, $releaseDir -Force | Out-Null

try {
    Export-HeadContent -Destination $sourceRoot
    Copy-DirectoryContents -Source $sourceRoot -Destination $packageRoot
    Copy-DirectoryContents -Source $sourceRoot -Destination $buildRoot

    Assert-NoProhibitedContent -Root $packageRoot

    if (-not $SkipHtml) {
        Convert-MarkdownToHtml -MarkdownPath (Join-Path $packageRoot 'README.md') -HtmlPath (Join-Path $packageRoot 'README.html') -AddNavigation
        foreach ($relativeMarkdown in @(
            'Documents\Use_Cases\Smart-Agriculture-Monitoring-System.md',
            'Documents\Use_Cases\Fleet_Management.md',
            'Documents\Use_Cases\Flood-Management.md'
        )) {
            $markdownPath = Join-Path $packageRoot $relativeMarkdown
            Convert-MarkdownToHtml -MarkdownPath $markdownPath -HtmlPath ([System.IO.Path]::ChangeExtension($markdownPath, '.html'))
        }
    }

    if (-not $SourceOnly) {
        Stage-WindowsRuntime -BuildRoot $buildRoot -PackageRoot $packageRoot -MsBuildPath (Find-MSBuild)
        Stage-PiRuntime -BuildRoot $buildRoot -PackageRoot $packageRoot
    }

    Assert-NoProhibitedContent -Root $packageRoot
    Assert-RequiredPackageContent -PackageRoot $packageRoot -RuntimeExpected:(-not $SourceOnly) -HtmlExpected:(-not $SkipHtml)

    $manifestEntries = New-ManifestEntries -StagingDir $packageStaging
    $manifestEntries | Set-Content -LiteralPath $manifestPath -Encoding UTF8

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::CreateFromDirectory($packageStaging, $zipPath, [System.IO.Compression.CompressionLevel]::Optimal, $false)

    Assert-ZipMatchesManifest -ZipPath $zipPath -ManifestPath $manifestPath
    Test-ExtractedPackage -ZipPath $zipPath -ReleaseName $releaseName -RuntimeExpected:(-not $SourceOnly) -HtmlExpected:(-not $SkipHtml)

    Write-Host "Branch: $branch"
    Write-Host "Commit: $commit"
    Write-Host "SDK package version: $Version"
    Write-Host "ZIP output: $zipPath"
    Write-Host "Manifest output: $manifestPath"
    Write-Host "Package file count: $($manifestEntries.Count)"
    Write-Host "Source-only: $($SourceOnly.IsPresent)"
    Write-Host "HTML generated: $(-not $SkipHtml)"
    Write-Host "Runtime generated: $(-not $SourceOnly)"
    Write-Host "ZIP validated."
}
finally {
    if (Test-Path -LiteralPath $stagingDir) {
        Remove-Item -LiteralPath $stagingDir -Recurse -Force
    }
}
