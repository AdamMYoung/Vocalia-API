FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/apis/Publishing/Vocalia.Publishing/Vocalia.Publishing.csproj", "src/apis/Publishing/Vocalia.Publishing/"]
COPY ["src/apis/Publishing/Vocalia.Publishing.Db/Vocalia.Publishing.Db.csproj", "src/apis/Publishing/Vocalia.Publishing.Db/"]
COPY ["src/apis/Ingest/Vocalia.Ingest.Db/Vocalia.Ingest.Db.csproj", "src/apis/Ingest/Vocalia.Ingest.Db/"]
COPY ["src/services/Vocalia.ServiceBus.Types/Vocalia.ServiceBus.Types.csproj", "src/services/Vocalia.ServiceBus.Types/"]
COPY ["src/services/Vocalia.UserFacade/Vocalia.UserFacade.csproj", "src/services/Vocalia.UserFacade/"]
RUN dotnet restore "src/apis/Publishing/Vocalia.Publishing/Vocalia.Publishing.csproj"
COPY . .
WORKDIR "/src/src/apis/Publishing/Vocalia.Publishing"
RUN dotnet build "Vocalia.Publishing.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Vocalia.Publishing.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Vocalia.Publishing.dll"]