$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $PSScriptRoot
$gamePath = Join-Path $projectRoot "Build\Windows\Luminia.exe"
$buildLog = Join-Path $projectRoot "Build\unity-build.log"

function Find-UnityEditor {
    $searchRoots = @(
        (Join-Path $env:ProgramFiles "Unity\Hub\Editor"),
        (Join-Path ${env:ProgramFiles(x86)} "Unity\Hub\Editor")
    ) | Where-Object { $_ -and (Test-Path $_) }

    $editors = foreach ($root in $searchRoots) {
        Get-ChildItem -Path $root -Directory -ErrorAction SilentlyContinue | ForEach-Object {
            $candidate = Join-Path $_.FullName "Editor\Unity.exe"
            if (Test-Path $candidate) {
                [PSCustomObject]@{
                    Version = $_.Name
                    Path = $candidate
                }
            }
        }
    }

    return $editors |
        Sort-Object { try { [version]($_.Version -replace '[^0-9.].*$', '') } catch { [version]'0.0' } } -Descending |
        Select-Object -First 1
}

try {
    $sourceFiles = Get-ChildItem -Path (Join-Path $projectRoot "Assets"), (Join-Path $projectRoot "Packages"), (Join-Path $projectRoot "ProjectSettings") -File -Recurse -ErrorAction SilentlyContinue
    $latestSource = $sourceFiles | Sort-Object LastWriteTimeUtc -Descending | Select-Object -First 1
    $needsBuild = -not (Test-Path $gamePath)
    if (-not $needsBuild -and $latestSource) {
        $needsBuild = $latestSource.LastWriteTimeUtc -gt (Get-Item $gamePath).LastWriteTimeUtc
    }

    if (-not $needsBuild) {
        Write-Host "[Luminia] Existing Windows game found. Starting..." -ForegroundColor Green
        Start-Process -FilePath $gamePath -WorkingDirectory (Split-Path $gamePath)
        exit 0
    }

    Write-Host "[Luminia] First run: preparing a Windows game build." -ForegroundColor Cyan
    Write-Host "[Luminia] This can take several minutes." -ForegroundColor Cyan

    $unity = Find-UnityEditor
    if (-not $unity) {
        Write-Host ""
        Write-Host "Unity Editor was not found." -ForegroundColor Red
        Write-Host "Install Unity Hub and Unity 6, then double-click run-luminia.bat again."
        Write-Host "Unity Hub download: https://unity.com/download"
        Start-Process "https://unity.com/download"
        exit 10
    }

    Write-Host "[Luminia] Unity found: $($unity.Version)"
    Write-Host "[Luminia] Close Unity if this project is currently open."
    New-Item -ItemType Directory -Force -Path (Split-Path $buildLog) | Out-Null

    $arguments = @(
        "-batchmode",
        "-quit",
        "-projectPath", ('"' + $projectRoot + '"'),
        "-executeMethod", "Luminia.Editor.BuildWindows.BuildFromCommandLine",
        "-logFile", ('"' + $buildLog + '"')
    )

    $process = Start-Process -FilePath $unity.Path -ArgumentList $arguments -Wait -PassThru
    if ($process.ExitCode -ne 0) {
        Write-Host "[Luminia] Unity build failed with exit code $($process.ExitCode)." -ForegroundColor Red
        Write-Host "Build log: $buildLog"
        exit $process.ExitCode
    }

    if (-not (Test-Path $gamePath)) {
        Write-Host "[Luminia] Build finished but Luminia.exe was not created." -ForegroundColor Red
        Write-Host "Build log: $buildLog"
        exit 11
    }

    Write-Host "[Luminia] Build complete. Starting the game..." -ForegroundColor Green
    Start-Process -FilePath $gamePath -WorkingDirectory (Split-Path $gamePath)
    exit 0
}
catch {
    Write-Host "[Luminia] Unexpected launcher error:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 12
}
