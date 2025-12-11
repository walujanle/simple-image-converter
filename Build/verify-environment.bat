@echo off
setlocal enabledelayedexpansion
echo ========================================
echo Build System - Verification Check
echo ========================================
echo.

cd /d "%~dp0.."

echo Checking build environment...
echo.

set "ERROR_FOUND=0"

REM Check .NET SDK
echo [1/6] Checking .NET SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo [FAIL] .NET SDK not found!
    echo        Please install .NET 10 SDK
    set "ERROR_FOUND=1"
) else (
    for /f "delims=" %%v in ('dotnet --version') do set "DOTNET_VER=%%v"
    echo [OK]   .NET SDK version: !DOTNET_VER!
)

REM Check MSBuild
echo [2/6] Checking MSBuild...
set "MSBUILD_FOUND=0"

REM Try vswhere
for /f "usebackq tokens=*" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe 2^>nul`) do (
    set "MSBUILD_PATH=%%i"
    set "MSBUILD_FOUND=1"
    goto :msbuild_found
)

:msbuild_found
if !MSBUILD_FOUND! equ 1 (
    echo [OK]   MSBuild found: !MSBUILD_PATH!
) else (
    echo [INFO] Native MSBuild not found, will use dotnet msbuild
)

REM Check project file
echo [3/6] Checking project file...
if exist "simple-image-converter\simple-image-converter.csproj" (
    echo [OK]   Project file found
) else (
    echo [FAIL] Project file not found!
    set "ERROR_FOUND=1"
)

REM Check PowerShell
echo [4/6] Checking PowerShell...
powershell -Command "Write-Host 'PowerShell OK'" >nul 2>&1
if errorlevel 1 (
    echo [FAIL] PowerShell not available!
    set "ERROR_FOUND=1"
) else (
    for /f "delims=" %%v in ('powershell -Command "$PSVersionTable.PSVersion.Major"') do set "PS_VER=%%v"
    echo [OK]   PowerShell version: !PS_VER!
)

REM Check signtool (optional)
echo [5/6] Checking signtool (optional for signing)...
set "SIGNTOOL_FOUND=0"

REM Search for signtool
for /f "delims=" %%i in ('dir /b /s "%ProgramFiles(x86)%\Windows Kits\10\bin\*\x64\signtool.exe" 2^>nul ^| sort /r') do (
    set "SIGNTOOL_PATH=%%i"
    set "SIGNTOOL_FOUND=1"
    goto :signtool_found
)

:signtool_found
if !SIGNTOOL_FOUND! equ 1 (
    echo [OK]   signtool found: !SIGNTOOL_PATH!
) else (
    echo [WARN] signtool not found in PATH
    echo        Required for code signing
    echo        Install Windows SDK if you need signing
    echo        Download: https://developer.microsoft.com/windows/downloads/windows-sdk/
)

REM Check build scripts
echo [6/6] Checking build scripts...
set "SCRIPTS_OK=1"

if not exist "%~dp0build-unsigned.bat" (
    echo [FAIL] build-unsigned.bat missing
    set "ERROR_FOUND=1"
    set "SCRIPTS_OK=0"
)

if not exist "%~dp0build-signed.bat" (
    echo [FAIL] build-signed.bat missing
    set "ERROR_FOUND=1"
    set "SCRIPTS_OK=0"
)

if not exist "%~dp0build-signed.ps1" (
    echo [FAIL] build-signed.ps1 missing
    set "ERROR_FOUND=1"
    set "SCRIPTS_OK=0"
)

if not exist "%~dp0create-cert.ps1" (
    echo [FAIL] create-cert.ps1 missing
    set "ERROR_FOUND=1"
    set "SCRIPTS_OK=0"
)

if !SCRIPTS_OK! equ 1 (
    echo [OK]   All build scripts found
)

echo.
echo ========================================

if !ERROR_FOUND! equ 1 (
    echo Status: FAILED - Please fix errors above
    echo ========================================
    pause
    exit /b 1
) else (
    echo Status: READY - All checks passed!
    echo ========================================
    echo.
    echo Build Tools Available:
    if !MSBUILD_FOUND! equ 1 (
        echo  - MSBuild: Native ^(Visual Studio^)
    ) else (
        echo  - MSBuild: dotnet msbuild ^(.NET SDK^)
    )
    echo  - .NET SDK: !DOTNET_VER!
    if !SIGNTOOL_FOUND! equ 1 (
        echo  - Code Signing: Available
    ) else (
        echo  - Code Signing: Not available ^(install Windows SDK^)
    )
    echo.
    echo You can now run builds:
    echo  - BUILD-MENU.bat ^(Recommended^)
    echo  - build-unsigned.bat
    if !SIGNTOOL_FOUND! equ 1 (
        echo  - build-signed.bat ^(with code signing^)
    ) else (
        echo  - build-signed.bat ^(without code signing^)
    )
    echo.
)

pause
