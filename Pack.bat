@ECHO OFF

echo.
echo Copying all files to package to local gettext-cs-utils folder

mkdir .\gettext-cs-utils\Gnu.Gettext.Win32\
copy .\Gettext.CsUtils\Bin\Gnu.Gettext.Win32\*.dll .\gettext-cs-utils\Gnu.Gettext.Win32\
copy .\Gettext.CsUtils\Bin\Gnu.Gettext.Win32\*.exe .\gettext-cs-utils\Gnu.Gettext.Win32\

mkdir .\gettext-cs-utils\Scripts
copy .\Gettext.CsUtils\Scripts\*.bat .\gettext-cs-utils\Scripts\

mkdir .\gettext-cs-utils\Binaries\
copy .\Gettext.CsUtils\Core\Gettext.Cs\bin\Release\*.dll .\gettext-cs-utils\Binaries\
copy .\Gettext.CsUtils\Core\Gettext.Cs.Web\bin\Release\*.dll .\gettext-cs-utils\Binaries\

mkdir .\gettext-cs-utils\Templates\
copy .\Gettext.CsUtils\Core\Gettext.Cs\Templates\*.tt .\gettext-cs-utils\Templates\

mkdir .\gettext-cs-utils\Tools\
copy .\Gettext.CsUtils\Tools\Gettext.AspExtract\bin\Release\*.exe .\gettext-cs-utils\Tools\
copy .\Gettext.CsUtils\Tools\Gettext.DatabaseResourceGenerator\bin\Release\*.exe .\gettext-cs-utils\Tools\
copy .\Gettext.CsUtils\Tools\Gettext.Iconv\bin\Release\*.exe .\gettext-cs-utils\Tools\
copy .\Gettext.CsUtils\Tools\Gettext.Msgfmt\bin\Release\*.exe .\gettext-cs-utils\Tools\
copy .\Gettext.CsUtils\Tools\Gettext.ResourcesReplacer\bin\Release\*.exe .\gettext-cs-utils\Tools\

pause