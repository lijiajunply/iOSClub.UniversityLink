FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["iOSClub.UniversityLink/iOSClub.UniversityLink/iOSClub.UniversityLink.csproj", "iOSClub.UniversityLink/iOSClub.UniversityLink/"]
COPY ["iOSClub.UniversityLink/iOSClub.UniversityLink.Client/iOSClub.UniversityLink.Client.csproj", "iOSClub.UniversityLink/iOSClub.UniversityLink.Client/"]
COPY ["UniversityLink.DataModels/UniversityLink.DataModels.csproj", "UniversityLink.DataModels/"]
RUN dotnet restore "iOSClub.UniversityLink/iOSClub.UniversityLink/iOSClub.UniversityLink.csproj"
COPY . .
WORKDIR "/src/iOSClub.UniversityLink/iOSClub.UniversityLink"
RUN dotnet build "iOSClub.UniversityLink.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "iOSClub.UniversityLink.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "iOSClub.UniversityLink.dll"]
