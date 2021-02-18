If not exist postgres\data call initdb.bat

cd %~dp0

start .\postgres\bin\pg_ctl -D ../data start

start .\redis\redis-server.exe

start .\galactus.exe

start .\automuteus.exe

start .\wingman.exe