FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish jjagentfoundry.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8088
ENV ASPNETCORE_URLS=http://+:8088

ENTRYPOINT ["dotnet", "jjagentfoundry.dll"]
