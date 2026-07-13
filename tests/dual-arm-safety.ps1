param(
    [string]$Root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
)

$upDownLoad = Get-Content -LiteralPath (Join-Path $Root 'UI/Class/UpDownLoad.cs') -Raw
$myfunc = Get-Content -LiteralPath (Join-Path $Root 'UI/Class/myfunc.cs') -Raw

$failures = New-Object System.Collections.Generic.List[string]

if ($upDownLoad -notmatch 'MT\.BeQuitEn\(true,\s*true\)') {
    $failures.Add('UpDownLoad abnormal exit must set the global quit flag.')
}

if ($upDownLoad -notmatch 'UD1Stop\(\);\s*[\r\n\s]*UD2Stop\(\);') {
    $failures.Add('UpDownLoad abnormal exit must stop both arms immediately.')
}

if ($myfunc -notmatch 'RecoverUpDownLoadBeforeRun\(ref\s+VAR\.gsys_set\.bquit\)') {
    $failures.Add('Run startup must recover up/down-load arms before launching concurrent tasks.')
}

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Host 'Dual-arm safety source checks passed.'
