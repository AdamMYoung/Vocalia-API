FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["src/apis/Social/Vocalia.Social/Vocalia.Social.csproj", "src/apis/Social/Vocalia.Social/"]
COPY ["src/apis/Social/Vocalia.Social.Db/Vocalia.Social.Db.csproj", "src/apis/Social/Vocalia.Social.Db/"]
RUN dotnet restore "src/apis/Social/Vocalia.Social/Vocalia.Social.csproj"
COPY . .
WORKDIR "/src/src/apis/Social/Vocalia.Social"
RUN dotnet build "Vocalia.Social.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Vocalia.Social.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Vocalia.Social.dll"]