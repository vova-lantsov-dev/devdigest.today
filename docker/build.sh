#!/bin/bash

bold=$(tput bold)
normal=$(tput sgr0)
project="devdigest.today"

clear

now=`date +%Y%m%d%H%M%S`
tag=latest

echo "Build //devdigest Websit Docker image"
cd ../src/WebSite

dotnet publish -c Release -o ../../docker/out

cd ../../docker

docker build -t $project:$tag -f ./.dockerfile .
docker build -t $project:$now -f ./.dockerfile .

rm -rf out

echo "Containers built:"
echo "   ${bold}$project:$tag${normal}"
echo "   ${bold}$project:$now${normal}"

echo ""
echo ""

echo "To test it use command:"
echo "   ${bold}docker run --rm -it -p 8000:80 $project:$tag${normal}"
echo "   ${bold}docker run --rm -it -p 8000:80 $project:$now${normal}"

echo ""
echo ""

echo "To push to Docker Hub:"
echo "   ${bold}docker tag $project:$now docker.pkg.github.com/dncuug/devdigest.today/$project:$now${normal}"
echo "   ${bold}docker push docker.pkg.github.com/dncuug/devdigest.today/$project:%$now${normal}"

echo ""
echo ""

echo "   ${bold}docker tag $project:latest docker.pkg.github.com/dncuug/devdigest.today/$project:latest${normal}"
echo "   ${bold}docker push docker.pkg.github.com/dncuug/devdigest.today/$project:latest${normal}"

echo ""
echo ""

echo $now > "../src/Site/wwwroot/build.version"



