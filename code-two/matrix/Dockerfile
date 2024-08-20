FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy the *.csproj files.
COPY ./matrix .
#COPY ./poly .

# Copy the folders needed to run the application.
#COPY ./run/config /matrix/config/
#COPY ./run/logs /matrix/logs/

# Copy and publish the application and its libraries.
COPY . .
RUN dotnet restore ./matrix.csproj
RUN dotnet publish ./matrix.csproj -c debug -o /matrix --no-restore

# Final stage/image
FROM mcr.microsoft.com/dotnet/runtime:8.0
#WORKDIR /matrix
#COPY --from=build /matrix .
#VOLUME [ "/config" ]
#COPY ./config/Settings.xml -v /config/
ENV DOTNET_AttachStdout=1
ENV DOTNET DOTNET_CLI_TELEMETRY_OPTOUT=1
ENTRYPOINT ["dotnet", "matrix.dll"]
CMD ["-u"]