@echo off

::Docker killing container
docker kill ndtipz
::Build Image specified in Dockerfile
docker image build -t ndtipz .

::Run container
docker container run --rm -it -d --name ndtipz --publish 80:80 ndtipz
