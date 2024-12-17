FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy the *.csproj files.
COPY ./agent .
COPY ./bph .
COPY ./matrix .
COPY ./poly .
COPY ./run .

# Copy the folders needed to run the application.
COPY ./run/config /bph/config/
COPY ./run/logs /bph/logs/

# Copy and publish the application and its libraries.
COPY . .
RUN dotnet restore ./run/run.csproj
RUN dotnet publish ./run/run.csproj -c debug -o /bph --no-restore

# Final stage/image
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /bph
COPY --from=build /bph .
VOLUME [ "/config" ]
COPY ./config/Settings.xml -v /config/
ENV DOTNET_AttachStdout=1
ENV DOTNET DOTNET_CLI_TELEMETRY_OPTOUT=1
ENTRYPOINT ["dotnet", "run.dll"]
CMD ["-u"]