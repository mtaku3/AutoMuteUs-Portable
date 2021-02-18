cd %~dp0postgres\bin

initdb -D ../data -U postgres -A trust

pg_ctl -D ../data start

psql -U postgres

ALTER USER postgres WITH PASSWORD 'postgres';

\q