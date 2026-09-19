param(
    [string]$UnityEditor = ""
)

$ErrorActionPreference = "Stop"
$requiredVersion = "6000.0.40f1"
$projectRoot = [System.IO.Path]::GetFullPath((Split-Path -Parent $PSScriptRoot))
$outputDirectory = Join-Path $projectRoot "Builds\Windows"
$outputExe = Join-Path $outputDirectory "3DGamePrototype.exe"
$logDirectory = Join-Path $projectRoot "Logs"
$logPath = Join-Path $logDirectory "windows-build.log"

function Find-UnityEditor {
    if ($UnityEditor) {
        if (Test-Path -LiteralPath $UnityEditor -PathType Leaf) {
            return [System.IO.Path]::GetFullPath($UnityEditor)
        }

        throw "Unity 6000.0.40f1 Editor was not found at: $UnityEditor"
    }

    $candidates = @()
    foreach ($drive in Get-PSDrive -PSProvider FileSystem) {
        $candidates += Join-Path $drive.Root "Program Files\Unity\Hub\Editor\$requiredVersion\Editor\Unity.exe"
        $candidates += Join-Path $drive.Root "Unity\Hub\Editor\$requiredVersion\Editor\Unity.exe"
    }

    foreach ($candidate in $candidates) {
        if (Test-Path -LiteralPath $candidate -PathType Leaf) {
            return $candidate
        }
    }

    throw "Unity 6000.0.40f1 Editor is not installed or was not found. Install it with Unity Hub, including Windows Build Support, or pass -UnityEditor with the full Unity.exe path."
}

function Confirm-WindowsBuildSupport([string]$editorPath) {
    $editorDirectory = Split-Path -Parent $editorPath
    $modulePath = Join-Path $editorDirectory "Data\PlaybackEngines\windowsstandalonesupport"
    if (-not (Test-Path -LiteralPath $modulePath -PathType Container)) {
        throw "Windows Build Support is missing for Unity 6000.0.40f1. Add the module in Unity Hub, then run this script again. Expected module path: $modulePath"
    }
}

function Confirm-UnityVersion([string]$editorPath) {
    $productVersion = (Get-Item -LiteralPath $editorPath).VersionInfo.ProductVersion
    $pathHasRequiredVersion = $editorPath -match [regex]::Escape($requiredVersion)
    $fileHasRequiredVersion = $productVersion -match '^6000\.0\.40'

    if (-not $pathHasRequiredVersion -and -not $fileHasRequiredVersion) {
        $found = if ($productVersion) { $productVersion } else { "unknown" }
        throw "This build requires Unity 6000.0.40f1. The selected executable reports version '$found': $editorPath"
    }
}

function Confirm-BuildOutput {
    $dataDirectory = Join-Path $outputDirectory "3DGamePrototype_Data"
    $unityPlayer = Join-Path $outputDirectory "UnityPlayer.dll"
    $missing = @()

    if (-not (Test-Path -LiteralPath $outputExe -PathType Leaf)) { $missing += "3DGamePrototype.exe" }
    if (-not (Test-Path -LiteralPath $dataDirectory -PathType Container)) { $missing += "3DGamePrototype_Data" }
    if (-not (Test-Path -LiteralPath $unityPlayer -PathType Leaf)) { $missing += "UnityPlayer.dll" }

    if ($missing.Count -gt 0) {
        throw "Unity reported a successful process exit, but the Windows build is incomplete. Missing: $($missing -join ', ')"
    }

    $dataFiles = Get-ChildItem -LiteralPath $dataDirectory -File -Recurse -ErrorAction Stop
    if ($dataFiles.Count -eq 0) {
        throw "The Windows build is incomplete: 3DGamePrototype_Data contains no files."
    }

    $exeSize = (Get-Item -LiteralPath $outputExe).Length
    $playerSize = (Get-Item -LiteralPath $unityPlayer).Length
    if ($exeSize -eq 0 -or $playerSize -eq 0) {
        throw "The Windows build contains an empty executable or UnityPlayer.dll."
    }

    Write-Host "BUILD_SUCCESS"
    Write-Host "Executable: $outputExe ($exeSize bytes)"
    Write-Host "Data folder: $dataDirectory ($($dataFiles.Count) files)"
    Write-Host "Unity runtime: $unityPlayer ($playerSize bytes)"
}

$resolvedEditor = Find-UnityEditor
Confirm-UnityVersion $resolvedEditor
Confirm-WindowsBuildSupport $resolvedEditor
New-Item -ItemType Directory -Path $outputDirectory, $logDirectory -Force | Out-Null

Write-Host "Unity Editor: $resolvedEditor"
Write-Host "Project: $projectRoot"
Write-Host "Output: $outputExe"

$unityArguments = @(
    "-batchmode",
    "-quit",
    "-nographics",
    "-DisableDirectoryMonitor",
    "-accept-apiupdate",
    "-projectPath", $projectRoot,
    "-buildTarget", "StandaloneWindows64",
    "-executeMethod", "LightweightGame.Editor.WindowsBuild.Build",
    "-logFile", $logPath
)
$unityProcess = Start-Process -FilePath $resolvedEditor -ArgumentList $unityArguments -Wait -PassThru -WindowStyle Hidden

if ($unityProcess.ExitCode -ne 0) {
    throw "Unity Windows build failed with exit code $($unityProcess.ExitCode). See the build log: $logPath"
}

Confirm-BuildOutput
