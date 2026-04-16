# Build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy files from the current directory on the host to the working directory in the container
COPY . .

# Restore packages
RUN dotnet restore ./jjagent.csproj
RUN dotnet publish ./jjagent.csproj -c Release --no-restore -o /app -p:AssemblyName=app

# Run the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app

# Copy everything needed to run the app from the "build" stage.
COPY --from=build /app .

EXPOSE 8088
ENTRYPOINT ["dotnet", "app.dll"]
