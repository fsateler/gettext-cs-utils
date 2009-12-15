@echo off

if "%1" == "" goto NONAME
if "%file_list%" == "" goto NOFILELIST
if "%path_xgettext%" == "" goto NOXGETTEXT
if "%path_output%" == "" goto NOOUTPUT
if "%asp_files_root%" == "" goto NOASPFILES
if "%path_aspextract%" == "" goto NOASPEXTRACT

SET class_name=%2
if "%class_name%" == "" SET class_name=%1

echo.
echo Extracting all strings from asp %asp_files_root% files in t:t tags into .postrings files...
%path_aspextract% -tt:t -k%2.T -epostrings -p%asp_files_root%

echo.
echo Creating files lists to be retrieved by gettext...
dir %file_list% /S /B > %1.gettext.fileslist

echo.
echo Creating %1 po file from all %2 strings...
%path_xgettext% -k -k%2.T -k%2.M --from-code=UTF-8 -LC# --omit-header -o%path_output%\%1.po -f%1.gettext.fileslist

echo.
echo Removing all temporary files...
del /Q *.po
del /Q %asp_files_root%\*.postrings
del /Q *.gettext.fileslist

echo.
echo Finished
goto END


:NONAME
echo.
echo Must specify as first parameter the name of the resource.
goto END

:NOFILELIST
echo.
echo Must specify file_list environment variable.
goto END

:NOXGETTEXT
echo.
echo Must specify path_xgettext environment variable.
goto END

:NOOUTPUT
echo.
echo Must specify path_output environment variable.
goto END

:NOASPFILES
echo.
echo Must specify asp_files_root environment variable.
goto END

:NOASPEXTRACT
echo.
echo Must specify path_aspextract environment variable.
goto END

:END

