@echo off
echo ========================================
echo Simple Image Converter - Unsigned Build
echo ========================================
echo.

REM Navigate to project directory
cd /d "%~dp0.."

set "PROJECT_FILE=simple-image-converter\simple-image-converter.csproj"
set "BUILD_OUTPUT=%~dp0Output\Unsigned"

REM Step 1: Find MSBuild
echo [1/4] Locating build tools...
set "MSBUILD="

REM Try to find MSBuild from Visual Studio installations
for /f "usebackq tokens=*" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe 2^>nul`) do (
    set "MSBUILD=%%i"
)

REM If not found, use dotnet msbuild
if "%MSBUILD%"=="" (
    where dotnet >nul 2>&1
    if errorlevel 1 (
        echo ERROR: Neither MSBuild nor .NET SDK found!
        echo Please install Visual Studio or .NET SDK.
        pause
        exit /b 1
    )
    echo Using dotnet msbuild...
    set "MSBUILD=dotnet msbuild"
) else (
    echo Found MSBuild: !MSBUILD!
)

REM Step 2: Clean previous builds
echo [2/4] Cleaning previous builds...
if exist "%BUILD_OUTPUT%" rd /s /q "%BUILD_OUTPUT%"
if exist "simple-image-converter\bin" rd /s /q "simple-image-converter\bin"
if exist "simple-image-converter\obj" rd /s /q "simple-image-converter\obj"
echo Clean completed.

REM Step 3: Restore dependencies
echo [3/4] Restoring NuGet packages...
echo %MSBUILD% | findstr /C:"dotnet msbuild" >nul
if errorlevel 1 (
    REM Using native MSBuild
    "%MSBUILD%" "%PROJECT_FILE%" /t:Restore /p:Configuration=Release /p:RuntimeIdentifier=win-x64 /verbosity:minimal
) else (
    REM Using dotnet
    dotnet restore "%PROJECT_FILE%" --runtime win-x64 --verbosity quiet
)
if errorlevel 1 (
    echo ERROR: Restore failed!
    pause
    exit /b 1
)

REM Step 4: Build the project
echo [4/4] Building project (Release - Unsigned)...

REM Detect if using dotnet msbuild or native msbuild
echo %MSBUILD% | findstr /C:"dotnet msbuild" >nul
if errorlevel 1 (
    REM Using native MSBuild
    "%MSBUILD%" "%PROJECT_FILE%" /verbosity:minimal /t:Publish /p:Configuration=Release /p:RuntimeIdentifier=win-x64 /p:SelfContained=true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:EnableCompressionInSingleFile=true /p:DebugType=None /p:DebugSymbols=false /p:PublishReadyToRun=false /p:PublishDir="%BUILD_OUTPUT%"
) else (
    REM Using dotnet msbuild
    dotnet publish "%PROJECT_FILE%" /verbosity:minimal /p:Configuration=Release /p:RuntimeIdentifier=win-x64 /p:SelfContained=true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:EnableCompressionInSingleFile=true /p:DebugType=None /p:DebugSymbols=false /p:PublishReadyToRun=false /p:PublishDir="%BUILD_OUTPUT%"
)

if errorlevel 1 (
    echo ERROR: Build failed!
    pause
    exit /b 1
)

REM Copy license and readme if exists
if exist "LICENSE" copy /y "LICENSE" "%BUILD_OUTPUT%\" >nul
if exist "README.md" copy /y "README.md" "%BUILD_OUTPUT%\" >nul

echo.
echo ========================================
echo Build completed successfully!
echo Output: Build\Output\Unsigned\
echo ========================================
echo.

explorer "%BUILD_OUTPUT%"
pause
