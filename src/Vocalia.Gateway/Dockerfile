FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["src/Vocalia.Gateway/Vocalia.Gateway.csproj", "src/Vocalia.Gateway/"]
RUN dotnet restore "src/Vocalia.Gateway/Vocalia.Gateway.csproj"
COPY . .
WORKDIR "/src/src/Vocalia.Gateway"
RUN dotnet build "Vocalia.Gateway.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Vocalia.Gateway.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Vocalia.Gateway.dll"]