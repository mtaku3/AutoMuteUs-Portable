cd %~dp0

If not exist .build mkdir .build

cd .build

call ../automuteus/build.bat

call ../galactus/build.bat

call ../wingman/build.bat

cd %~dp0

If not exist .build\postgres xcopy postgres .build\postgres /s/e/i

If not exist .build\redis xcopy redis .build\redis /s/e/i

copy initdb.bat .build\

copy run.bat .build\

pause