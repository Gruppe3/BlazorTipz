@echo off

::Docker killing container
docker kill ndtipz
::Build Image specified in Dockerfile
docker image build -t ndtipz .

::Run container
docker container run --rm -it -d --name ndtipz --publish 80:80 ndtipz

::Docker killing container
docker kill mariadb

::Launch mariadb database
docker run --rm --name mariadb -p 3308:3306/tcp -v "%cd%\database":/var/lib/mysql -e MYSQL_ROOT_PASSWORD=Test1234 -d mariadb:10.5.11

::Fill database with data
docker exec -i mariadb mysql -u root -p Test1234 < BlazorTipzDDL.sql

echo.
echo "Link: http://localhost:80/"
echo.

pause