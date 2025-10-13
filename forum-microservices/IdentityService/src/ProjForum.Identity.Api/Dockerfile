FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ProjForum.Identity.Api/ProjForum.Identity.Api.csproj", "src/ProjForum.Identity.Api/"]
COPY ["src/ProjForum.Identity.Infrastructure/ProjForum.Identity.Infrastructure.csproj", "src/ProjForum.Identity.Infrastructure/"]
COPY ["src/ProjForum.Identity.Domain/ProjForum.Identity.Domain.csproj", "src/ProjForum.Identity.Domain/"]
COPY ["src/ProjForum.Identity.Application/ProjForum.Identity.Application.csproj", "src/ProjForum.Identity.Application/"]
RUN dotnet restore "src/ProjForum.Identity.Api/ProjForum.Identity.Api.csproj"
COPY . .
WORKDIR "/src/src/ProjForum.Identity.Api"
RUN dotnet build "ProjForum.Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProjForum.Identity.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjForum.Identity.Api.dll"]
