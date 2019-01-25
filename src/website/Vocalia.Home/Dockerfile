#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 5101

FROM microsoft/dotnet:2.1-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY ["Vocalia.Home/Vocalia.Home.csproj", "Vocalia.Home/"]
RUN dotnet restore "Vocalia.Home/Vocalia.Home.csproj"
COPY . .
WORKDIR "/src/Vocalia.Home"
RUN dotnet build "Vocalia.Home.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Vocalia.Home.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Vocalia.Home.dll"]