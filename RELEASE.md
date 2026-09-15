# Release Packaging

This repository publishes the ISBM 2.0 Client SDK samples as a custom SDK ZIP attached to each GitHub Release. GitHub also provides automatic source archives for every tag.

The custom ZIP is generated from approved tracked repository content plus intentionally generated documentation and runtime outputs. Do not create release ZIPs manually from Windows Explorer or from a working directory copy.

Generated release outputs live only under:

```text
artifacts/releases/
```

That directory is generated/local output and is intentionally ignored by Git.

## Versioning

SDK release version != RapidRedPanda.ISBM.ClientAdapter package version.

The SDK ZIP name follows the release tag:

```text
ISBM-2.0-Client-SDK-vX.Y.Z.zip
```

For example:

```text
ISBM-2.0-Client-SDK-v0.5.0.zip
```

The packaging script accepts `X.Y.Z` and `X.Y.Z-prerelease` values. Pass the version without the leading `v`.

## Release Flow

1. Choose the SDK release version.
2. Verify the intended `RapidRedPanda.ISBM.ClientAdapter` dependency version in the sample projects.
3. Validate staging and confirm the release branch contains the intended commits.
4. Merge or release from the intended release branch.
5. Require a clean working tree.
6. Run the packaging script:

   ```powershell
   .\scripts\Package-Sdk.ps1 -Version 0.5.0
   ```

   For validation packages, use a prerelease version and `-AllowDirty` only when local unrelated changes are intentionally excluded from the package because the script stages committed `HEAD` content:

   ```powershell
   .\scripts\Package-Sdk.ps1 -Version 0.5.0-review3 -AllowDirty
   ```

7. Inspect the generated manifest:

   ```text
   artifacts/releases/ISBM-2.0-Client-SDK-v0.5.0-manifest.txt
   ```

8. Inspect the ZIP contents and confirm the top-level folder is correct.
9. Verify prohibited files are absent, including `.git`, `.vs`, `bin`, `obj`, `packages`, `*.nupkg`, `*.snupkg`, `*.user`, `*.pdb`, `Configs.json`, `CSharp/NuGet.config`, `CSharp/ValidationRunner`, and generated release artifacts.
10. Create the SDK Git tag.
11. Create the GitHub Release.
12. Attach the custom SDK ZIP.
13. Allow GitHub to provide its automatic source ZIP and tar.gz archives.

## Packaged Content

The package includes public SDK material from tracked repository files:

- `README.md`
- `LICENSE`
- `Documents/`
- `CSharp/Windows/`
- `CSharp/Raspberry-Pi-OS/`

The package preserves the repository structure beneath one top-level release folder. The root `.gitignore` and this `RELEASE.md` file are not included because they are repository maintenance metadata rather than user-facing SDK content.

The package also includes generated, untracked release content:

- `README.html`
- HTML counterparts for end-user Use Case Markdown files under `Documents/Use_Cases/`
- `Sample Runtime/Windows/` with the five Windows sample executables and required runtime dependencies
- `Sample Runtime/Raspberry-Pi-OS/` with framework-dependent .NET 8 publish outputs
- `Self-contained Deployment/Raspberry-Pi-OS/` with `linux-arm` self-contained .NET 8 publish outputs

Generated runtime folders must be assembled from fresh build or publish output during packaging. Do not copy existing `bin`, `obj`, `packages`, or developer build directories into the package.

The generated manifest is written next to the ZIP rather than included inside it. Keeping the manifest separate makes it useful as release-audit evidence without adding generated metadata to the SDK contents.

## Packaging Implementation

The packaging script stages all package content under a temporary OS directory outside the repository. It exports the approved tracked content from `HEAD`, generates HTML documentation into the staging tree, builds/publishes runtime outputs from a separate temporary build tree, validates the final staging tree, writes a sorted manifest, creates the ZIP, validates the ZIP against the manifest, extracts the ZIP to a short temporary path, and runs post-package validation.

The default package includes source, generated HTML, Windows runtime, Raspberry Pi framework-dependent runtime, and Raspberry Pi self-contained deployment. Optional switches:

```powershell
.\scripts\Package-Sdk.ps1 -Version 0.5.0 -SourceOnly
.\scripts\Package-Sdk.ps1 -Version 0.5.0 -SkipHtml
.\scripts\Package-Sdk.ps1 -Version 0.5.0-review3 -AllowDirty
```

## Runtime Generation

Windows sample runtime generation uses Visual Studio/MSBuild:

```powershell
MSBuild.exe <project>.csproj /restore /p:Configuration=Release /p:Platform=AnyCPU
```

Only runtime-needed files are staged: the sample executable, `.exe.config`, `RapidRedPanda.ISBM.ClientAdapter.dll`, `Newtonsoft.Json.dll`, and BOD payload files where applicable. Debug symbols, caches, logs, XML documentation files, and build directories are excluded.

Raspberry Pi OS framework-dependent runtime generation uses:

```powershell
dotnet publish <project>.csproj -c Release --self-contained false -p:UseAppHost=false
```

Raspberry Pi OS self-contained deployment generation uses:

```powershell
dotnet publish <project>.csproj -c Release -r linux-arm --self-contained true -p:PublishSingleFile=false -p:PublishTrimmed=false
```

Both Raspberry Pi package areas include `Configs-Example.json` and sample payload JSON. Real `Configs.json` files are prohibited everywhere in the final package.

## Post-Package Validation

The packaging script validates:

- required source files and projects
- generated `README.html`
- generated Use Case HTML files
- local HTML links and image references
- required Windows runtime files
- required Raspberry Pi framework-dependent runtime files
- required Raspberry Pi self-contained runtime files
- absence of prohibited files
- manifest and ZIP file-list equality
- restore/build of all five packaged Windows source projects with an isolated NuGet package cache

## Release Notes Template

### Highlights

### New Features & Enhancements

### Platform Updates

### ClientAdapter

### Windows Samples

### Raspberry Pi Samples

### Configuration & Templates

### Migration Notes

### Known Limitations
