$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$source = Join-Path $root "Assets\GeniaClipboard.ico.b64"
$target = Join-Path $root "Assets\GeniaClipboard.ico"

if (-not (Test-Path $source)) {
    throw "Missing icon source: $source"
}

$base64 = (Get-Content -Raw $source).Trim()
[IO.File]::WriteAllBytes($target, [Convert]::FromBase64String($base64))
