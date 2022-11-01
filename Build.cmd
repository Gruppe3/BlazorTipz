@echo off

::Docker killing container
docker kill ndtipz
::Build Image specified in Dockerfile
docker image build -t ndtipz .

::Run container
docker container run --rm -it -d --name ndtipz --publish 80:80 ndtipz

::Docker killing container
docker kill mariadb

::Docker update mariadb
docker pull mariadb:10.5.11

::Launch mariadb database in Docker on port 3308:3306/tcp
docker container run --rm -it -d --name mariadb --publish 3308:3306/tcp -e MYSQL_ROOT_PASSWORD=ndtipz mariadb:10.5.11

::Docker copy BlazorTipzDDL.sql into mariadb container
docker cp BlazorTipzDDL.sql mariadb:/docker-entrypoint-initdb.d/BlazorTipzDDL.sql
::fetch and run the sql script to create the database
docker exec -i -u root mariadb mysql -h 127.0.0.1 -p 3308:3306 -pndtipz -f mariadb < BlazorTipzDDL.sql







echo.
echo "Link: http://localhost:80/"
echo.

pause