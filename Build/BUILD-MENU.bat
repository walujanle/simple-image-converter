@echo off
color 0A
title Simple Image Converter - Build Menu

:menu
cls
echo.
echo  ================================================
echo     Simple Image Converter - Build Manager
echo  ================================================
echo.
echo  Select build option:
echo.
echo  [1] Build Unsigned (Standard Build)
echo  [2] Build Signed (Self-Signed Certificate)
echo  [3] Clean All Build Outputs
echo  [4] Open Build Output Folder
echo  [5] View Build Documentation
echo  [0] Exit
echo.
echo  ================================================
echo.

set /p choice="Enter your choice (0-5): "

if "%choice%"=="1" goto unsigned
if "%choice%"=="2" goto signed
if "%choice%"=="3" goto clean
if "%choice%"=="4" goto open_output
if "%choice%"=="5" goto docs
if "%choice%"=="0" goto exit

echo Invalid choice! Please try again.
timeout /t 2 >nul
goto menu

:unsigned
cls
echo Starting Unsigned Build...
call "%~dp0build-unsigned.bat"
pause
goto menu

:signed
cls
echo Starting Signed Build...
call "%~dp0build-signed.bat"
pause
goto menu

:clean
cls
call "%~dp0clean-all.bat"
goto menu

:open_output
cls
if exist "%~dp0Output" (
    explorer "%~dp0Output"
    echo Output folder opened.
) else (
    echo Output folder does not exist yet. Build first!
)
timeout /t 2 >nul
goto menu

:docs
cls
if exist "%~dp0README.md" (
    notepad "%~dp0README.md"
) else (
    echo README.md not found!
    timeout /t 2 >nul
)
goto menu

:exit
cls
echo.
echo Thank you for using Simple Image Converter Build Manager!
echo.
timeout /t 1 >nul
exit
