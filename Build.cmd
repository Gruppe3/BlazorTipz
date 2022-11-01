@echo off

::Docker killing container
docker kill NDtipz
::Build Image specified in Dockerfile
docker image build -t NDtipz .

::Run container
docker container run --rm -it -d --name NDtipz --publish 80:80 NDtipz
