cd %~dp0

rmdir /s/q .build
mkdir .build

mkdir .build\bin

cd .build

call ../automuteus/build.bat

call ../galactus/build.bat

call ../wingman/build.bat

cd %~dp0

If not exist .build\postgres xcopy postgres .build\bin\postgres /s/e/i

If not exist .build\redis xcopy redis .build\bin\redis /s/e/i

copy initdb.bat .build\

copy run.bat .build\

copy .env.example .build\.env

pause