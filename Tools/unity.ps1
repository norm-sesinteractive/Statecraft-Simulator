<#
.SYNOPSIS
    Runs Unity in batch mode for this project: compile check, edit-mode tests, or project setup.

.DESCRIPTION
    Uses the editor version named in ProjectSettings/ProjectVersion.txt. The editor must be closed:
    Unity locks the project, so a second instance fails. Logs and test results land in Logs/batch/
    (git-ignored). The headless harness (Tools/Headless) is the fast path for sim work and runs
    with the editor open; use this script for everything that needs UnityEngine.

    Modes:
      compile  Imports and compiles every assembly, then quits.
      test     Runs the edit-mode tests (Unity Test Runner) and summarizes the results.
      setup    Applies player settings and rebuilds Assets/Scenes/Main.unity (ProjectSetup.RunFromCommandLine).

    Exit codes: 0 success, 1 compile or setup failure, 2 test failures, 3 editor open or Unity missing.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File Tools/unity.ps1 -Mode test
#>
param(
    [ValidateSet('compile', 'test', 'setup')]
    [string]$Mode = 'compile',
    [string]$BuildTarget = 'Android',
    [string]$OutDir
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
if (-not $OutDir) { $OutDir = Join-Path $root 'Logs\batch' }
New-Item -ItemType Directory -Force -Path $OutDir | Out-Null

$versionMatch = Select-String -Path (Join-Path $root 'ProjectSettings\ProjectVersion.txt') -Pattern '^m_EditorVersion:\s*(\S+)' | Select-Object -First 1
$version = $versionMatch.Matches[0].Groups[1].Value
$candidates = @(
    "C:\Program Files\Unity\Hub\Editor\$version\Editor\Unity.exe",
    "D:\Program Files\Unity\Hub\Editor\$version\Editor\Unity.exe",
    "D:\Program Files D\Unity\Hub\Editor\$version\Editor\Unity.exe"
)
$unity = $candidates | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $unity) {
    Write-Host "Unity $version not found. Looked in: $($candidates -join '; ')"
    exit 3
}

# Any Unity process whose command line names this project holds it. An open editor fails fast; a batch
# run that is still shutting down (its import workers linger a few seconds) is waited out briefly.
function Get-ProjectHolders {
    @(Get-CimInstance Win32_Process -Filter "Name='Unity.exe'" -ErrorAction SilentlyContinue |
        Where-Object { $_.CommandLine -and $_.CommandLine.Replace('/', '\').ToLowerInvariant().Contains($root.ToLowerInvariant()) })
}
$deadline = (Get-Date).AddSeconds(30)
$holders = Get-ProjectHolders
while ($holders.Count -gt 0 -and (Get-Date) -lt $deadline) {
    $editorOpen = @($holders | Where-Object { $_.CommandLine -notmatch '(?i)-batchmode' }).Count -gt 0
    if ($editorOpen) { break }
    Start-Sleep -Seconds 2
    $holders = Get-ProjectHolders
}
if ($holders.Count -gt 0) {
    Write-Host "The Unity editor (or another batch run) has this project open. Close it, or use the headless harness:"
    Write-Host "  dotnet test Tools/Headless/Statecraft.Headless.slnx"
    exit 3
}

$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$log = Join-Path $OutDir "unity-$Mode-$stamp.log"
$results = Join-Path $OutDir "results-$stamp.xml"
$arguments = @('-batchmode', '-projectPath', "`"$root`"", '-buildTarget', $BuildTarget, '-logFile', "`"$log`"")
switch ($Mode) {
    'compile' { $arguments += @('-quit', '-nographics') }
    'setup'   { $arguments += @('-quit', '-executeMethod', 'Statecraft.Editor.ProjectSetup.RunFromCommandLine') }
    'test'    { $arguments += @('-nographics', '-runTests', '-testPlatform', 'EditMode', '-testResults', "`"$results`"") }
}

Write-Host "Unity $version, mode '$Mode', target $BuildTarget"
Write-Host "Log: $log"
$started = Get-Date
$process = Start-Process -FilePath $unity -ArgumentList $arguments -PassThru -Wait -NoNewWindow
$seconds = [int]((Get-Date) - $started).TotalSeconds
Write-Host "Unity exited with code $($process.ExitCode) after $seconds s"

$compileErrors = @()
$compileWarnings = @()
if (Test-Path $log) {
    $compileErrors = @(Select-String -Path $log -Pattern 'error CS\d+' | ForEach-Object { $_.Line.Trim() } | Sort-Object -Unique)
    $compileWarnings = @(Select-String -Path $log -Pattern 'warning CS\d+' | ForEach-Object { $_.Line.Trim() } | Sort-Object -Unique)
}
Write-Host "Compiler: $($compileErrors.Count) error(s), $($compileWarnings.Count) warning(s)"
$compileErrors | Select-Object -First 30 | ForEach-Object { Write-Host "  $_" }
$compileWarnings | Select-Object -First 10 | ForEach-Object { Write-Host "  $_" }

$failedTests = 0
if ($Mode -eq 'test') {
    if (Test-Path $results) {
        [xml]$xml = Get-Content -LiteralPath $results
        $run = $xml.'test-run'
        $failedTests = [int]$run.failed
        Write-Host "Tests: $($run.total) total, $($run.passed) passed, $($run.failed) failed, $($run.skipped) skipped"
        foreach ($case in $xml.SelectNodes("//test-case[@result='Failed']")) {
            Write-Host "  FAILED: $($case.fullname)"
            if ($case.failure -and $case.failure.message) { Write-Host "    $($case.failure.message.InnerText.Trim())" }
        }
    } else {
        Write-Host "No test results file was written; see the log."
        $failedTests = -1
    }
}

if ($compileErrors.Count -gt 0) { exit 1 }
if ($failedTests -ne 0) { exit 2 }
if ($process.ExitCode -ne 0) {
    Write-Host "Last log lines:"
    if (Test-Path $log) { Get-Content -LiteralPath $log -Tail 30 | ForEach-Object { Write-Host "  $_" } }
    exit 1
}
exit 0
