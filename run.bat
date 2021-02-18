If not exist bin\postgres\data call initdb.bat

cd %~dp0

start bin\postgres\bin\pg_ctl -D bin/postgres/data start

start bin\redis\redis-server.exe

start bin\galactus.exe

start bin\automuteus.exe

start bin\wingman.exe