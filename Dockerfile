FROM microsoft/dotnet:2.2.104-sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY ./src ./src
COPY ./tests ./tests
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "WebSite.dll"]