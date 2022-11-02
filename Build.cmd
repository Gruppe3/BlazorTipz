@echo off

::Docker killing containers
docker kill ndtipz
docker kill mariadb

::application BlazorTipz
::Build application Image specified in Dockerfile
docker image build -t ndtipz -f BlazorTipz/Dockerfile .

::Run application container
docker container run --rm -it -d --name ndtipz --publish 80:80 ndtipz


::MariaDB database
::Docker update mariadb libs
docker pull mariadb:10.5.11

::Launch mariadb database in Docker on port 3308:3306/tcp
docker container run --rm -it -d --name mariadb --publish 3308:3306/tcp -e MYSQL_ROOT_PASSWORD=ndtipz mariadb:10.5.11

::Docker copy BlazorTipzDDL.sql into mariadb container
docker cp BlazorTipzDDL.sql mariadb:/docker-entrypoint-initdb.d/BlazorTipzDDL.sql

echo.
echo "Link: http://localhost:80/"
echo.

pause