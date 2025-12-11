@echo off
setlocal enabledelayedexpansion

REM Simple wrapper to call PowerShell signed build script
REM This provides backward compatibility for users who prefer batch files

echo ========================================
echo Simple Image Converter - Signed Build
echo ========================================
echo.

REM Check for administrator privileges and request elevation if needed
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo Requesting administrator privileges...
    echo.
    powershell -Command "Start-Process powershell.exe -ArgumentList '-NoProfile -ExecutionPolicy Bypass -File \"%~dp0build-signed.ps1\"' -Verb RunAs"
    exit /b
)

REM Call PowerShell script directly
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0build-signed.ps1"

if errorlevel 1 (
    echo.
    echo Build script failed!
    pause
    exit /b 1
)
