@ECHO OFF

echo.
echo Setting up global variables...
SET path_xgettext=..\..\..\Bin\Gnu.Gettext.Win32\xgettext.exe
SET path_source=..\..
SET path_output=..\Templates

echo.
echo Generating strings po file...
SET file_list=%path_source%\*.cs
CALL ..\..\..\Scripts\ExtractStrings.bat Strings