@echo off
echo ========================================
echo Simple Image Converter - Clean Build
echo ========================================
echo.

set /p CONFIRM="Are you sure you want to clean all build outputs? (Y/N): "
if /i not "%CONFIRM%"=="Y" (
    echo Cancelled.
    pause
    exit /b 0
)

cd /d "%~dp0.."

echo Cleaning dotnet build cache...
dotnet clean --configuration Release --verbosity quiet
dotnet clean --configuration Debug --verbosity quiet

echo Cleaning Build output directories...
if exist "%~dp0Output" (
    rd /s /q "%~dp0Output"
    echo - Deleted: Build\Output\
)

if exist "%~dp0Temp" (
    rd /s /q "%~dp0Temp"
    echo - Deleted: Build\Temp\
)

if exist "simple-image-converter\bin" (
    rd /s /q "simple-image-converter\bin"
    echo - Deleted: simple-image-converter\bin\
)

if exist "simple-image-converter\obj" (
    rd /s /q "simple-image-converter\obj"
    echo - Deleted: simple-image-converter\obj\
)

echo.
echo ========================================
echo Clean completed successfully!
echo ========================================
pause
