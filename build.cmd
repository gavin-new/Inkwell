@echo off
REM MarkdownEditor 一键构建脚本
REM 要求: .NET 10 SDK
REM 产物: src\bin\Release\net10.0-windows\win-x64\publish\MarkdownEditor.exe

setlocal
cd /d "%~dp0src"

echo ===========================================
echo  Building MarkdownEditor (Release)
echo ===========================================

dotnet publish -c Release -r win-x64 --nologo

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo  BUILD FAILED
    exit /b 1
)

echo.
echo ===========================================
echo  Build succeeded!
echo  Output: src\bin\Release\net10.0-windows\win-x64\publish\MarkdownEditor.exe
echo ===========================================

REM 复制最新 EXE 到 dist
copy /Y "bin\Release\net10.0-windows\win-x64\publish\MarkdownEditor.exe" "..\dist\MarkdownEditor.exe" >nul
echo  Copied to dist\MarkdownEditor.exe

endlocal
