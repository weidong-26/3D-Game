$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$assetIds = @(
    'modern_arm_chair_01',
    'modern_coffee_table_01',
    'modern_wooden_cabinet',
    'drawer_cabinet',
    'dining_chair_02',
    'side_table_01',
    'modern_ceiling_lamp_01'
)
$records = @()
foreach ($assetId in $assetIds) {
    $files = Invoke-RestMethod -Uri "https://api.polyhaven.com/files/$assetId"
    $assetDirectory = Join-Path $projectRoot "Assets/Art/Furniture/PolyHaven/$assetId"
    New-Item -ItemType Directory -Force -Path $assetDirectory | Out-Null
    $downloads = @($files.fbx.'1k'.fbx)
    foreach ($entry in $files.PSObject.Properties) {
        if ($entry.Name -match '(?i)(diff|nor_dx|rough|metal)' -and $entry.Value.'1k'.jpg) {
            $downloads += $entry.Value.'1k'.jpg
        }
    }
    foreach ($download in $downloads) {
        $fileName = [System.IO.Path]::GetFileName(([uri]$download.url).AbsolutePath)
        $target = Join-Path $assetDirectory $fileName
        if (-not (Test-Path -LiteralPath $target)) {
            Invoke-WebRequest -Uri $download.url -OutFile $target
        }
        $actualHash = (Get-FileHash -Algorithm MD5 -LiteralPath $target).Hash.ToLowerInvariant()
        if ($actualHash -ne $download.md5.ToLowerInvariant()) {
            throw "MD5 mismatch: $target"
        }
        $records += [pscustomobject]@{
            asset = $assetId
            file = $fileName
            url = $download.url
            md5 = $actualHash
        }
    }
    Write-Host "Verified $assetId ($($downloads.Count) files)"
}
$records | Export-Csv -NoTypeInformation -Encoding UTF8 -Path (Join-Path $projectRoot 'Assets/Art/Furniture/PolyHaven/download_manifest.csv')
