@ECHO OFF

TITLE Install 
MODE 60,7
COLOR 87

DIR /B /A-D out >nul 2>nul && GOTO ex || GOTO skip

:ex
  PUSHD out
  FOR /F %%G in ('DIR /B /A-D') DO CALL :move %%G
  GOTO die

:skip
  ECHO DONE nothing installed
  GOTO die

:die
  POPD
  PAUSE >nul
  EXIT /B 0

:move
  SET from=%~f1
  SET to=%USERPROFILE%\Desktop\%~n1.exe
  ECHO MOVING %~nx1
  ECHO  FROM %from%
  ECHO  TO %to%
  MOVE /Y %from% %to% >nul 2>nul
  IF ERRORLEVEL 1 (ECHO FAILED) ELSE (ECHO DONE installing)