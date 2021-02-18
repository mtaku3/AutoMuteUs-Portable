cd %~dp0bin\postgres\bin

initdb -D ../data -U postgres -A trust

pg_ctl -D ../data start

psql -U postgres -c "ALTER USER postgres WITH PASSWORD 'postgres';"

pg_ctl -D ../data stop