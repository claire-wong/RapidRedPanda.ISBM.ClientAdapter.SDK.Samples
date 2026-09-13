# Release Packaging

This repository publishes the ISBM 2.0 Client SDK samples as a custom SDK ZIP attached to each GitHub Release. GitHub also provides automatic source archives for every tag.

The custom ZIP is generated from approved tracked repository content only. Do not create release ZIPs manually from Windows Explorer or from a working directory copy.

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

7. Inspect the generated manifest:

   ```text
   artifacts/releases/ISBM-2.0-Client-SDK-v0.5.0-manifest.txt
   ```

8. Inspect the ZIP contents and confirm the top-level folder is correct.
9. Verify prohibited files are absent, including `.git`, `.vs`, `bin`, `obj`, `packages`, `*.nupkg`, `*.snupkg`, `*.user`, `Configs.json`, `CSharp/NuGet.config`, `CSharp/ValidationRunner`, and generated release artifacts.
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

The package preserves the repository structure beneath one top-level release folder. The root `.gitignore` is not included because it is repository maintenance metadata rather than user-facing SDK content.

The generated manifest is written next to the ZIP rather than included inside it. Keeping the manifest separate makes it useful as release-audit evidence without adding generated metadata to the SDK contents.

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
