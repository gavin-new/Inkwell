# MarkdownEditor Screenshot Script
# Launch EXE -> wait for window -> capture window -> save PNG -> close EXE
# Usage: powershell -ExecutionPolicy Bypass -File capture-screenshot.ps1
# Output: docs/screenshots/main.png
#
# NOTE: This script avoids Chinese characters in the source because
# PowerShell 5.1 parses .ps1 files using the system ANSI code page
# which can mangle UTF-8 bytes and break string parsing. Window title
# is found by enumerating top-level windows of the process instead.

param(
    [string]$ExePath = "..\dist\MarkdownEditor.exe",
    [string]$OutputDir = "screenshots",
    [int]$WaitSeconds = 5
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $PSCommandPath
$ExeFullPath = Join-Path $ScriptDir $ExePath | Resolve-Path -ErrorAction SilentlyContinue
$OutFullDir = Join-Path $ScriptDir $OutputDir

if (-not $ExeFullPath -or -not (Test-Path $ExeFullPath)) {
    Write-Host "EXE not found: $ExePath" -ForegroundColor Red
    Write-Host "Please build first, or specify -ExePath" -ForegroundColor Yellow
    exit 1
}

if (-not (Test-Path $OutFullDir)) {
    New-Item -ItemType Directory -Path $OutFullDir -Force | Out-Null
}

# Win32 API imports + EnumWindows callback to find window by PID
$signature = @"
using System;
using System.Drawing;
using System.Runtime.InteropServices;

public class Win32Screenshot {
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    public static extern bool PrintWindow(IntPtr hWnd, IntPtr hDC, uint nFlags);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT {
        public int Left, Top, Right, Bottom;
    }

    // Find first visible top-level window of given PID
    public static IntPtr FindMainWindow(uint targetPid) {
        IntPtr found = IntPtr.Zero;
        EnumWindows((hWnd, lParam) => {
            uint pid;
            GetWindowThreadProcessId(hWnd, out pid);
            if (pid == targetPid && IsWindowVisible(hWnd)) {
                int len = GetWindowTextLength(hWnd);
                if (len > 0) {
                    found = hWnd;
                    return false; // stop
                }
            }
            return true; // continue
        }, IntPtr.Zero);
        return found;
    }
}
"@

Add-Type -AssemblyName System.Drawing
Add-Type -TypeDefinition $signature -ErrorAction SilentlyContinue

Write-Host "=== MarkdownEditor Screenshot Script ===" -ForegroundColor Cyan
Write-Host "EXE:    $ExeFullPath"
Write-Host "Output: $OutFullDir"
Write-Host ""

# 1) Launch EXE
Write-Host "[1/5] Launching EXE..." -ForegroundColor Yellow
$proc = Start-Process -FilePath $ExeFullPath -PassThru
Write-Host "  PID = $($proc.Id)"

# 2) Wait for main window (by enumerating windows of this process)
Write-Host "[2/5] Waiting for main window (max $WaitSeconds sec)..." -ForegroundColor Yellow
$hwnd = [IntPtr]::Zero
for ($i = 0; $i -lt $WaitSeconds * 10; $i++) {
    Start-Sleep -Milliseconds 100
    $hwnd = [Win32Screenshot]::FindMainWindow([uint32]$proc.Id)
    if ($hwnd -ne [IntPtr]::Zero) { break }
}

if ($hwnd -eq [IntPtr]::Zero) {
    Write-Host "  ERROR: Main window not found" -ForegroundColor Red
    Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
    exit 1
}
Write-Host "  HWND = $hwnd"

# 3) Wait for WebView2 render
Write-Host "[3/5] Waiting for page render (3 sec)..." -ForegroundColor Yellow
Start-Sleep -Seconds 3

# 4) Capture window via PrintWindow
Write-Host "[4/5] Capturing window..." -ForegroundColor Yellow
$rect = New-Object Win32Screenshot+RECT
[Win32Screenshot]::GetWindowRect($hwnd, [ref]$rect) | Out-Null
$width = $rect.Right - $rect.Left
$height = $rect.Bottom - $rect.Top
Write-Host "  Window size: ${width}x${height}"

if ($width -le 0 -or $height -le 0) {
    Write-Host "  ERROR: Invalid window size" -ForegroundColor Red
    Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
    exit 1
}

$bmp = New-Object System.Drawing.Bitmap $width, $height
$g = [System.Drawing.Graphics]::FromImage($bmp)
$hdc = $g.GetHdc()
# 0x2 = PW_RENDERFULLCONTENT (Win 8.1+, lets WebView2 render GPU-accelerated)
$ok = [Win32Screenshot]::PrintWindow($hwnd, $hdc, 0x2)
$g.ReleaseHdc($hdc)
$g.Dispose()

if (-not $ok) {
    Write-Host "  PW_RENDERFULLCONTENT failed, trying default flag..." -ForegroundColor Yellow
    $bmp.Dispose()
    $bmp = New-Object System.Drawing.Bitmap $width, $height
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $hdc = $g.GetHdc()
    $ok = [Win32Screenshot]::PrintWindow($hwnd, $hdc, 0)
    $g.ReleaseHdc($hdc)
    $g.Dispose()
}

# 5) Save
$outFile = Join-Path $OutFullDir "main.png"
$bmp.Save($outFile, [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
$fileSize = (Get-Item $outFile).Length
Write-Host "[5/5] Saved: $outFile" -ForegroundColor Green
Write-Host "  Size: $([Math]::Round($fileSize / 1KB, 1)) KB"

# Cleanup
Write-Host ""
Write-Host "Closing EXE..." -ForegroundColor Yellow
Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
Get-Process | Where-Object { $_.Name -like "*MarkdownEditor*" -or $_.Name -like "*msedgewebview2*" } | Stop-Process -Force -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "Done!" -ForegroundColor Green
Write-Host "Screenshot: $outFile"
Write-Host "Reference in README: ![Main UI](docs/screenshots/main.png)"
