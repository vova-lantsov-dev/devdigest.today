## Developers Digest Website

<a href="https://github.com/dncuug/devdigest.today/graphs/contributors" alt="Contributors">
  <img src="https://img.shields.io/github/contributors/dncuug/devdigest.today" />
</a>
<a href="https://twitter.com/intent/follow?screen_name=devdigest_today">
  <img src="https://img.shields.io/twitter/follow/devdigest_today?style=social&logo=twitter" alt="follow on Twitter">
</a>

Welcome to Developers Digest <a href="https://devdigest.today">website</a>!

Project supported by 
<a href="https://www.facebook.com/dncuug/">Ukrainian .NET Developer Community</a>,  <a href="https://www.facebook.com/groups/azure.ua/">Microsoft Azure Ukraine User Group</a>, and <a href="https://www.facebook.com/groups/xamarin.ua">Xamarin Ukraine User Group</a>.


## How to build docker image

```
docker build ./ --file ./Dockerfile --tag ghcr.io/dncuug/devdigest.today/devdigest.today:latest

docker push ghcr.io/dncuug/devdigest.today/devdigest.today:latest

docker run --rm -it -e CognitiveServicesTextAnalyticsKey=xxx -e WebSiteUrl=http://localhost:8000/ -e ConnectionStrings__DefaultConnection="xxx" -p 8000:80 ghcr.io/dncuug/devdigest.today/devdigest.today:latest 
```

### How to login into GitHub container registry
```
docker login ghcr.io
```