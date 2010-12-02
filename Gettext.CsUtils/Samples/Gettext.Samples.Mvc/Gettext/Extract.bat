@ECHO OFF

echo.
echo Setting up global variables...
SET path_xgettext=..\..\..\Bin\Gnu.Gettext.Win32\xgettext.exe
SET path_aspextract=..\..\..\Tools\Gettext.AspExtract\bin\Debug\AspExtract.exe
SET path_output=.\Templates
SET file_list=..\*.cs
SET asp_files_root=..

echo.
echo Generating strings po file...
CALL ..\..\..\Scripts\ExtractAspNetStrings.bat Strings