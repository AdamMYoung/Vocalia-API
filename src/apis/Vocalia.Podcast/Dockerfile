#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 5004

FROM microsoft/dotnet:2.1-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY ["src/apis/Vocalia.Podcast/Vocalia.Podcast.csproj", "src/apis/Vocalia.Podcast/"]
RUN dotnet restore "src/apis/Vocalia.Podcast/Vocalia.Podcast.csproj"
COPY . .
WORKDIR "/src/src/apis/Vocalia.Podcast"
RUN dotnet build "Vocalia.Podcast.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Vocalia.Podcast.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Vocalia.Podcast.dll"]