FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY IdentityService/IdentityService.sln ./

COPY IdentityService/src/ProjForum.Identity.Api/ProjForum.Identity.Api.csproj src/ProjForum.Identity.Api/
COPY IdentityService/src/ProjForum.Identity.Application/ProjForum.Identity.Application.csproj src/ProjForum.Identity.Application/
COPY IdentityService/src/ProjForum.Identity.Domain/ProjForum.Identity.Domain.csproj src/ProjForum.Identity.Domain/
COPY IdentityService/src/ProjForum.Identity.Infrastructure/ProjForum.Identity.Infrastructure.csproj src/ProjForum.Identity.Infrastructure/

COPY SharedModels/SharedModels/SharedModels.csproj ../SharedModels/SharedModels/
COPY BuildingBlocks/ProjForum.BuildingBlocks/ProjForum.BuildingBlocks.csproj ../BuildingBlocks/ProjForum.BuildingBlocks/

RUN dotnet restore IdentityService.sln

COPY . .

RUN dotnet publish IdentityService/src/ProjForum.Identity.Api/ProjForum.Identity.Api.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "ProjForum.Identity.Api.dll"]