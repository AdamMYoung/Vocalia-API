FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["src/apis/Ingest/Vocalia.Ingest/Vocalia.Ingest.csproj", "src/apis/Ingest/Vocalia.Ingest/"]
RUN dotnet restore "src/apis/Ingest/Vocalia.Ingest/Vocalia.Ingest.csproj"
COPY . .
WORKDIR "/src/src/apis/Ingest/Vocalia.Ingest"
RUN dotnet build "Vocalia.Ingest.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Vocalia.Ingest.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Vocalia.Ingest.dll"]