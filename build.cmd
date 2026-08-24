@echo off
REM Inkwell 一键构建脚本
REM 要求: .NET 10 SDK
REM 产物: src\bin\Release\net10.0-windows\win-x64\publish\Inkwell.exe

setlocal
cd /d "%~dp0src"

echo ===========================================
echo  Building Inkwell (Release)
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
echo  Output: src\bin\Release\net10.0-windows\win-x64\publish\Inkwell.exe
echo ===========================================

REM 复制最新 EXE 到 dist
copy /Y "bin\Release\net10.0-windows\win-x64\publish\Inkwell.exe" "..\dist\Inkwell.exe" >nul
echo  Copied to dist\Inkwell.exe

endlocal
