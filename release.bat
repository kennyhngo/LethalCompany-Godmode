@echo off

set zipfilename=godmode.zip
set srcdir=%cd%

"C:\Program Files\WinRAR\winrar.exe" a -ep1 "%zipfilename%" ^
    "%srcdir%\icon.png" ^
    "%srcdir%\README.md" ^
    "%srcdir%\CHANGELOG.md" ^
    "%srcdir%\manifest.json" ^
    "%srcdir%\bin\debug\Godmode.dll"

echo Files compressed to %zipfilename%
pause
