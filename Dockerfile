FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.2-aspnet-runtime
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "--version"]
