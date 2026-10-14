<#
.SYNOPSIS
    Checks the whole project: headless build and tests, doc lint, and optionally Unity tests and regenerated docs.

.DESCRIPTION
    -Unity  also runs Unity's edit-mode tests through Tools/unity.ps1 (the editor must be closed).
    -Docs   regenerates every generated doc first (doctool generate --sim).
    Exit code 0 when every check passes, 1 otherwise.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File Tools/verify.ps1 -Unity -Docs
#>
param(
    [switch]$Unity,
    [switch]$Docs
)

$root = Split-Path -Parent $PSScriptRoot
Set-Location $root
$failed = @()

if ($Docs) {
    Write-Host "== Regenerating docs"
    python Tools/docs/doctool.py generate --sim
    if ($LASTEXITCODE -ne 0) { $failed += 'doc generation' }
}

Write-Host "== Headless build and tests"
dotnet test Tools/Headless/Statecraft.Headless.slnx --nologo -v q
if ($LASTEXITCODE -ne 0) { $failed += 'headless tests' }

Write-Host "== Doc lint"
python Tools/docs/doctool.py lint --strict
if ($LASTEXITCODE -ne 0) { $failed += 'doc lint' }

if ($Unity) {
    Write-Host "== Unity edit-mode tests"
    powershell -NoProfile -ExecutionPolicy Bypass -File (Join-Path $PSScriptRoot 'unity.ps1') -Mode test
    if ($LASTEXITCODE -ne 0) { $failed += 'Unity tests' }
}

if ($failed.Count -gt 0) {
    Write-Host "FAILED: $($failed -join ', ')"
    exit 1
}
Write-Host "All checks passed."
exit 0
