@echo off
chcp 65001 >nul
setlocal EnableExtensions

rem Always work from the project directory. This keeps every path passed to MSBuild relative.
pushd "%~dp0"
if errorlevel 1 (
  echo Не удалось открыть каталог проекта.
  pause
  exit /b 1
)

where dotnet >nul 2>nul
if errorlevel 1 goto :no_dotnet

set "PROJECT=GeniaClipboard.csproj"
set "PUBLISH=bin\Release\net8.0-windows\win-x64\publish"
set "OUTPUT=dist\GeniaClipboard-win-x64"
set "ARCHIVE=dist\GeniaClipboard-win-x64.zip"
set "CHECKSUM=dist\SHA256SUMS.txt"

if exist "%OUTPUT%" rmdir /s /q "%OUTPUT%"
if exist "%ARCHIVE%" del /q "%ARCHIVE%"
if exist "%CHECKSUM%" del /q "%CHECKSUM%"

rem IMPORTANT: do not use dotnet publish -o/--output here.
rem dotnet can normalize -o to an absolute MSBuild property, which is fragile when the
rem project directory contains spaces. Publish to the SDK's standard relative folder instead.
dotnet publish "%PROJECT%" -c Release
if errorlevel 1 goto :build_failed

if not exist "%PUBLISH%\GeniaClipboard.exe" goto :publish_missing

mkdir "%OUTPUT%" >nul 2>nul
xcopy "%PUBLISH%\*" "%OUTPUT%\" /E /I /Y /Q >nul
if errorlevel 1 goto :copy_failed

copy /y "README.md" "%OUTPUT%\README.md" >nul
if errorlevel 1 goto :copy_failed
copy /y "README_RU.md" "%OUTPUT%\README_RU.md" >nul
if errorlevel 1 goto :copy_failed
copy /y "LICENSE" "%OUTPUT%\LICENSE" >nul
if errorlevel 1 goto :copy_failed

powershell -NoProfile -ExecutionPolicy Bypass -Command "Compress-Archive -Path '%OUTPUT%\*' -DestinationPath '%ARCHIVE%' -Force"
if errorlevel 1 goto :zip_failed

powershell -NoProfile -ExecutionPolicy Bypass -Command "$h=(Get-FileHash '%ARCHIVE%' -Algorithm SHA256).Hash.ToLowerInvariant(); Set-Content -Path '%CHECKSUM%' -Value ($h + '  GeniaClipboard-win-x64.zip') -Encoding Ascii"
if errorlevel 1 goto :checksum_failed

echo.
echo Готово: %CD%\%ARCHIVE%
echo SHA-256: %CD%\%CHECKSUM%
set "EXITCODE=0"
goto :finish

:no_dotnet
echo Не найден .NET SDK. Установите .NET 8 SDK или откройте проект в Visual Studio.
set "EXITCODE=1"
goto :finish

:build_failed
echo.
echo Сборка завершилась с ошибкой.
set "EXITCODE=1"
goto :finish

:publish_missing
echo.
echo Сборка завершилась, но GeniaClipboard.exe не найден в:
echo %CD%\%PUBLISH%
echo Проверьте TargetFramework и RuntimeIdentifier в GeniaClipboard.csproj.
set "EXITCODE=1"
goto :finish

:copy_failed
echo.
echo Сборка завершилась, но не удалось подготовить папку dist.
set "EXITCODE=1"
goto :finish

:zip_failed
echo.
echo EXE собран, но ZIP создать не удалось: %CD%\%OUTPUT%
set "EXITCODE=1"
goto :finish

:checksum_failed
echo.
echo ZIP создан, но SHA-256 checksum создать не удалось.
set "EXITCODE=1"

:finish
popd
pause
exit /b %EXITCODE%
