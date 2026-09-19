param(
    [string]$UnityEditor = ""
)

$ErrorActionPreference = "Stop"
$projectRoot = Split-Path -Parent $PSScriptRoot
$compiler = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
$testExecutable = Join-Path $projectRoot "Tests\CoreTests.exe"

if (-not (Test-Path -LiteralPath $compiler)) {
    throw "C# compiler not found: $compiler"
}

& $compiler /nologo /out:$testExecutable `
    (Join-Path $projectRoot "Assets\Scripts\Core\AppearanceData.cs") `
    (Join-Path $projectRoot "Tests\CoreTests.cs")
if ($LASTEXITCODE -ne 0) {
    throw "Core test compilation failed."
}

& $testExecutable
if ($LASTEXITCODE -ne 0) {
    throw "Core tests failed."
}

$movementExecutable = Join-Path $projectRoot "Tests\MovementRulesTests.exe"
& $compiler /nologo /out:$movementExecutable `
    (Join-Path $projectRoot "Assets\Scripts\Core\MovementRules.cs") `
    (Join-Path $projectRoot "Tests\MovementRulesTests.cs")
if ($LASTEXITCODE -ne 0) { throw "Movement test compilation failed." }
& $movementExecutable
if ($LASTEXITCODE -ne 0) { throw "Movement rules tests failed." }

$syntaxAssembly = Join-Path $projectRoot "Tests\UnityScriptsSyntax.dll"
$runtimeSources = Get-ChildItem -LiteralPath (Join-Path $projectRoot "Assets\Scripts\Runtime") -Filter "*.cs" -File |
    Select-Object -ExpandProperty FullName
& $compiler /nologo /target:library /out:$syntaxAssembly `
    (Join-Path $projectRoot "Tests\UnityStubs.cs") `
    (Join-Path $projectRoot "Assets\Scripts\Core\AppearanceData.cs") `
    (Join-Path $projectRoot "Assets\Scripts\Core\MovementRules.cs") `
    $runtimeSources
if ($LASTEXITCODE -ne 0) {
    throw "Unity script syntax/type check failed."
}
Write-Host "Unity script syntax/type check passed with local API stubs."

$editorSyntaxAssembly = Join-Path $projectRoot "Tests\UnityEditorScriptsSyntax.dll"
& $compiler /nologo /target:library /out:$editorSyntaxAssembly `
    (Join-Path $projectRoot "Tests\UnityStubs.cs") `
    (Join-Path $projectRoot "Tests\UnityEditorStubs.cs") `
    (Join-Path $projectRoot "Assets\Editor\WindowsBuild.cs")
if ($LASTEXITCODE -ne 0) {
    throw "Unity Editor build script syntax/type check failed."
}
Write-Host "Unity Editor build script syntax/type check passed with local API stubs."

& powershell.exe -NoProfile -ExecutionPolicy Bypass -File (Join-Path $projectRoot "Tests\BuildWindowsScript.Tests.ps1")
if ($LASTEXITCODE -ne 0) {
    throw "Windows build PowerShell tests failed."
}

$required = @(
    "Assets\Scenes\Main.unity",
    "Assets\Scripts\Runtime\GameBootstrap.cs",
    "Assets\Scripts\Runtime\ProceduralHeadMesh.cs",
    "Assets\Scripts\Runtime\CharacterCreatorUI.cs",
    "Assets\Editor\WindowsBuild.cs",
    "Tools\Build-Windows.ps1",
    "ProjectSettings\ProjectVersion.txt",
    "Packages\manifest.json"
)

foreach ($relativePath in $required) {
    $path = Join-Path $projectRoot $relativePath
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Required project file is missing: $relativePath"
    }
}

if (-not $UnityEditor) {
    $editorRoots = @()
    foreach ($drive in Get-PSDrive -PSProvider FileSystem) {
        $editorRoots += Join-Path $drive.Root "Program Files\Unity\Hub\Editor"
        $editorRoots += Join-Path $drive.Root "Unity\Hub\Editor"
    }

    $UnityEditor = Get-ChildItem -LiteralPath $editorRoots -Filter Unity.exe -File -Recurse -ErrorAction SilentlyContinue |
        Select-Object -First 1 -ExpandProperty FullName
}

if ($UnityEditor -and (Test-Path -LiteralPath $UnityEditor)) {
    $logPath = Join-Path $projectRoot "Logs\unity-compile.log"
    New-Item -ItemType Directory -Path (Split-Path -Parent $logPath) -Force | Out-Null
    $unityArguments = @(
        "-batchmode",
        "-quit",
        "-nographics",
        "-DisableDirectoryMonitor",
        "-projectPath", $projectRoot,
        "-logFile", $logPath
    )
    $unityProcess = Start-Process -FilePath $UnityEditor -ArgumentList $unityArguments -Wait -PassThru -WindowStyle Hidden
    if ($unityProcess.ExitCode -ne 0) {
        throw "Unity batch compilation failed with exit code $($unityProcess.ExitCode). See $logPath"
    }
    Write-Host "Unity batch compilation passed. Log: $logPath"
}
else {
    Write-Warning "Unity Editor was not found. Core tests and project structure passed; Unity compilation was skipped."
}

Remove-Item -LiteralPath $testExecutable, $movementExecutable, $syntaxAssembly, $editorSyntaxAssembly -Force -ErrorAction SilentlyContinue
