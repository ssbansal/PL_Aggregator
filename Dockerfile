FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
COPY --from=build-env /app/out /app
WORKDIR /app
ENTRYPOINT ["dotnet", "--version"]
