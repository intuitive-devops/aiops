FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the *.csproj files.
COPY ./skan .
COPY ./run .

# Copy the folders needed to run the application.
#COPY ./run/config /bph/config/
#COPY ./run/logs /bph/logs/

# Copy and publish the application and its libraries.
COPY . .
RUN dotnet restore ./run/run.csproj
RUN dotnet publish ./run/run.csproj -c debug -o /skan --no-restore

# Final stage/image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /skan
COPY --from=build /skan .
#VOLUME [ "/config" ]
#COPY ./config/Settings.xml -v /config/
ENV DOTNET_AttachStdout=1
ENV DOTNET DOTNET_CLI_TELEMETRY_OPTOUT=1
ENTRYPOINT ["dotnet", "run.dll"]
CMD ["-u"]