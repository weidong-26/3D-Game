$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$assets = @('WoodFloor040', 'Concrete045', 'Metal029')
foreach ($assetId in $assets) {
    $name = "${assetId}_1K-JPG.zip"
    $url = "https://ambientcg.com/get?file=$name"
    $directory = Join-Path $projectRoot "Assets/Art/Materials/ambientCG/$assetId"
    New-Item -ItemType Directory -Force -Path $directory | Out-Null
    $archive = Join-Path $env:TEMP $name
    $extract = Join-Path $env:TEMP "${assetId}_home_material"
    Invoke-WebRequest -Uri $url -OutFile $archive
    Expand-Archive -LiteralPath $archive -DestinationPath $extract -Force
    foreach ($suffix in @('Color', 'NormalDX', 'Roughness')) {
        $source = Join-Path $extract "${assetId}_1K-JPG_${suffix}.jpg"
        Copy-Item -LiteralPath $source -Destination $directory
    }
    $resolvedExtract = (Resolve-Path -LiteralPath $extract).Path
    $resolvedTemp = (Resolve-Path -LiteralPath $env:TEMP).Path.TrimEnd([IO.Path]::DirectorySeparatorChar)
    if (-not $resolvedExtract.StartsWith($resolvedTemp + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Unexpected extraction path: $resolvedExtract"
    }
    Remove-Item -LiteralPath $extract -Recurse -Force
    Remove-Item -LiteralPath $archive
    Write-Host "Downloaded $assetId"
}
