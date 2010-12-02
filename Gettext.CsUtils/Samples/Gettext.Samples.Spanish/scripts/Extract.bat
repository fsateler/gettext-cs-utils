@ECHO OFF

echo.
echo Setting up global variables...
SET path_xgettext=..\..\..\Bin\Gnu.Gettext.Win32\xgettext.exe
SET path_output=..\Templates
SET file_list=.\..\*.cs

echo.
echo Generating strings po file...
CALL ..\..\..\Scripts\ExtractStrings.bat Strings