FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/apis/Editor/Vocalia.Editor/Vocalia.Editor.csproj", "src/apis/Editor/Vocalia.Editor/"]
COPY ["src/services/Vocalia.Streams/Vocalia.Streams.csproj", "src/services/Vocalia.Streams/"]
COPY ["src/services/Vocalia.Audio/Vocalia.Audio.csproj", "src/services/Vocalia.Audio/"]
COPY ["src/services/Vocalia.ServiceBus.Types/Vocalia.ServiceBus.Types.csproj", "src/services/Vocalia.ServiceBus.Types/"]
COPY ["src/services/Vocalia.UserFacade/Vocalia.UserFacade.csproj", "src/services/Vocalia.UserFacade/"]
COPY ["src/apis/Editor/Vocalia.Editor.Db/Vocalia.Editor.Db.csproj", "src/apis/Editor/Vocalia.Editor.Db/"]
RUN dotnet restore "src/apis/Editor/Vocalia.Editor/Vocalia.Editor.csproj"
COPY . .
WORKDIR "/src/src/apis/Editor/Vocalia.Editor"
RUN dotnet build "Vocalia.Editor.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Vocalia.Editor.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Vocalia.Editor.dll"]