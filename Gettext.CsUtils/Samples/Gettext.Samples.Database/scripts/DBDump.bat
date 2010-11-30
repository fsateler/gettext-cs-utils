@ECHO OFF

SET dbrsgen=..\..\..\Tools\Gettext.DatabaseResourceGenerator\bin\Debug\DatabaseResourceGenerator.exe

echo.
echo Dumping culture sets into DB...

echo Culture es
CALL %dbrsgen% -i ..\Translated\es\Strings.po -c es -a

echo Culture en
CALL %dbrsgen% -i ..\Translated\en\Strings.po -c en -a

echo Culture pt
CALL %dbrsgen% -i ..\Translated\pt\Strings.po -c pt -a

echo Culture fr
CALL %dbrsgen% -i ..\Translated\fr\Strings.po -c fr -a

pause