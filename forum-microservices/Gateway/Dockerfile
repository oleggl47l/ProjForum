FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Gateway.sln ./
COPY src/ProjForum.Gateway.Api/*.csproj src/ProjForum.Gateway.Api/

RUN dotnet restore Gateway.sln 

COPY . .

RUN dotnet publish  src/ProjForum.Gateway.Api/ProjForum.Gateway.Api.csproj -c Release -o /app/out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "ProjForum.Gateway.Api.dll"]
