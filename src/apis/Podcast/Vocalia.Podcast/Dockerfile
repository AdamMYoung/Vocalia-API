FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY src/apis/Podcast/Vocalia.Podcast/Vocalia.Podcast.csproj src/apis/Podcast/Vocalia.Podcast/
COPY src/apis/Podcast/Vocalia.Podcast.Db/Vocalia.Podcast.Db.csproj src/apis/Podcast/Vocalia.Podcast.Db/
COPY src/apis/Podcast/Facades/Vocalia.Facades.GPodder/Vocalia.Facades.GPodder.csproj src/apis/Podcast/Facades/Vocalia.Facades.GPodder/
COPY src/apis/Podcast/Facades/Vocalia.Facades.iTunes/Vocalia.Facades.iTunes.csproj src/apis/Podcast/Facades/Vocalia.Facades.iTunes/
RUN dotnet restore src/apis/Podcast/Vocalia.Podcast/Vocalia.Podcast.csproj
COPY . .
WORKDIR /src/src/apis/Podcast/Vocalia.Podcast
RUN dotnet build Vocalia.Podcast.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Vocalia.Podcast.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Vocalia.Podcast.dll"]
