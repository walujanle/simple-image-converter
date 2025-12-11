@echo off
echo Testing MSBuild command syntax...
echo.

set "BUILD_OUTPUT=Build\Output\Signed"
set "PROJECT_FILE=simple-image-converter\simple-image-converter.csproj"

echo Command that will be executed:
echo.
echo dotnet publish "%PROJECT_FILE%" /p:Configuration=Release /p:RuntimeIdentifier=win-x64 /p:SelfContained=true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:EnableCompressionInSingleFile=true /p:DebugType=None /p:DebugSymbols=false /p:PublishDir="%BUILD_OUTPUT%\" /p:PublishReadyToRun=false /verbosity:minimal
echo.
echo.
echo Press any key to run actual build test...
pause >nul

REM Clean first
if exist "%BUILD_OUTPUT%" rd /s /q "%BUILD_OUTPUT%"
if exist "simple-image-converter\bin" rd /s /q "simple-image-converter\bin"
if exist "simple-image-converter\obj" rd /s /q "simple-image-converter\obj"

echo.
echo Running build...
echo.

dotnet publish "%PROJECT_FILE%" /verbosity:minimal /p:Configuration=Release /p:RuntimeIdentifier=win-x64 /p:SelfContained=true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:EnableCompressionInSingleFile=true /p:DebugType=None /p:DebugSymbols=false /p:PublishReadyToRun=false /p:PublishDir="%BUILD_OUTPUT%\"

if errorlevel 1 (
    echo.
    echo BUILD FAILED!
    echo.
) else (
    echo.
    echo BUILD SUCCESS!
    echo.
    if exist "%BUILD_OUTPUT%\simple-image-converter.exe" (
        echo Executable created: %BUILD_OUTPUT%\simple-image-converter.exe
    )
)

echo.
pause
