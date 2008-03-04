@echo off
set basedir=N:\tools
mkdir build 2> NUL
booc -t:library -r:System.Xml -o:%basedir%\build\vsconvert.dll %basedir%\vsconvert.boo  
booc -t:exe -r:%basedir%\build\vsconvert.dll -o:%basedir%\build\vsconverthook.exe %basedir%\vsconverthook.boo

if ERRORLEVEL 0 goto copy
goto end
:copy
xcopy /Y %basedir%\build\* Z:\svn\hooks
:end