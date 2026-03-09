---
title: "Build and Package"
---

# Build and Package

Build, test, and pack from repository root:

```bash
dotnet restore
dotnet build -c Release --no-restore
dotnet test -c Release --no-build
dotnet pack -c Release --no-build -o artifacts/packages
```

Packages are written to `artifacts/packages` as `.nupkg` and `.snupkg` files.

## Build The Sample App

```bash
dotnet build samples/AvaloniaDemo.Desktop/AvaloniaDemo.Desktop.csproj -c Release
```

## Build Documentation

```bash
./build-docs.sh
./check-docs.sh
./serve-docs.sh
```

PowerShell:

```powershell
./build-docs.ps1
./serve-docs.ps1
```

Documentation output is written to `site/.lunet/build/www`.

## CI

- `build.yml` builds, tests, and packs the repository
- `docs.yml` builds the Lunet site and publishes it to GitHub Pages
- `release.yml` builds release packages and publishes NuGet artifacts
