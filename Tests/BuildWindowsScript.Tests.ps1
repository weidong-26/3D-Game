$ErrorActionPreference = "Continue"

$projectRoot = Split-Path -Parent $PSScriptRoot
$buildScript = Join-Path $projectRoot "Tools\Build-Windows.ps1"
$missingEditor = Join-Path $projectRoot "Tests\missing-unity\Unity.exe"

$output = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $buildScript -UnityEditor $missingEditor 2>&1
$exitCode = $LASTEXITCODE
$message = $output -join [Environment]::NewLine

if ($exitCode -eq 0) {
    throw "Build script reported success with a missing Unity Editor."
}

if ($message -notmatch "Unity 6000\.0\.40f1") {
    throw "Build script did not clearly identify the required Unity version. Output: $message"
}

Write-Host "PASS missing Unity Editor is reported clearly."

$wrongVersion = (Get-Command powershell.exe).Source
$output = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $buildScript -UnityEditor $wrongVersion 2>&1
$exitCode = $LASTEXITCODE
$message = $output -join [Environment]::NewLine

if ($exitCode -eq 0) {
    throw "Build script accepted a non-Unity executable."
}

if ($message -notmatch "requires Unity 6000\.0\.40f1") {
    throw "Build script did not reject the wrong editor version clearly. Output: $message"
}

Write-Host "PASS wrong Unity Editor version is rejected."
