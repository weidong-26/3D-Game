$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$assetIds = @(
    'SchoolChair_01',
    'SchoolDesk_01',
    'standing_chalkboard_01',
    'binder_notebook',
    'book_encyclopedia_set_01',
    'mounted_fluorescent_lights',
    'painted_wooden_bench',
    'fir_sapling',
    'shrub_04',
    'street_lamp_02',
    'metal_trash_can',
    'wooden_picnic_table',
    'modular_urban_apartments_facade',
    'modular_factory_facade',
    'fire_hydrant',
    'water_manhole_cover'
)
$records = @()
foreach ($assetId in $assetIds) {
    $files = Invoke-RestMethod -Uri "https://api.polyhaven.com/files/$assetId"
    $assetDirectory = Join-Path $projectRoot "Assets/Art/EnvironmentPhase2/PolyHaven/$assetId"
    New-Item -ItemType Directory -Force -Path $assetDirectory | Out-Null
    $downloads = @($files.fbx.'1k'.fbx)
    foreach ($entry in $files.PSObject.Properties) {
        if ($entry.Name -match '(?i)(diff|nor_dx|rough|metal|alpha|ao|emissive)' -and $entry.Value.'1k'.jpg) {
            $downloads += $entry.Value.'1k'.jpg
        }
    }
    $downloads = @($downloads | Where-Object { $_ -ne $null } | Sort-Object url -Unique)
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
$manifest = Join-Path $projectRoot 'Assets/Art/EnvironmentPhase2/PolyHaven/download_manifest.csv'
$records | Export-Csv -NoTypeInformation -Encoding UTF8 -Path $manifest

