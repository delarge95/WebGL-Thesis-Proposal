# Gamma to Linear Color Converter for USS files
# Formula: linear = (gamma/255)^2.2 * 255

$files = @(
    "e:\WebGL_tesis\desarrollo\unity_project\Assets\UI\Styles\Theme.uss",
    "e:\WebGL_tesis\desarrollo\unity_project\Assets\UI\Styles\Hotspots.uss"
)

# Build lookup table for all 0-255 values
$lut = @()
for ($i = 0; $i -le 255; $i++) {
    if ($i -eq 0) { $lut += 0 }
    elseif ($i -eq 255) { $lut += 255 }
    else {
        $linear = [Math]::Round([Math]::Pow($i / 255.0, 2.2) * 255.0)
        $lut += [int]$linear
    }
}

# Print some key conversions for verification
Write-Host "=== Lookup Table Samples ==="
Write-Host "  10 -> $($lut[10])"
Write-Host "  18 -> $($lut[18])"
Write-Host "  24 -> $($lut[24])"
Write-Host "  50 -> $($lut[50])"
Write-Host " 100 -> $($lut[100])"
Write-Host " 170 -> $($lut[170])"
Write-Host " 200 -> $($lut[200])"
Write-Host " 230 -> $($lut[230])"
Write-Host " 255 -> $($lut[255])"
Write-Host ""

foreach ($file in $files) {
    Write-Host "Processing: $file"
    $content = Get-Content $file -Raw
    $count = 0

    # Convert rgb(r, g, b)
    $content = [regex]::Replace($content, 'rgb\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*\)', {
        param($m)
        $r = $lut[[int]$m.Groups[1].Value]
        $g = $lut[[int]$m.Groups[2].Value]
        $b = $lut[[int]$m.Groups[3].Value]
        $script:count++
        "rgb($r, $g, $b)"
    })

    # Convert rgba(r, g, b, a) - alpha unchanged
    $content = [regex]::Replace($content, 'rgba\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*([0-9.]+)\s*\)', {
        param($m)
        $r = $lut[[int]$m.Groups[1].Value]
        $g = $lut[[int]$m.Groups[2].Value]
        $b = $lut[[int]$m.Groups[3].Value]
        $a = $m.Groups[4].Value
        $script:count++
        "rgba($r, $g, $b, $a)"
    })

    # Convert #RRGGBB hex
    $content = [regex]::Replace($content, '#([0-9A-Fa-f]{6})\b', {
        param($m)
        $hex = $m.Groups[1].Value
        $r = $lut[[Convert]::ToInt32($hex.Substring(0,2), 16)]
        $g = $lut[[Convert]::ToInt32($hex.Substring(2,2), 16)]
        $b = $lut[[Convert]::ToInt32($hex.Substring(4,2), 16)]
        $script:count++
        "#{0:X2}{1:X2}{2:X2}" -f $r, $g, $b
    })

    Set-Content $file -Value $content -NoNewline -Encoding UTF8
    Write-Host "  Converted $count color expressions."
}

Write-Host ""
Write-Host "Done! Gamma -> Linear conversion complete."
