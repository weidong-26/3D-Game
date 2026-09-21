$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$assets = @('Asphalt033', 'PavingStones151', 'Grass005', 'Tiles141', 'PaintedPlaster017')
foreach ($assetId in $assets) {
    $name = "${assetId}_1K-JPG.zip"
    $url = "https://ambientcg.com/get?file=$name"
    $directory = Join-Path $projectRoot "Assets/Art/EnvironmentPhase2/ambientCG/$assetId"
    New-Item -ItemType Directory -Force -Path $directory | Out-Null
    $archive = Join-Path $env:TEMP $name
    $extract = Join-Path $env:TEMP ("phase2_" + $assetId + '_' + [guid]::NewGuid().ToString('N'))
    Invoke-WebRequest -Uri $url -OutFile $archive
    Expand-Archive -LiteralPath $archive -DestinationPath $extract
    foreach ($suffix in @('Color', 'NormalDX', 'Roughness')) {
        $source = Join-Path $extract "${assetId}_1K-JPG_${suffix}.jpg"
        if (-not (Test-Path -LiteralPath $source)) { throw "Missing material map: $source" }
        Copy-Item -LiteralPath $source -Destination $directory
    }
    $resolvedExtract = [System.IO.Path]::GetFullPath($extract)
    $resolvedTemp = [System.IO.Path]::GetFullPath($env:TEMP).TrimEnd([IO.Path]::DirectorySeparatorChar)
    if (-not $resolvedExtract.StartsWith($resolvedTemp + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Unexpected extraction path: $resolvedExtract"
    }
    Remove-Item -LiteralPath $extract -Recurse -Force
    Remove-Item -LiteralPath $archive -Force
    Write-Host "Downloaded $assetId"
}

