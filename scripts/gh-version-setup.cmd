@echo off
SETLOCAL ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION

SET CACHE=%0\..\gh.cache.bat
SET RSPFILE=%0\..\msbuild-version.rsp

echo @echo off > %CACHE%

pushd %0\..
FOR /F "usebackq" %%i in (`git rev-parse HEAD`) do (
  SET GIT_SHA=%%i
)
popd

set    ANKHSVN_MAJOR=2
set    ANKHSVN_MINOR=8
set    ANKHSVN_PATCH=%1

echo Prepare building AnkhSVN %ANKHSVN_MAJOR%.%ANKHSVN_MINOR%.%ANKHSVN_PATCH%

(
  echo SET ANKHSVN_MAJOR=%ANKHSVN_MAJOR%
  echo SET ANKHSVN_MINOR=%ANKHSVN_MINOR%
  echo SET ANKHSVN_PATCH=%ANKHSVN_PATCH%
  echo SET GIT_SHA=%GIT_SHA%
) >> %CACHE%

(
  echo /p:ForceAssemblyVersion=%ANKHSVN_MAJOR%.%ANKHSVN_MINOR%.%ANKHSVN_PATCH%
  echo /p:ForceAssemblyCompany="AnkhSVN Project, powered by AmpScm, QQn & GitHub"
  echo /p:ForceAssemblyCopyright="Apache 2.0 licensed. See https://github.com/ampscm/AnkhSVN"
  REM echo /p:ForceAssemblyInformationalVersion=%ANKHSVN_MAJOR%.%ANKHSVN_MINOR%.%ANKHSVN_PATCH%-%GIT_SHA%
  echo /p:BuildBotBuild=true
  echo /p:RestoreForce=true
) > %RSPFILE%
