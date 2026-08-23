# MarkdownEditor 一键构建脚本（PowerShell）
# 要求: .NET 10 SDK
# 产物: src\bin\Release\net10.0-windows\win-x64\publish\MarkdownEditor.exe

$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot\src

Write-Host "===========================================" -ForegroundColor Cyan
Write-Host " Building MarkdownEditor (Release)" -ForegroundColor Cyan
Write-Host "===========================================" -ForegroundColor Cyan

dotnet publish -c Release -r win-x64 --nologo

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "BUILD FAILED" -ForegroundColor Red
    exit 1
}

$exe = "bin\Release\net10.0-windows\win-x64\publish\MarkdownEditor.exe"
if (Test-Path $exe) {
    $size = (Get-Item $exe).Length
    Write-Host ""
    Write-Host "===========================================" -ForegroundColor Green
    Write-Host " Build succeeded!" -ForegroundColor Green
    Write-Host " Output: $exe" -ForegroundColor Green
    Write-Host " Size:   $([Math]::Round($size / 1MB, 2)) MB" -ForegroundColor Green
    Write-Host "===========================================" -ForegroundColor Green

    # 复制到 dist
    Copy-Item -Path $exe -Destination "..\dist\MarkdownEditor.exe" -Force
    Write-Host " Copied to dist\MarkdownEditor.exe" -ForegroundColor Green
}
