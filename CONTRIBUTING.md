# Contributing to GeniaClipboard

**English** · [Русский](CONTRIBUTING_RU.md)

Thank you for your interest in the project.

## Requirements

- Windows 10/11 x64;
- .NET 8 SDK or Visual Studio with the .NET desktop development workload;
- Git.

## Local build

```powershell
dotnet restore GeniaClipboard.csproj
dotnet build GeniaClipboard.csproj -c Release
dotnet publish GeniaClipboard.csproj -c Release
```

You can also use `build-portable.cmd`.

## Before opening a pull request

Please test normal clipboard capture, exact duplicates, self-copy protection, multi-selection, batch copy/paste, search, pinning, deletion, TXT export, auto journal, `Ctrl+Shift+V`, and tray behavior.

Do not include real passwords, tokens, personal information, or private clipboard contents in issues, logs, screenshots, or test files.

## Change guidelines

- keep the existing C# style and nullable annotations;
- avoid new dependencies unless they are clearly justified;
- accompany clipboard behavior changes with an explicit test scenario;
- account for WinAPI limitations, including possible `SetForegroundWindow` failure;
- call out security-relevant changes separately in the pull request.

## Bug reports

Include the GeniaClipboard version, Windows version, reproduction steps, expected behavior, and actual behavior. Never publish secrets from your clipboard.
