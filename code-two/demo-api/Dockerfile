FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY code-two/demo-api/demo-api.csproj code-two/demo-api/
RUN dotnet restore code-two/demo-api/demo-api.csproj

COPY . .
RUN dotnet publish code-two/demo-api/demo-api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8787
ENV AIOPS_API_URLS=http://0.0.0.0:8787
ENV DOTNET_USE_POLLING_FILE_WATCHER=1

EXPOSE 8787
ENTRYPOINT ["dotnet", "demo-api.dll"]
