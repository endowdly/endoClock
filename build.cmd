@ECHO OFF

REM USAGE
REM At the 'solution' level this file needs a target.build file which just enumerates project folders on a newline.
REM At the 'project' level, this file needs a response file and a copy of itself in the project to call.
REM If this is just used to build one project, only needs the response file.

REM Get the 'project' Name
FOR %%I in (.) DO SET currentDirName=%%~nxI

IF /I "%~1"=="build" GOTO build

TITLE %currentDirName% Build
MODE 80,20
COLOR 87

ECHO SETTING toolpath
SET framework=C:\Windows\Microsoft.NET\Framework64\
IF NOT EXIST %framework% GOTO fail

REM This for loop should get the highest framework available.
FOR /F %%G in ('DIR /B /AD /ON %framework%') DO SET version=%%G
SET toolPath="%framework%%version%"
ECHO TOOLPATH %toolPath%

IF EXIST %currentDirName%.build (
  ECHO USING %currentDirName%.build
  FOR /F "delims=" %%T in (%currentDirName%.build) DO CALL :process %%T
  GOTO die)

:build
  SETLOCAL
  ECHO BUILDING %currentDirName%
  REM Get and set the response file.
  IF EXIST *.rsp (
    FOR /F %%G in ('DIR /B *.rsp') DO SET rf=%%G
  ) ELSE (
    ECHO FAIL no response file
    GOTO fail
  )

:clean 
  FOR %%G in (out, doc, .\res\*.resource) DO CALL :remove %%G

  IF ERRORLEVEL 1 GOTO fail
  ECHO USING %rf%

  REM Build Resource File first.
  IF EXIST res\*.cs (
    FOR %%G in (.\res\*.cs) DO ECHO COMPILING %%~nxG
    %toolPath%\csc.exe /t:library /out:.\res\%CurrentDirName%.resource /nologo .\res\*.cs >>doc\build.log 2>>doc\error.log) 
  
  REM List all source files.
  IF EXIST src (FOR %%G in (.\src\*.cs) DO ECHO COMPILING %%~nxG)
  IF EXIST *.cs (
    FOR %%G in (*.cs) DO ECHO COMPILING %%~nxG
  ) ELSE (
    ECHO DONE nothing to compile
    GOTO die
  )

  REM Make sure out dir exists.
  IF NOT EXIST .\out MD out
  IF NOT EXIST .\doc MD doc

  REM make the call to CSC
  %toolPath%\csc.exe @%rf% /nologo >>doc\build.log 2>>doc\error.log
  ENDLOCAL

  IF ERRORLEVEL 1 (GOTO fail) ELSE (ECHO BUILT)

:die
  GOTO :end

:fail 
  ECHO FAIL
  PAUSE >nul
  EXIT /B 1

:process
  ECHO %1
  PUSHD %1
  CALL build.cmd build
  POPD

:end
  PAUSE >nul
  EXIT /B 0

:remove
  IF EXIST %1 FOR %%G IN (.\%1\*) DO ECHO CLEANING %%~nxG & DEL /Q /F %1\*