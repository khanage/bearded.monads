@echo off

.\packages\Leeloo\tools\Fake.exe leeloo\build.fsx

REM This will leave the console opened if it was opened from windows
REM for %%x in (%cmdcmdline%) do if /i "%%~x"=="/c" set DOUBLECLICKED=1
REM if defined DOUBLECLICKED pause
