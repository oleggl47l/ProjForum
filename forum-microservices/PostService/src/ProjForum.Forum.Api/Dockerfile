FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY PostService/ForumService.sln ./

COPY PostService/src/ProjForum.Forum.Api/ProjForum.Forum.Api.csproj src/ProjForum.Forum.Api/
COPY PostService/src/ProjForum.Forum.Application/ProjForum.Forum.Application.csproj src/ProjForum.Forum.Application/
COPY PostService/src/ProjForum.Forum.Domain/ProjForum.Forum.Domain.csproj src/ProjForum.Forum.Domain/
COPY PostService/src/ProjForum.Forum.Infrastructure/ProjForum.Forum.Infrastructure.csproj src/ProjForum.Forum.Infrastructure/

COPY SharedModels/SharedModels/SharedModels.csproj ../SharedModels/SharedModels/
#COPY BuildingBlocks/ProjForum.BuildingBlocks/ProjForum.BuildingBlocks.csproj ../BuildingBlocks/ProjForum.BuildingBlocks/

RUN dotnet restore ForumService.sln

COPY . .

RUN dotnet publish PostService/src/ProjForum.Forum.Api/ProjForum.Forum.Api.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "ProjForum.Forum.Api.dll"]