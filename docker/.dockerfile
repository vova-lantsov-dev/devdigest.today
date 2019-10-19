FROM mcr.microsoft.com/dotnet/core/aspnet:3.0

WORKDIR /
COPY ./out/ ./

ENTRYPOINT ["dotnet", "WebSite.dll"]