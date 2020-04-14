FROM mcr.microsoft.com/dotnet/core/sdk:2.2.107 AS build-env
WORKDIR /app

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2.107
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "--version"]
